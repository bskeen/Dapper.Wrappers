using Dapper.Wrappers.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dapper.Wrappers.Analyzers.Common
{
    public static class IncrementalGeneratorInitializationContextExtensions
    {
        private const string GenerationNamespace = "Dapper.Wrappers.Generation";
        private static readonly ISet<string> _attributesToProcess = new HashSet<string>(new[]
        {
            $"{GenerationNamespace}.{nameof(TrackUpdatesAttribute)}",
            $"{GenerationNamespace}.{nameof(QueryBuilderFilterAttribute)}",
            $"{GenerationNamespace}.{nameof(QueryBuilderInsertAttribute)}",
            $"{GenerationNamespace}.{nameof(QueryBuilderOrderingAttribute)}",
            $"{GenerationNamespace}.{nameof(QueryBuilderUpdateAttribute)}"
        });

        private static readonly ClassPropertyDataComparer _classPropertyComparer = new ClassPropertyDataComparer(new PropertyDataComparer());

        public static bool ShouldProcessAttribute(string attributeName) => _attributesToProcess.Contains(attributeName);

        public static IncrementalValuesProvider<ClassDeclarationSyntax> GetClassDeclarations(this IncrementalGeneratorInitializationContext context)
        {
            return context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: GetSemanticTargetForGeneration)
                .Where(static c => c is not null)!;
        }

        public static void AddTrackUpdatesClasses(this IncrementalGeneratorInitializationContext context)
        {
            var classDeclarations = GetClassDeclarations(context);

            IncrementalValuesProvider<(ClassDeclarationSyntax declaration, Compilation compilation)> compilationWithClassDeclarations = classDeclarations.Combine(context.CompilationProvider);

            var classPropertyInfo = compilationWithClassDeclarations
                .Select(static (compilationAndClass, cancellation) => GetClassPropertyData(compilationAndClass.declaration, compilationAndClass.compilation, cancellation))
                .Where(static classProperties => classProperties.Properties.Count > 0)
                .WithComparer(_classPropertyComparer);

            var propertyImplementations = classPropertyInfo
                .Select(GetPropertyImplementations)
                .WithComparer(new ClassPropertyImplementationComparer());

            var classImplementations = propertyImplementations.Select(GenerateClassImplementation);

            context.RegisterSourceOutput(classImplementations, (context, source) => context.AddSource($"{source.Namespace}.{source.Name}.cs", source.Implementation));
        }

        static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
            node is ClassDeclarationSyntax c && c.AttributeLists.Count > 0;

        static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var attribute in attributeListSyntax.Attributes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    if (ShouldProcessAttribute(fullName))
                    {
                        return classDeclarationSyntax;
                    }
                }
            }

            return null;
        }

        static ClassPropertyData GetClassPropertyData(ClassDeclarationSyntax classDeclaration, Compilation compilation, CancellationToken cancellationToken)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classUsings = classDeclaration.SyntaxTree.GetCompilationUnitRoot().Usings;
            var classUsingStrings = new string[classUsings.Count];
            var currentIndex = 0;

            var classSymbolInfo = semanticModel.GetDeclaredSymbol(classDeclaration);

            if (classSymbolInfo is null)
            {
                return default(ClassPropertyData);
            }

            var namespaceName = classSymbolInfo.ContainingNamespace.ToDisplayString();

            foreach (var usingStatement in classUsings)
            {
                classUsingStrings[currentIndex++] = usingStatement.ToString();
            }

            List<PropertyData> propertyData = new List<PropertyData>();

            foreach (var member in classDeclaration.Members)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (member is not PropertyDeclarationSyntax propertyDeclaration)
                {
                    continue;
                }

                var currentPropertyData = GetPropertyData(propertyDeclaration, semanticModel, cancellationToken);

                if (!currentPropertyData.hasData)
                {
                    continue;
                }

                propertyData.Add(new PropertyData(currentPropertyData.propertyName, currentPropertyData.propertyType));
            }

            return new ClassPropertyData(namespaceName, classDeclaration.Identifier.ToString(), classUsingStrings, propertyData);
        }

        private static (bool hasData, string propertyName, string propertyType) GetPropertyData(PropertyDeclarationSyntax property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var propertySymbol = semanticModel.GetDeclaredSymbol(property);
            
            if (propertySymbol == null || !propertySymbol.IsVirtual)
            {
                return (false, string.Empty, string.Empty);
            }

            var propertyTypeName = property.Type.ToString();

            foreach (var attributeList in property.AttributeLists)
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var attribute in attributeList.Attributes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (semanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    if (fullName == $"{GenerationNamespace}.{nameof(IgnoreUpdatesAttribute)}")
                    {
                        return (false, string.Empty, string.Empty);
                    }
                }
            }

            return (true, property.Identifier.ToString(), propertyTypeName);
        }

        static ClassPropertyImplementation GetPropertyImplementations(ClassPropertyData classPropertyData, CancellationToken cancellationToken)
        {
            var implementations = new string[classPropertyData.Properties.Count];

            for (int i = 0; i < classPropertyData.Properties.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                implementations[i] = GetOverriddenPropertyImplementation(classPropertyData.Properties[i]);
            }

            return new ClassPropertyImplementation(
                classPropertyData.Namespace,
                classPropertyData.ClassName,
                classPropertyData.ClassUsings,
                implementations);
        }

        private static string GetOverriddenPropertyImplementation(PropertyData propertyData)
        {
            return $@"        private bool _is{propertyData.Name}Set;
        public override {propertyData.TypeName} {propertyData.Name}
        {{
            get
            {{
                return base.{propertyData.Name};
            }}

            set
            {{
                _is{propertyData.Name}Set = true;
                base.{propertyData.Name} = value;
            }}
        }}

        public bool Is{propertyData.Name}Set
        {{
            get
            {{
                return _is{propertyData.Name}Set;
            }}
        }}";
        }

        static ClassImplementation GenerateClassImplementation(ClassPropertyImplementation propertyImplementation, CancellationToken cancellationToken)
        {
            var implementation = $@"{string.Join("\r\n", propertyImplementation.ClassUsings)}

namespace {propertyImplementation.ClassNamespace}
{{
    public class {propertyImplementation.ClassName}Tracked : {propertyImplementation.ClassName}
    {{
{string.Join("\r\n\r\n", propertyImplementation.PropertyImplementations)}
    }}
}}";

            return new ClassImplementation(propertyImplementation.ClassNamespace, $"{propertyImplementation.ClassName}Tracked", propertyImplementation.ClassUsings, implementation);
        }
    }
}
