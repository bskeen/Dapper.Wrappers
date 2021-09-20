using System.Collections.Generic;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.QueryFormatters
{
    public class FilterFormatter : QueryFormatter<object>, IFilterFormatter
    {
        private readonly IQueryOperationFormatter _operationFormatter;

        public FilterFormatter(IQueryOperationFormatter operationFormatter)
        {
            _operationFormatter = operationFormatter;
        }

        public string FormatFilterOperations(IQueryContext queryContext, IDictionary<string, QueryOperationMetadata> operationMetadata,
            IEnumerable<QueryOperation> filterOperations = null)
        {
            var formattedFilterItems = FormatOperations(queryContext, filterOperations, operationMetadata,
                GetNonOrderingFormatOperation(_operationFormatter.FormatFilterOperation), NoopOperationAction, null);

            if (formattedFilterItems.Count == 0)
            {
                return string.Empty;
            }

            return _operationFormatter.FormatFilterOperations(formattedFilterItems);
        }
    }
}
