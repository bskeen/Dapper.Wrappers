// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Used to process the results from a GridReader so they are in the
    /// correct form to be used by the consumer.
    /// </summary>
    /// <typeparam name="M">The type of model to be processed.</typeparam>
    public interface IQueryResultsProcessor<M> : IQueryResultsHandler
    {
        /// <summary>
        /// Processes a collection of query results.
        /// </summary>
        /// <returns>
        /// A list of processed query results, along with the total number of results present in the database.
        /// </returns>
        (IEnumerable<M> Results, int TotalResultCount) GetAllResults();

        /// <summary>
        /// Processes a single query result.
        /// </summary>
        /// <returns>A processed query result.</returns>
        M GetOneResult();
    }
}
