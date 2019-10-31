// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Updates the context to add all the pieces to run different kinds
    /// of queries.
    /// </summary>
    public interface IQueryGenerator<M>
    {
        /// <summary>
        /// Adds a get query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="filterItems">Declares how to filter the results.</param>
        /// <param name="orderItems">Declares how to order the results.</param>
        /// <param name="pagination">Declares how to paginate the results</param>
        IQueryResultsProcessor<M> AddGetQuery(
            IQueryContext context,
            IEnumerable<FilterItem> filterItems = null,
            IEnumerable<OrderItem> orderItems = null,
            Pagination pagination = null);

        /// <summary>
        /// Adds an insert query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="entityValues">
        /// The objects to be inserted into the database.
        /// </param>
        IQueryResultsProcessor<M> AddInsertQuery(
            IQueryContext context,
            IEnumerable<IEnumerable<QueryKeyValue>> entityValues);

        /// <summary>
        /// Adds an update query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="entityUpdates">The values to update.</param>
        /// <param name="filterItems">The filter describing which items should be updated.</param>
        IQueryResultsProcessor<M> AddUpdateQuery(
            IQueryContext context,
            IEnumerable<QueryKeyValue> entityUpdates,
            IEnumerable<FilterItem> filterItems);

        /// <summary>
        /// Adds a delete query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="deleteCriteria">The filter describing which items should be deleted.</param>
        void AddDeleteQuery(
            IQueryContext context,
            IEnumerable<FilterItem> deleteCriteria);
    }
}
