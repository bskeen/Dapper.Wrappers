namespace Dapper.Wrappers.Analyzers.Common
{
    internal class ClassPropertyImplementationComparer : IEqualityComparer<ClassPropertyImplementation>
    {
        public bool Equals(ClassPropertyImplementation x, ClassPropertyImplementation y)
        {
            return
                x.ClassNamespace == y.ClassNamespace &&
                x.ClassName == y.ClassName &&
                x.ClassUsings.SequenceEqual(y.ClassUsings) &&
                x.PropertyImplementations.SequenceEqual(y.PropertyImplementations);
        }

        public int GetHashCode(ClassPropertyImplementation obj)
        {
            var hashCode = new HashCode();

            hashCode.Add(obj.ClassNamespace);
            hashCode.Add(obj.ClassName);

            for (int i = 0; i < obj.ClassUsings.Length; i++)
            {
                hashCode.Add(obj.ClassUsings[i]);
            }

            for (int i = 0;i < obj.PropertyImplementations.Length; i++)
            {
                hashCode.Add(obj.PropertyImplementations[i]);
            }

            return hashCode.ToHashCode();
        }
    }
}
