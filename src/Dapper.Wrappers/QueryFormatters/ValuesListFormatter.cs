using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.QueryFormatters
{
    public class ValuesListFormatter : QueryFormatter<ValuesListFormatterState>, IValuesListFormatter
    {
        private readonly IQueryOperationFormatter _queryOperationFormatter;

        public ValuesListFormatter(IQueryOperationFormatter queryOperationFormatter)
        {
            _queryOperationFormatter = queryOperationFormatter;
        }

        public (string formattedValuesList, IEnumerable<MergeOperationMetadata> orderedMetadata) FormatValuesLists(
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
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var currentColumns = new HashSet<string>();
            var operationOrder = new List<MergeOperationMetadata>();
            var valuesLists = new List<string>();

            var state = new ValuesListFormatterState
            {
                RequiredMetadata = requiredOperations,
                OrderedMetadata = operationOrder,
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
                var orderedOperations = GetOrderedOperations(operationsList, state.OrderedMetadata, valuesListMetadata,
                    defaultOperations);

                formattedOperations = FormatOperations(context, orderedOperations, valuesListMetadata,
                    GetNonOrderingFormatOperation(_queryOperationFormatter.FormatInsertOperation),
                    InsertOperationAction, state);

                valuesLists.Add(_queryOperationFormatter.FormatInsertOperations(formattedOperations));
            }

            var combinedOperationsLists = _queryOperationFormatter.FormatMultipleInsertValuesLists(valuesLists);

            return (combinedOperationsLists, state.OrderedMetadata);
        }

        private void InsertOperationAction(MergeOperationMetadata metadata, int index, ValuesListFormatterState state)
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
                state.OrderedMetadata.Add(metadata);
            }
            else
            {
                if (state.OrderedMetadata[index].ReferencedColumn != metadata.ReferencedColumn)
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
        private void AddRequiredInsertOperations(IQueryContext context, ValuesListFormatterState state,
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

                state.OrderedMetadata.Add(requiredOperation.Value);
                formattedOperations.Add(_queryOperationFormatter.FormatInsertOperation(requiredOperation.Value.BaseQueryString, parameterNames));
            }
        }

        private IEnumerable<QueryOperation> GetOrderedOperations(IEnumerable<QueryOperation> operations,
            List<MergeOperationMetadata> columnOrder, IDictionary<string, MergeOperationMetadata> valuesListMetadata,
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
                if (operationLookup.TryGetValue(column.ReferencedColumn, out var operation))
                {
                    yield return operation;
                }
                else if (defaultOperations.TryGetValue(column.ReferencedColumn, out var defaultOperation))
                {
                    yield return defaultOperation;
                }
                else
                {
                    throw new ArgumentException($"Value must be supplied for '{column.ReferencedColumn}'.");
                }
            }
        }
    }
}
