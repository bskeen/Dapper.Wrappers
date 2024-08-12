using System;
using System.Collections.Generic;

namespace Dapper.Wrappers.Generation
{
    public interface IDynamicTypeLookupProvider
    {
        IReadOnlyDictionary<Type, Type> DynamicTypeLookup { get; }
    }
}
