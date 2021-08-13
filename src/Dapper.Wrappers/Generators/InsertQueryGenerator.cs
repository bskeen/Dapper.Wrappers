// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using Dapper.Wrappers.Formatters;
using System;
using System.Collections.Generic;

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

        protected InsertQueryGenerator(IQueryFormatter queryFormatter)
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

            void InsertOperationAction(MergeOperationMetadata metadata)
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
