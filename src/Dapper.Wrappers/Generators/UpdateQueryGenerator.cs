// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using Dapper.Wrappers.Formatters;
using System;
using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run an update query.
    /// </summary>
    /// <typeparam name="M">The type of model being returned.</typeparam>
    public abstract class UpdateQueryGenerator : FilterableQueryGenerator, IUpdateQueryGenerator
    {
        /// <summary>
        /// Returns the base update query string.
        /// </summary>
        protected abstract string UpdateQueryString { get; }

        /// <summary>
        /// Specifies the possible update operations to be applied to the results.
        /// </summary>
        protected abstract IDictionary<string, MergeOperationMetadata> UpdateOperationMetadata { get; }

        protected UpdateQueryGenerator(IQueryFormatter queryFormatter)
            : base(queryFormatter)
        {
        }

        /// <summary>
        /// Adds an update query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="updateOperations">The values to update.</param>
        /// <param name="updateCriteria">The filter describing which items should be updated.</param>
        /// <returns>The query results processor that will provide the results.</returns>
        public virtual void AddUpdateQuery(IQueryContext context,
            IEnumerable<QueryOperation> updateOperations, IEnumerable<QueryOperation> updateCriteria)
        {
            var currentColumns = new HashSet<string>();

            var formattedOperations = FormatOperations(context, updateOperations, UpdateOperationMetadata,
                GetNonOrderingFormatOperation(QueryFormatter.FormatUpdateOperation), UpdateOperationAction);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No update operations specified.");
            }

            string operations = QueryFormatter.FormatUpdateOperations(formattedOperations);

            string criteria = FormatFilterOperations(context, updateOperations);

            var query = QueryFormatter.FormatUpdateQuery(UpdateQueryString, operations, criteria);

            context.AddQuery(query);

            void UpdateOperationAction(MergeOperationMetadata metadata)
            {
                if (currentColumns.Contains(metadata.ReferencedColumn))
                {
                    throw new ArgumentException("Cannot have multiple updates of the same column.");
                }

                currentColumns.Add(metadata.ReferencedColumn);
            }
        }
    }
}
