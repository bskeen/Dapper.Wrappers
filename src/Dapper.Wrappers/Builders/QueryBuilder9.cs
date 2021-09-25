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
    /// <typeparam name="TOperations4">The fourth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations5">The fifth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations6">The sixth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations7">The seventh type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations8">The eighth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations9">The ninth type used to get the query operations.</typeparam>
    public abstract class QueryBuilder<TBuilderContext, TOperations1, TOperations2, TOperations3, TOperations4, TOperations5,
        TOperations6, TOperations7, TOperations8, TOperations9> : QueryBuilder,
        IQueryBuilder<TBuilderContext, TOperations1, TOperations2, TOperations3, TOperations4, TOperations5, TOperations6, TOperations7,
            TOperations8, TOperations9>
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
        /// <param name="queryOperations9">The ninth operations to include in the query.</param>
        public void AddQueryToContext(IQueryContext context, ParsedQueryOperations queryOperations1,
            ParsedQueryOperations queryOperations2,
            ParsedQueryOperations queryOperations3,
            ParsedQueryOperations queryOperations4,
            ParsedQueryOperations queryOperations5,
            ParsedQueryOperations queryOperations6,
            ParsedQueryOperations queryOperations7,
            ParsedQueryOperations queryOperations8,
            ParsedQueryOperations queryOperations9)
        {
            var builderContext = InitializeContext();
            var formattedOperations1 = GetFormattedOperations1(context, queryOperations1, builderContext);
            var formattedOperations2 = GetFormattedOperations2(context, queryOperations2, builderContext);
            var formattedOperations3 = GetFormattedOperations3(context, queryOperations3, builderContext);
            var formattedOperations4 = GetFormattedOperations4(context, queryOperations4, builderContext);
            var formattedOperations5 = GetFormattedOperations5(context, queryOperations5, builderContext);
            var formattedOperations6 = GetFormattedOperations6(context, queryOperations6, builderContext);
            var formattedOperations7 = GetFormattedOperations7(context, queryOperations7, builderContext);
            var formattedOperations8 = GetFormattedOperations8(context, queryOperations8, builderContext);
            var formattedOperations9 = GetFormattedOperations9(context, queryOperations9, builderContext);

            var formattedQuery = string.Format(QueryFormat, formattedOperations1, formattedOperations2,
                formattedOperations3, formattedOperations4, formattedOperations5, formattedOperations6,
                formattedOperations7, formattedOperations8, formattedOperations9);

            context.AddQuery(formattedQuery);
        }

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
        /// <param name="operationObject9">The ninth operations to include in the query.</param>
        public void AddQueryToContext(IQueryContext context, TOperations1 operationObject1,
            TOperations2 operationObject2, TOperations3 operationObject3, TOperations4 operationObject4,
            TOperations5 operationObject5, TOperations6 operationObject6, TOperations7 operationObject7,
            TOperations8 operationObject8, TOperations9 operationObject9)
        {
            var operations1 = GetOperationsFromObject1(operationObject1);
            var operations2 = GetOperationsFromObject2(operationObject2);
            var operations3 = GetOperationsFromObject3(operationObject3);
            var operations4 = GetOperationsFromObject4(operationObject4);
            var operations5 = GetOperationsFromObject5(operationObject5);
            var operations6 = GetOperationsFromObject6(operationObject6);
            var operations7 = GetOperationsFromObject7(operationObject7);
            var operations8 = GetOperationsFromObject8(operationObject8);
            var operations9 = GetOperationsFromObject9(operationObject9);

            AddQueryToContext(context, operations1, operations2, operations3, operations4, operations5, operations6,
                operations7, operations8, operations9);
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
        public abstract ParsedQueryOperations GetOperationsFromObject1(TOperations1 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations1(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject2(TOperations2 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations2(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject3(TOperations3 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations3(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject4(TOperations4 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations4(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject5(TOperations5 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations5(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject6(TOperations6 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations6(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject7(TOperations7 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations7(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject8(TOperations8 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations8(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        public abstract ParsedQueryOperations GetOperationsFromObject9(TOperations9 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <param name="builderContext">Any state that needs to be shared between calls to GetFormattedOperations.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract string GetFormattedOperations9(IQueryContext context, ParsedQueryOperations operations, TBuilderContext builderContext);
    }
}