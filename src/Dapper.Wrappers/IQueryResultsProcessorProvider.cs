// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers
{
    /// <summary>
    /// Returns a new IQueryResultsProcessor of the given type.
    /// </summary>
    public interface IQueryResultsProcessorProvider
    {
        /// <summary>
        /// Returns a new IQueryResultsProcessor of the given type.
        /// </summary>
        /// <typeparam name="M">The type of model that should be processed by the IQueryResultsProcessor.</typeparam>
        /// <returns>A new IQueryResultsProcessor of the given type.</returns>
        IQueryResultsProcessor<M> GetQueryResultsProcessor<M>();
    }
}
