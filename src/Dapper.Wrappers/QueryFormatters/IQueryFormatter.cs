using System;
using System.Collections.Generic;

namespace Dapper.Wrappers.QueryFormatters
{
    public interface IQueryFormatter<T>
    {
        List<string> FormatOperations<TOpMetadata>(IQueryContext context,
            IEnumerable<QueryOperation> operations, IDictionary<string, TOpMetadata> operationMetadata,
            Func<string, IEnumerable<string>, OrderDirections?, string> formatOperation,
            Action<TOpMetadata, int, T> operationAction, T operationActionState, bool checkOrdering = false,
            bool useUniqueVariables = true) where TOpMetadata : QueryOperationMetadata;
    }
}
