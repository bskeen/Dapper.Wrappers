// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Dapper.Wrappers.Builders
{
    /// <summary>
    /// Defines a query builder type that includes methods to use operation types to build the query.
    /// </summary>
    /// <typeparam name="TBuilderContext">
    /// The type containing the state that needs to be shared between calls to GetFormattedOperations.
    /// </typeparam>
    /// <typeparam name="TOperations">The type used to get the query operations.</typeparam>
    public abstract class QueryBuilder<TBuilderContext, TOperations> : QueryBuilder, IQueryBuilder<TBuilderContext, TOperations>
    {
        /// <summary>
        /// Adds a query to the given QueryContext, using the given query operations.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="queryOperations">The operations to include in the query.</param>
        public void AddQueryToContext(IQueryContext context, ParsedQueryOperations queryOperations)
        {
            var builderContext = InitializeContext();

            var formattedOperations = new List<object>();
            formattedOperations.AddRange(GetFormattedOperations(context, queryOperations, builderContext));

            var formattedQuery = string.Format(QueryFormat, formattedOperations.ToArray());
            
            context.AddQuery(formattedQuery);
        }

        /// <summary>
        /// Adds a query to the given QueryContext, using the given query operations.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="operationObject">The operations to include in the query.</param>
        public void AddQueryToContext(IQueryContext context, TOperations operationObject)
        {
            var operations = GetOperationsFromObject(operationObject);
            AddQueryToContext(context, operations);
        }

        /// <summary>
        /// Gets a default builder context to use while building.
        /// </summary>
        /// <returns>A default builder context to use while building.</returns>
        public abstract TBuilderContext InitializeContext();

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject(TOperations operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract IEnumerable<string> GetFormattedOperations(IQueryContext context,
            ParsedQueryOperations operations, TBuilderContext builderContext);
    }
}
