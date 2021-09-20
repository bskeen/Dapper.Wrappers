using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.QueryFormatters
{
    public class ValuesListFormatter : QueryFormatter<List<MergeOperationMetadata>>, IValuesListFormatter
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

            var orderedMetadata = new List<MergeOperationMetadata>();

            var formattedOperations = FormatOperations(context, valuesListOperations, valuesListMetadata,
                GetNonOrderingFormatOperation(_queryOperationFormatter.FormatInsertOperation), InsertOperationAction, orderedMetadata);

            AddRequiredInsertOperations(context, requiredOperations, formattedOperations);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No insert operations specified.");
            }

            var columnList = QueryFormatter.FormatInsertColumns(formattedColumnNames);
            var operationList = QueryFormatter.FormatInsertOperations(formattedOperations);
        }

        private void InsertOperationAction(MergeOperationMetadata metadata, int index, List<MergeOperationMetadata> state)
        {
            if (requiredOperations.ContainsKey(metadata.ReferencedColumn))
            {
                requiredOperations.Remove(metadata.ReferencedColumn);
            }

            if (currentColumns.Contains(metadata.ReferencedColumn))
            {
                throw new ArgumentException("Cannot have multiple inserts into the same column.");
            }

            currentColumns.Add(metadata.ReferencedColumn);
            formattedColumnNames.Add(QueryFormatter.FormatInsertColumn(metadata.ReferencedColumn));
        }

        /// <summary>
        /// Once the requested insert operations have been processed, adds any additional operations that
        /// are required for the insert.
        /// </summary>
        /// <param name="context">The query context to update.</param>
        /// <param name="requiredOperations">The dictionary of required insert operations, if any.</param>
        /// <param name="formattedColumnNames">The list of columns into which values will be inserted.</param>
        /// <param name="formattedOperations">The list of insert operations.</param>
        private void AddRequiredInsertOperations(IQueryContext context,
            IDictionary<string, MergeOperationMetadata> requiredOperations, List<string> formattedColumnNames,
            List<string> formattedOperations)
        {
            foreach (var requiredOperation in requiredOperations)
            {
                List<string> parameterNames = new List<string>();

                foreach (var parameter in requiredOperation.Value.Parameters)
                {
                    if (!parameter.HasDefault)
                    {
                        throw new ArgumentException($"Value must be supplied for '{requiredOperation.Key}'.");
                    }

                    var variableName = context.AddVariable(parameter.Name, parameter.DefaultValue, parameter.DbType);

                    parameterNames.Add(variableName);
                }

                formattedColumnNames.Add(QueryFormatter.FormatInsertColumn(requiredOperation.Key));
                formattedOperations.Add(QueryFormatter.FormatInsertOperation(requiredOperation.Value.BaseQueryString, parameterNames));
            }
        }
    }
}
