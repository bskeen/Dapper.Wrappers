using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.QueryFormatters
{
    public class ValuesListFormatter : QueryFormatter<object>, IValuesListFormatter
    {
        private readonly IQueryOperationFormatter _queryOperationFormatter;

        public ValuesListFormatter(IQueryOperationFormatter queryOperationFormatter)
        {
            _queryOperationFormatter = queryOperationFormatter;
        }

        public (string formattedValuesList, IEnumerable<MergeOperationMetadata> orderedMetadata) FormatValuesList(
            IQueryContext context, IEnumerable<QueryOperation> valuesListOperations,
            IDictionary<string, MergeOperationMetadata> valuesListMetadata,
            IDictionary<string, QueryOperation> defaultOperations)
        {
            if (valuesListOperations == null)
            {
                throw new ArgumentException("Insert operations cannot be null.");
            }

            var requiredOperations = valuesListMetadata.Where(metadata => metadata.Value.IsRequired)
                .ToDictionary(metadata => metadata.Key, metadata => metadata.Value);

            List<string> formattedColumnNames = new List<string>();

            var formattedOperations = FormatOperations(context, valuesListOperations, valuesListMetadata,
                GetNonOrderingFormatOperation(_queryOperationFormatter.FormatInsertOperation), InsertOperationAction);

            AddRequiredInsertOperations(context, requiredOperations, formattedColumnNames, formattedOperations);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No insert operations specified.");
            }

            var columnList = QueryFormatter.FormatInsertColumns(formattedColumnNames);
            var operationList = QueryFormatter.FormatInsertOperations(formattedOperations);
        }
    }
}
