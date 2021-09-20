using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.QueryFormatters
{
    public interface IInsertColumnsFormatter
    {
        string FormatInsertColumns(IEnumerable<QueryOperation> insertOperations,
            IDictionary<string, MergeOperationMetadata> operationMetadata);
    }
}
