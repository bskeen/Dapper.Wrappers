// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers
{
    public interface IQueryResultsProcessorProvider
    {
        IQueryResultsProcessor<M> GetQueryResultsProcessor<M>();
    }
}
