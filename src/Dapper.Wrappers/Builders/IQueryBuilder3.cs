﻿// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Builders
{
    /// <summary>
    /// Defines a query builder type that includes methods to use operation types to build the query.
    /// </summary>
    /// <typeparam name="TBuilderContext">
    /// The type containing the state that needs to be shared between calls to GetFormattedOperations.
    /// </typeparam>
    /// <typeparam name="TOperations1">The first type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations2">The second type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations3">The third type used to get the query operations.</typeparam>
    public interface IQueryBuilder<TBuilderContext, in TOperations1, in TOperations2, in TOperations3> : IQueryBuilder
    {
        /// <summary>
        /// Adds a query to the given QueryContext, using the given query operations.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="queryOperations1">The first operations to include in the query.</param>
        /// <param name="queryOperations2">The second operations to include in the query.</param>
        /// <param name="queryOperations3">The third operations to include in the query.</param>
        void AddQueryToContext(IQueryContext context, ParsedQueryOperations queryOperations1,
            ParsedQueryOperations queryOperations2, ParsedQueryOperations queryOperations3);

        /// <summary>
        /// Adds a query to the given QueryContext, using the given query operations.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="operationObject1">The first operations to include in the query.</param>
        /// <param name="operationObject2">The second operations to include in the query.</param>
        /// <param name="operationObject3">The third operations to include in the query.</param>
        void AddQueryToContext(IQueryContext context, TOperations1 operationObject1, TOperations2 operationObject2,
            TOperations3 operationObject3);

        /// <summary>
        /// Gets a default builder context to use while building.
        /// </summary>
        /// <returns>A default builder context to use while building.</returns>
        TBuilderContext InitializeContext();

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject1(TOperations1 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations1(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject2(TOperations2 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations2(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject3(TOperations3 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations3(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);
    }
}
