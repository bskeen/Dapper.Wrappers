// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Provides the base functionality for an IQueryResultsProcessor.
    /// </summary>
    /// <typeparam name="M">The type of model to be returned.</typeparam>
    public abstract class BaseQueryResultsProcessor<M> : IQueryResultsProcessor<M>
    {
        /// <summary>
        /// A TaskCompletionSource used to return results when available.
        /// </summary>
        protected TaskCompletionSource<(IEnumerable<M> Results, int TotalResultCount)> ResultsReadySource;

        protected BaseQueryResultsProcessor()
        {
            ResultsReadySource = new TaskCompletionSource<(IEnumerable<M> Results, int TotalResultCount)>();
        }

        /// <summary>
        /// Returns all results, with a count of how many results are available to be returned (excluding pagination).
        /// </summary>
        /// <returns>All query results, along with a count of available results.</returns>
        public async Task<(IEnumerable<M> Results, int TotalResultCount)> GetAllResultsAsync()
        {
            return await ResultsReadySource.Task;
        }

        /// <summary>
        /// Returns the first result from the query.
        /// </summary>
        /// <returns>The first result from the query.</returns>
        public async Task<M> GetOneResultAsync()
        {
            var results = await ResultsReadySource.Task;
            return results.Item1.FirstOrDefault();
        }

        /// <summary>
        /// Called by the QueryContext when executing the query. This method is used to read and process
        /// results from the queries.
        /// </summary>
        /// <param name="resultsReader">The GridReader to read results from.</param>
        public abstract void ReadResults(SqlMapper.GridReader resultsReader);
    }
}
