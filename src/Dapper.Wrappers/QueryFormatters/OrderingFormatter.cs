using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.QueryFormatters
{
    public class OrderingFormatter : QueryFormatter<object>, IOrderingFormatter
    {
        private readonly IQueryOperationFormatter _queryOperationFormatter;

        public OrderingFormatter(IQueryOperationFormatter queryOperationFormatter)
        {
            _queryOperationFormatter = queryOperationFormatter;
        }

        public string FormatOrderOperations(IQueryContext context, IDictionary<string, QueryOperationMetadata> operationMetadata, string defaultOrdering,
            IEnumerable<QueryOperation> orderOperations = null, Pagination pagination = null)
        {
            string formattedPagination = null;
            if (pagination != null)
            {
                var skipVariable =
                    _queryOperationFormatter.FormatVariable(context.AddVariable("skip", pagination.Skip,
                        System.Data.DbType.Int32));
                var takeVariable =
                    _queryOperationFormatter.FormatVariable(context.AddVariable("take", pagination.Take,
                        System.Data.DbType.Int32));

                formattedPagination = _queryOperationFormatter.FormatPagination(skipVariable, takeVariable);
            }

            var formattedOrderItems = FormatOperations(context, orderOperations, operationMetadata,
                _queryOperationFormatter.FormatOrderOperation, NoopOperationAction, null, true);

            if (formattedOrderItems.Count == 0)
            {
                return pagination is null
                    ? string.Empty
                    : _queryOperationFormatter.FormatOrderOperations(new[] {defaultOrdering}, formattedPagination);
            }

            return _queryOperationFormatter.FormatOrderOperations(formattedOrderItems, formattedPagination);
        }
    }
}
