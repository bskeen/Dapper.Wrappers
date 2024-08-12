namespace Dapper.Wrappers.Analyzers.Common
{
    internal class ClassPropertyDataComparer : IEqualityComparer<ClassPropertyData>
    {
        private readonly IEqualityComparer<PropertyData> _propertyDataComparer;

        public ClassPropertyDataComparer(IEqualityComparer<PropertyData> propertyDataComparer)
        {
            _propertyDataComparer = propertyDataComparer;
        }

        public bool Equals(ClassPropertyData x, ClassPropertyData y)
        {
            return
                x.Namespace == y.Namespace &&
                x.ClassName == y.ClassName &&
                x.ClassUsings.SequenceEqual(y.ClassUsings) &&
                x.Properties.SequenceEqual(y.Properties, _propertyDataComparer);

            
        }

        public int GetHashCode(ClassPropertyData obj)
        {
            var hash = new HashCode();
            hash.Add(obj.Namespace);
            hash.Add(obj.ClassName);

            for (int i = 0; i < obj.ClassUsings.Length; i++)
            {
                hash.Add(obj.ClassUsings[i]);
            }

            for (int i = 0; i < obj.Properties.Count; i++)
            {
                hash.Add(_propertyDataComparer.GetHashCode(obj.Properties[i]));
            }

            return hash.ToHashCode();
        }
    }
}
