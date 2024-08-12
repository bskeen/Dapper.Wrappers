namespace Dapper.Wrappers.Analyzers.Common
{
    internal class PropertyDataComparer : IEqualityComparer<PropertyData>
    {
        public bool Equals(PropertyData x, PropertyData y)
        {
            return x.Name == y.Name && x.TypeName == y.TypeName;
        }

        public int GetHashCode(PropertyData obj)
        {
            return HashCode.Combine(obj.Name, obj.TypeName);
        }
    }
}
