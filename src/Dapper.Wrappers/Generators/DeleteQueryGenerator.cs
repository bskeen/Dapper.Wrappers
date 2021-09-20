﻿// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run a get query.
    /// </summary>
    public abstract class DeleteQueryGenerator : FilterableQueryGenerator, IDeleteQueryGenerator
    {
        /// <summary>
        /// Returns the base delete query string.
        /// </summary>
        protected abstract string DeleteQueryString { get; }

        protected DeleteQueryGenerator(IQueryOperationFormatter queryFormatter)
            : base(queryFormatter)
        {
        }

        /// <summary>
        /// Adds a delete query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="deleteCriteria">The filter describing which items should be deleted.</param>
        /// <returns>The query results processor that will provide the results.</returns>
        public virtual void AddDeleteQuery(IQueryContext context, IEnumerable<QueryOperation> deleteCriteria)
        {
            string criteria = FormatFilterOperations(context, deleteCriteria);

            var query = QueryFormatter.FormatDeleteQuery(DeleteQueryString, criteria);

            context.AddQuery(query);
        }
    }
}
