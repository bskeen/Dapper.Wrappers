using System.Collections.Generic;

namespace Dapper.Wrappers.QueryFormatters
{
    public interface IFilterFormatter
    {
        /// <summary>
        /// Formats the given filter operations or an empty string if none are provided.
        /// </summary>
        /// <param name="queryContext">The context to be updated.</param>
        /// <param name="operationMetadata">The metadata to use when formatting the input operations.</param>
        /// <param name="filterOperations">The filter operations to be applied to the results.</param>
        /// <returns></returns>
        string FormatFilterOperations(IQueryContext queryContext,
            IDictionary<string, QueryOperationMetadata> operationMetadata,
            IEnumerable<QueryOperation> filterOperations = null);
    }
}
