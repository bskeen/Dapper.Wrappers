// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using Dapper.Wrappers.Formatters;
using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Provides common functionality necessary for query builders that allow dynamic filtration of results.
    /// </summary>
    public abstract class FilterableQueryGenerator : QueryGenerator
    {
        /// <summary>
        /// Specifies the possible filter operations to be applied to the results.
        /// </summary>
        protected abstract IDictionary<string, QueryOperationMetadata> FilterOperationMetadata { get; }

        protected FilterableQueryGenerator(IQueryFormatter queryFormatter)
            : base(queryFormatter)
        {
        }

        /// <summary>
        /// Formats the given filter operations or an empty string if none are provided.
        /// </summary>
        /// <param name="queryContext">The context to be updated.</param>
        /// <param name="filterOperations">The filter operations to be applied to the results.</param>
        /// <returns></returns>
        protected string FormatFilterOperations(IQueryContext queryContext,
            IEnumerable<QueryOperation> filterOperations = null)
        {
            var formattedFilterItems = FormatOperations(queryContext, filterOperations, FilterOperationMetadata,
                GetNonOrderingFormatOperation(QueryFormatter.FormatFilterOperation), NoopOperationAction);

            if (formattedFilterItems.Count == 0)
            {
                return string.Empty;
            }

            return QueryFormatter.FormatFilterOperations(formattedFilterItems);
        }
    }
}
