// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run an update query.
    /// </summary>
    /// <typeparam name="M">The type of model being returned.</typeparam>
    public interface IUpdateQueryGenerator<M>
    {
        /// <summary>
        /// Adds an update query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="updateOperations">The values to update.</param>
        /// <param name="updateCriteria">The filter describing which items should be updated.</param>
        /// <returns>The query results processor that will provide the results.</returns>
        IQueryResultsProcessor<M> AddUpdateQuery(IQueryContext context, IEnumerable<QueryOperation> updateOperations,
            IEnumerable<QueryOperation> updateCriteria);
    }
}
