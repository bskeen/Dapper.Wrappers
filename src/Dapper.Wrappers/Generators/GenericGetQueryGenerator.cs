// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Dapper.Wrappers.Formatters;

namespace Dapper.Wrappers.Generators
{
    public abstract class GenericGetQueryGenerator<TFilter, TOrder> : GetQueryGenerator,
        IGenericGetQueryGenerator<TFilter, TOrder>
    {
        protected GenericGetQueryGenerator(IQueryFormatter queryFormatter) : base(queryFormatter)
        {
        }

        public abstract IEnumerable<QueryOperation> GetFilterOperationsFromSource(TFilter source);

        public abstract IEnumerable<QueryOperation> GetOrderOperationsFromSource(TOrder source);

        public Pagination? GetPagination(int? skip, int? take, int? skipUpperThreshold = null,
            int? takeUpperThreshold = 100)
        {
            if (!skip.HasValue && !take.HasValue)
            {
                return null;
            }

            var skipAmount = Math.Max(skip ?? 0, 0);
            skipAmount = skipUpperThreshold.HasValue ? Math.Min(skipAmount, skipUpperThreshold.Value) : skipAmount;

            var takeAmount = Math.Max(take ?? 10, 0);
            takeAmount = takeUpperThreshold.HasValue ? Math.Min(takeAmount, takeUpperThreshold.Value) : takeAmount;

            return new Pagination
            {
                Skip = skipAmount,
                Take = takeAmount
            };
        }
    }
}
