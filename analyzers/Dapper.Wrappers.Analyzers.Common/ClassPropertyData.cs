namespace Dapper.Wrappers.Analyzers.Common
{
    readonly internal struct ClassPropertyData
    {
        public string Namespace { get; }
        public string ClassName { get; }
        public string[] ClassUsings { get; }
        public IReadOnlyList<PropertyData> Properties { get; }

        public ClassPropertyData()
        {
            Namespace = String.Empty;
            ClassName = String.Empty;
            ClassUsings = new string[0];
            Properties = new List<PropertyData>().AsReadOnly();
        }

        public ClassPropertyData(string classNamespace, string className, string[] classUsings, List<PropertyData> properties)
        {
            Namespace = classNamespace;
            ClassName = className;
            ClassUsings = classUsings;
            Properties = properties.AsReadOnly();
        }
    }
}
