using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.QueryFormatters
{
    public class InsertColumnsFormatter : QueryFormatter<object>, IInsertColumnsFormatter
    {
        private readonly IQueryOperationFormatter _queryOperationFormatter;

        public InsertColumnsFormatter(IQueryOperationFormatter queryOperationFormatter)
        {
            _queryOperationFormatter = queryOperationFormatter;
        }

        public string FormatInsertColumns(IEnumerable<QueryOperation> insertOperations,
            IDictionary<string, MergeOperationMetadata> operationMetadata)
        {
            var alreadyReferencedColumns = new HashSet<string>();
            var formattedColumns = new List<string>();

            foreach (var insertOperation in insertOperations)
            {
                if (!operationMetadata.TryGetValue(insertOperation.Name, out var metadata))
                {
                    continue;
                }

                if (alreadyReferencedColumns.Contains(metadata.ReferencedColumn))
                {
                    throw new ArgumentException("Cannot insert multiple values into the same column.");
                }

                alreadyReferencedColumns.Add(metadata.ReferencedColumn);
                formattedColumns.Add(_queryOperationFormatter.FormatInsertColumn(metadata.ReferencedColumn));
            }

            return _queryOperationFormatter.FormatInsertColumns(formattedColumns);
        }

        public string FormatInsertColumns(IEnumerable<string> columns)
        {
            var formattedColumns = columns.Select(c => _queryOperationFormatter.FormatInsertColumn(c));
            return _queryOperationFormatter.FormatInsertColumns(formattedColumns);
        }
    }
}
