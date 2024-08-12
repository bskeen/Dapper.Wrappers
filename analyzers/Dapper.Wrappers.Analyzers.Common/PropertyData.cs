namespace Dapper.Wrappers.Analyzers.Common
{
    readonly internal struct PropertyData
    {
        public string Name { get; }
        public string TypeName { get; }

        public PropertyData(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }
    }
}
