// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    public interface IGenericGetQueryGenerator<in TFilter, in TOrder> : IGetQueryGenerator
    {
        IEnumerable<QueryOperation> GetFilterOperationsFromSource(TFilter source);
        IEnumerable<QueryOperation> GetOrderOperationsFromSource(TOrder source);
        Pagination GetPagination(int? skip, int? take, int? skipUpperThreshold = null, int? takeUpperThreshold = 100);
    }
}
