namespace Dapper.Wrappers.Analyzers.Common
{
    readonly internal struct ClassPropertyImplementation
    {
        public string ClassNamespace { get; }
        public string ClassName { get; }
        public string[] ClassUsings { get; }
        public string[] PropertyImplementations { get; }

        public ClassPropertyImplementation(string classNamespace, string className, string[] classUsings, string[] propertyImplementations)
        {
            ClassNamespace = classNamespace;
            ClassName = className;
            ClassUsings = classUsings;
            PropertyImplementations = propertyImplementations;
        }
    }
}
