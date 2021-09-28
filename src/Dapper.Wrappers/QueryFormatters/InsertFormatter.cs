using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.QueryFormatters
{
    public class InsertFormatter : QueryFormatter<InsertFormatterState>, IInsertFormatter
    {
        private readonly IQueryOperationFormatter _queryOperationFormatter;

        public InsertFormatter(IQueryOperationFormatter queryOperationFormatter)
        {
            _queryOperationFormatter = queryOperationFormatter;
        }

        public (string formattedColumnsList, string formattedValuesList) FormatInsertPieces(
            IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> valuesListOperations,
            IDictionary<string, MergeOperationMetadata> valuesListMetadata,
            IDictionary<string, QueryOperation> defaultOperations)
        {
            var operationsLists = valuesListOperations?.Select(vlo => vlo?.ToList())
                .OrderByDescending(vlo => vlo?.Count ?? 0).ToList();

            var firstOperations = operationsLists?.FirstOrDefault();

            if (firstOperations is null)
            {
                throw new ArgumentException("Insert operations cannot be empty.");
            }

            var requiredOperations = valuesListMetadata.Where(kvp => kvp.Value.IsRequired)
                .ToDictionary(kvp => kvp.Value.ReferencedColumn, kvp => kvp.Value);

            var currentColumns = new HashSet<string>();
            var columnOrder = new List<string>();
            var valuesLists = new List<string>();

            var state = new InsertFormatterState
            {
                RequiredMetadata = requiredOperations,
                ColumnOrder = columnOrder,
                AlreadyReferencedColumns = currentColumns,
                IsFirstList = true
            };

            var formattedOperations = FormatOperations(context, firstOperations, valuesListMetadata,
                GetNonOrderingFormatOperation(_queryOperationFormatter.FormatInsertOperation), InsertOperationAction, state);

            AddRequiredInsertOperations(context, state, formattedOperations);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No values list operations specified.");
            }

            valuesLists.Add(_queryOperationFormatter.FormatInsertOperations(formattedOperations));

            state.IsFirstList = false;

            foreach (var operationsList in operationsLists.Skip(1))
            {
                var orderedOperations = GetOrderedOperations(operationsList, state.ColumnOrder, valuesListMetadata,
                    defaultOperations);

                formattedOperations = FormatOperations(context, orderedOperations, valuesListMetadata,
                    GetNonOrderingFormatOperation(_queryOperationFormatter.FormatInsertOperation),
                    InsertOperationAction, state);

                valuesLists.Add(_queryOperationFormatter.FormatInsertOperations(formattedOperations));
            }

            var columns = columnOrder.Select(_queryOperationFormatter.FormatInsertColumn);
            var formattedColumns = _queryOperationFormatter.FormatInsertColumns(columns);
            var combinedOperationsLists = _queryOperationFormatter.FormatMultipleInsertValuesLists(valuesLists);

            return (formattedColumns, combinedOperationsLists);
        }

        private void InsertOperationAction(MergeOperationMetadata metadata, int index, InsertFormatterState state)
        {
            if (state.IsFirstList)
            {
                if (state.RequiredMetadata.ContainsKey(metadata.ReferencedColumn))
                {
                    state.RequiredMetadata.Remove(metadata.ReferencedColumn);
                }

                if (state.AlreadyReferencedColumns.Contains(metadata.ReferencedColumn))
                {
                    throw new ArgumentException("Cannot have multiple inserts into the same column.");
                }

                state.AlreadyReferencedColumns.Add(metadata.ReferencedColumn);
                state.ColumnOrder.Add(metadata.ReferencedColumn);
            }
            else
            {
                if (state.ColumnOrder[index] != metadata.ReferencedColumn)
                {
                    throw new ArgumentException("Columns must be created in the same order.");
                }
            }

            
        }

        /// <summary>
        /// Once the requested insert operations have been processed, adds any additional operations that
        /// are required for the insert.
        /// </summary>
        /// <param name="context">The query context to update.</param>
        /// <param name="requiredOperations">The dictionary of required insert operations, if any.</param>
        /// <param name="formattedColumnNames">The list of columns into which values will be inserted.</param>
        /// <param name="formattedOperations">The list of insert operations.</param>
        private void AddRequiredInsertOperations(IQueryContext context, InsertFormatterState state,
            List<string> formattedOperations)
        {
            foreach (var requiredOperation in state.RequiredMetadata)
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
                
                state.ColumnOrder.Add(requiredOperation.Value.ReferencedColumn);
                formattedOperations.Add(_queryOperationFormatter.FormatInsertOperation(requiredOperation.Value.BaseQueryString, parameterNames));
            }
        }

        private IEnumerable<QueryOperation> GetOrderedOperations(IEnumerable<QueryOperation> operations,
            List<string> columnOrder, IDictionary<string, MergeOperationMetadata> valuesListMetadata,
            IDictionary<string, QueryOperation> defaultOperations)
        {
            if (operations is null)
            {
                throw new ArgumentException("Cannot process a null operation.");
            }

            var operationLookup = new Dictionary<string, QueryOperation>();
            foreach (var operation in operations)
            {
                var metadata = valuesListMetadata[operation.Name];

                if (operationLookup.ContainsKey(metadata.ReferencedColumn))
                {
                    throw new ArgumentException("Cannot have multiple inserts into the same column.");
                }

                operationLookup[metadata.ReferencedColumn] = operation;
            }

            foreach (var column in columnOrder)
            {
                if (operationLookup.TryGetValue(column, out var operation))
                {
                    yield return operation;
                }
                else if (defaultOperations.TryGetValue(column, out var defaultOperation))
                {
                    yield return defaultOperation;
                }
                else
                {
                    throw new ArgumentException($"Value must be supplied for '{column}'.");
                }
            }
        }
    }
}
