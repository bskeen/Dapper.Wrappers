// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using Dapper.Wrappers.Formatters;
using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run a get query.
    /// </summary>
    /// <typeparam name="M">The type of model being returned.</typeparam>
    public abstract class BaseDeleteQueryGenerator<M> : BaseFilterableQueryGenerator, IDeleteQueryGenerator<M>
    {
        /// <summary>
        /// Returns the base delete query string.
        /// </summary>
        protected abstract string DeleteQueryString { get; }

        protected BaseDeleteQueryGenerator(IQueryFormatter queryFormatter, IQueryResultsProcessorProvider resultsProcessorProvider)
            : base(queryFormatter, resultsProcessorProvider)
        {
        }

        /// <summary>
        /// Adds a delete query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="deleteCriteria">The filter describing which items should be deleted.</param>
        /// <returns>The query results processor that will provide the results.</returns>
        public virtual IQueryResultsProcessor<M> AddDeleteQuery(IQueryContext context, IEnumerable<QueryOperation> deleteCriteria)
        {
            string criteria = FormatFilterOperations(context, deleteCriteria);

            var query = QueryFormatter.FormatDeleteQuery(DeleteQueryString, criteria);

            var resultsHandler = ResultsProcessorProvider.GetQueryResultsProcessor<M>();

            context.AddQuery(query, resultsHandler);

            return resultsHandler;
        }
    }
}
