using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.QueryFormatters
{
    public interface IUpdateFormatter
    {
        string FormatUpdateOperations(IQueryContext context, IEnumerable<QueryOperation> updateOperations,
            IDictionary<string, MergeOperationMetadata> operationMetadata);
    }
}
