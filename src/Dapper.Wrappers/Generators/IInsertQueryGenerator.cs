// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run an insert query.
    /// </summary>
    /// <typeparam name="M">The type of model being returned.</typeparam>
    public interface IInsertQueryGenerator<M>
    {
        /// <summary>
        /// Adds an insert query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="insertOperations">
        /// The objects to be inserted into the database.
        /// </param>
        /// <returns>The query results processor that will provide the results.</returns>
        IQueryResultsProcessor<M> AddInsertQuery(IQueryContext context, IEnumerable<QueryOperation> insertOperations);
    }
}
