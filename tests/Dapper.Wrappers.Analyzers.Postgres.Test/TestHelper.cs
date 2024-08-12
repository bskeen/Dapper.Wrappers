using Dapper.Wrappers.Postgres;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;

namespace Dapper.Wrappers.Analyzers.Postgres.Test
{
    public static class TestHelper
    {
        public static Task Verify(string source)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            var references = new List<MetadataReference>();
            //Load project libraries
            var assemblies = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .Select(a => a.Location);
            foreach (var item in assemblies)
            {
                if (!item.Contains("xunit"))
                {
                    references.Add(MetadataReference.CreateFromFile(item));
                }
            }

            references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: references);

            var generator = new PostgresSourceGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGenerators(compilation);

            return Verifier
                .Verify(driver)
                .UseDirectory("Snapshots");
        }
    }
}
