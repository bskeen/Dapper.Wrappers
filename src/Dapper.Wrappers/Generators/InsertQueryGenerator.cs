// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run an insert query.
    /// </summary>
    public abstract class InsertQueryGenerator : QueryGenerator, IInsertQueryGenerator
    {
        /// <summary>
        /// Returns the base insert query string.
        /// </summary>
        protected abstract string InsertQueryString { get; }

        /// <summary>
        /// Specifies the possible insert operations to be applied to the results.
        /// </summary>
        protected abstract IDictionary<string, MergeOperationMetadata> InsertOperationMetadata { get; }

        /// <summary>
        /// Returns a new dictionary each time with a copy of the required insert operations. The keys used are the
        /// referenced column of the insert operation.
        /// </summary>
        /// <returns>A dictionary containing the required insert operations, indexed by the referenced column.</returns>
        protected abstract IDictionary<string, MergeOperationMetadata> GetRequiredInsertOperationMetadata();

        /// <summary>
        /// Default operations to be used in case an operation is missing from one of the values lists. The keys are
        /// the referenced column of the insert operation.
        /// </summary>
        protected abstract IDictionary<string, QueryOperation> DefaultOperations { get; }

        protected InsertQueryGenerator(IQueryOperationFormatter queryFormatter)
            : base(queryFormatter)
        {
        }

        /// <summary>
        /// Adds an insert query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="insertOperations">
        /// The objects to be inserted into the database.
        /// </param>
        /// <returns>The query results processor that will provide the results.</returns>
        public virtual void AddInsertQuery(IQueryContext context, IEnumerable<QueryOperation> insertOperations)
        {
            if (insertOperations == null)
            {
                throw new ArgumentException("Insert operations cannot be null.");
            }

            var requiredOperations = GetRequiredInsertOperationMetadata();

            var currentColumns = new HashSet<string>();

            List<string> formattedColumnNames = new List<string>();

            var formattedOperations = FormatOperations(context, insertOperations, InsertOperationMetadata,
                GetNonOrderingFormatOperation(QueryFormatter.FormatInsertOperation), InsertOperationAction);

            AddRequiredInsertOperations(context, requiredOperations, formattedColumnNames, formattedOperations);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No insert operations specified.");
            }

            var columnList = QueryFormatter.FormatInsertColumns(formattedColumnNames);
            var operationList = QueryFormatter.FormatInsertOperations(formattedOperations);
            var query = QueryFormatter.FormatInsertQuery(InsertQueryString, columnList, operationList);

            context.AddQuery(query);

            void InsertOperationAction(MergeOperationMetadata metadata, int index, bool firstList)
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
        }

        /// <summary>
        /// Adds an insert query with multiple values lists to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="insertOperations">The operations corresponding to the objects to be inserted into the database.</param>
        public void AddMultipleInsertQuery(IQueryContext context,
            IEnumerable<IEnumerable<QueryOperation>> insertOperations)
        {
            var operationsLists = insertOperations?.Select(io => io?.ToList()).OrderByDescending(io => io?.Count ?? 0)
                .ToList();

            var firstOperations = operationsLists?.FirstOrDefault();

            if (firstOperations == null)
            {
                throw new ArgumentException("Insert operations cannot be empty.");
            }

            var requiredOperations = GetRequiredInsertOperationMetadata();

            var currentColumns = new HashSet<string>();
            var columnOrder = new List<string>();
            var valuesLists = new List<string>();

            List<string> formattedColumnNames = new List<string>();

            var formattedOperations = FormatOperations(context, firstOperations, InsertOperationMetadata,
                GetNonOrderingFormatOperation(QueryFormatter.FormatInsertOperation), InsertOperationAction);

            AddRequiredInsertOperations(context, requiredOperations, formattedColumnNames, formattedOperations);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No insert operations specified.");
            }

            valuesLists.Add(QueryFormatter.FormatInsertOperations(formattedOperations));

            foreach (var operationsList in operationsLists.Skip(1))
            {
                var orderedOperations = GetOrderedOperations(operationsList, columnOrder);

                formattedOperations = FormatOperations(context, orderedOperations, InsertOperationMetadata,
                    GetNonOrderingFormatOperation(QueryFormatter.FormatInsertOperation), InsertOperationAction,
                    isFirstOperationList: false);

                valuesLists.Add(QueryFormatter.FormatInsertOperations(formattedOperations));
            }

            var columnList = QueryFormatter.FormatInsertColumns(formattedColumnNames);
            var operationList = QueryFormatter.FormatMultipleInsertValuesLists(valuesLists);
            var query = QueryFormatter.FormatInsertQuery(InsertQueryString, columnList, operationList);

            context.AddQuery(query);

            void InsertOperationAction(MergeOperationMetadata metadata, int index, bool firstList)
            {
                if (firstList)
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
                    columnOrder.Add(metadata.ReferencedColumn);
                    formattedColumnNames.Add(QueryFormatter.FormatInsertColumn(metadata.ReferencedColumn));
                }
                else
                {
                    if (columnOrder[index] != metadata.ReferencedColumn)
                    {
                        throw new ArgumentException("Columns must be created in the same order.");
                    }
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

        private IEnumerable<QueryOperation> GetOrderedOperations(IEnumerable<QueryOperation> operations,
            List<string> columnOrder)
        {
            if (operations is null)
            {
                throw new ArgumentException("Cannot process a null operation.");
            }

            var operationLookup = new Dictionary<string, QueryOperation>();
            foreach (var operation in operations)
            {
                var metadata = InsertOperationMetadata[operation.Name];

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
                else if (DefaultOperations.TryGetValue(column, out var defaultOperation))
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
