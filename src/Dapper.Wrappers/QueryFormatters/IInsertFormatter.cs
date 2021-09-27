using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.QueryFormatters
{
    public interface IInsertFormatter
    {
        (string formattedColumnsList, string formattedValuesList) FormatInsertPieces(
            IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> valuesListOperations,
            IDictionary<string, MergeOperationMetadata> valuesListMetadata,
            IDictionary<string, QueryOperation> defaultOperations);
    }
}
