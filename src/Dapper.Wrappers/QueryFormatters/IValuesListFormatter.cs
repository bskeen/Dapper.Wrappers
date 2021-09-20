using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.QueryFormatters
{
    public interface IValuesListFormatter
    {
        (string formattedValuesList, IEnumerable<MergeOperationMetadata> orderedMetadata) FormatValuesList(
            IQueryContext context, IEnumerable<QueryOperation> valuesListOperations,
            IDictionary<string, MergeOperationMetadata> valuesListMetadata,
            IDictionary<string, QueryOperation> defaultOperations);
    }
}
