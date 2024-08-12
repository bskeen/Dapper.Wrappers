using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Analyzers.Common
{
    readonly internal struct ClassImplementation
    {
        public string Namespace { get; }
        public string Name { get; }
        public string[] UsingDeclarations { get; }
        public string Implementation { get; }

        public ClassImplementation(string classNamespace, string name, string[] usingDeclarations, string implementation)
        {
            Namespace = classNamespace;
            Name = name;
            UsingDeclarations = usingDeclarations;
            Implementation = implementation;
        }
    }
}
