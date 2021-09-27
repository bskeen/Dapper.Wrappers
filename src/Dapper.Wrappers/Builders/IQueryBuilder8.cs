// © 2021 by Benjamin Skeen
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
    /// <typeparam name="TOperations4">The fourth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations5">The fifth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations6">The sixth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations7">The seventh type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations8">The eighth type used to get the query operations.</typeparam>
    public interface IQueryBuilder<TBuilderContext, in TOperations1, in TOperations2, in TOperations3, in TOperations4, in TOperations5, in TOperations6,
        in TOperations7, in TOperations8> : IQueryBuilder
    {
        /// <summary>
        /// Adds a query to the given QueryContext, using the given query operations.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="queryOperations1">The first operations to include in the query.</param>
        /// <param name="queryOperations2">The second operations to include in the query.</param>
        /// <param name="queryOperations3">The third operations to include in the query.</param>
        /// <param name="queryOperations4">The fourth operations to include in the query.</param>
        /// <param name="queryOperations5">The fifth operations to include in the query.</param>
        /// <param name="queryOperations6">The sixth operations to include in the query.</param>
        /// <param name="queryOperations7">The seventh operations to include in the query.</param>
        /// <param name="queryOperations8">The eighth operations to include in the query.</param>
        void AddQueryToContext(IQueryContext context, ParsedQueryOperations queryOperations1,
            ParsedQueryOperations queryOperations2,
            ParsedQueryOperations queryOperations3,
            ParsedQueryOperations queryOperations4,
            ParsedQueryOperations queryOperations5,
            ParsedQueryOperations queryOperations6,
            ParsedQueryOperations queryOperations7,
            ParsedQueryOperations queryOperations8);

        /// <summary>
        /// Adds a query to the given QueryContext, using the given query operations.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="operationObject1">The first operations to include in the query.</param>
        /// <param name="operationObject2">The second operations to include in the query.</param>
        /// <param name="operationObject3">The third operations to include in the query.</param>
        /// <param name="operationObject4">The fourth operations to include in the query.</param>
        /// <param name="operationObject5">The fifth operations to include in the query.</param>
        /// <param name="operationObject6">The sixth operations to include in the query.</param>
        /// <param name="operationObject7">The seventh operations to include in the query.</param>
        /// <param name="operationObject8">The eighth operations to include in the query.</param>
        void AddQueryToContext(IQueryContext context, TOperations1 operationObject1,
            TOperations2 operationObject2, TOperations3 operationObject3, TOperations4 operationObject4,
            TOperations5 operationObject5, TOperations6 operationObject6, TOperations7 operationObject7,
            TOperations8 operationObject8);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject(TOperations1 operationObject);

        /// <summary>
        /// Gets a default builder context to use while building.
        /// </summary>
        /// <returns>A default builder context to use while building.</returns>
        TBuilderContext InitializeContext();

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
        ParsedQueryOperations GetOperationsFromObject1(TOperations2 operationObject);

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
        ParsedQueryOperations GetOperationsFromObject2(TOperations3 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations3(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject3(TOperations4 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations4(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject5(TOperations5 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations5(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject6(TOperations6 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations6(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject7(TOperations7 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations7(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        ParsedQueryOperations GetOperationsFromObject8(TOperations8 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        IEnumerable<string> GetFormattedOperations8(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);
    }
}
