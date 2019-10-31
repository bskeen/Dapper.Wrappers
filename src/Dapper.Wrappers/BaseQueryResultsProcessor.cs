// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Wrappers
{
    public abstract class BaseQueryResultsProcessor<M> : IQueryResultsProcessor<M>
    {
        protected TaskCompletionSource<(IEnumerable<M>, int)> ResultsReadySource;

        protected BaseQueryResultsProcessor()
        {
            ResultsReadySource = new TaskCompletionSource<(IEnumerable<M>, int)>();
        }

        public async Task<(IEnumerable<M> Results, int TotalResultCount)> GetAllResultsAsync()
        {
            return await ResultsReadySource.Task;
        }

        public async Task<M> GetOneResultAsync()
        {
            var results = await ResultsReadySource.Task;
            return results.Item1.FirstOrDefault();
        }

        public abstract void ReadResults(SqlMapper.GridReader resultsReader);
    }
}
