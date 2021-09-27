using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.QueryFormatters
{
    public interface IOrderingFormatter
    {
        (string orderOperations, string pagination) FormatOrderOperations(IQueryContext context,
            IDictionary<string, QueryOperationMetadata> operationMetadata, string defaultOrdering,
            IEnumerable<QueryOperation> orderOperations = null, Pagination pagination = null);
    }
}
