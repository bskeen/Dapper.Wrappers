// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run a get query.
    /// </summary>
    /// <typeparam name="M">The type of model being returned.</typeparam>
    public interface IGetQueryGenerator<M>
    {
        /// <summary>
        /// Adds a get query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="filterItems">Declares how to filter the results.</param>
        /// <param name="orderItems">Declares how to order the results.</param>
        /// <param name="pagination">Declares how to paginate the results</param>
        /// <returns>The query results processor that will provide the results.</returns>
        void AddGetQuery(IQueryContext context,
            IEnumerable<QueryOperation> filterOperations = null, IEnumerable<QueryOperation> orderOperations = null,
            Pagination pagination = null);
    }
}
