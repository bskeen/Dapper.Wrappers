using System;
using System.Collections.Generic;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.QueryFormatters
{
    public class UpdateFormatter : QueryFormatter<HashSet<string>>, IUpdateFormatter
    {
        private readonly IQueryOperationFormatter _queryOperationFormatter;

        public UpdateFormatter(IQueryOperationFormatter queryOperationFormatter)
        {
            _queryOperationFormatter = queryOperationFormatter;
        }

        public string FormatUpdateOperations(IQueryContext context, IEnumerable<QueryOperation> updateOperations,
            IDictionary<string, MergeOperationMetadata> operationMetadata)
        {
            var currentColumns = new HashSet<string>();

            var formattedOperations = FormatOperations(context, updateOperations, operationMetadata,
                GetNonOrderingFormatOperation(_queryOperationFormatter.FormatUpdateOperation), UpdateOperationAction, currentColumns);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No update operations specified.");
            }

            return _queryOperationFormatter.FormatUpdateOperations(formattedOperations);
        }

        private void UpdateOperationAction(MergeOperationMetadata operationMetadata, int currentIndex, HashSet<string> currentColumns)
        {
            if (currentColumns.Contains(operationMetadata.ReferencedColumn))
            {
                throw new ArgumentException("Cannot have multiple updates of the same column.");
            }

            currentColumns.Add(operationMetadata.ReferencedColumn);
        }
    }
}
