// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    public interface IGenericUpdateQueryGenerator<in TFilter, in TUpdate> : IUpdateQueryGenerator
    {
        IEnumerable<QueryOperation> GetFilterOperationsFromSource(TFilter source);
        IEnumerable<QueryOperation> GetUpdateOperationsFromSource(TUpdate source);
    }
}
