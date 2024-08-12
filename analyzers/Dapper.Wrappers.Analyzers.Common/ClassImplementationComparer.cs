using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Analyzers.Common
{
    internal class ClassImplementationComparer : IEqualityComparer<ClassImplementation>
    {
        public bool Equals(ClassImplementation x, ClassImplementation y)
        {
            return x.Namespace == y.Namespace &&
                x.Name == y.Name &&
                x.UsingDeclarations.SequenceEqual(y.UsingDeclarations) &&
                x.Implementation == y.Implementation;
        }

        public int GetHashCode(ClassImplementation obj)
        {
            var hashCode = new HashCode();

            hashCode.Add(obj.Namespace);
            hashCode.Add(obj.Name);

            for (int i = 0; i < obj.UsingDeclarations.Length; i++)
            {
                hashCode.Add(obj.UsingDeclarations[i]);
            }

            hashCode.Add(obj.Implementation);

            return hashCode.ToHashCode();
        }
    }
}
