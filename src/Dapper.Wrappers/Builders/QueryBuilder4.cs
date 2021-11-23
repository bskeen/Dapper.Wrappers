﻿// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Builders
{
    /// <summary>
    /// Defines a query builder type that includes methods to use operation types to build the query.
    /// </summary>
    /// <typeparam name="TOperations1">The first type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations2">The second type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations3">The third type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations4">The fourth type used to get the query operations.</typeparam>
    public abstract class QueryBuilder<TOperations1, TOperations2, TOperations3, TOperations4> : QueryBuilder,
        IQueryBuilder<TOperations1, TOperations2, TOperations3, TOperations4>
    {
        /// <summary>
        /// Adds a query to the given QueryContext, using the given query operations.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="queryOperations1">The first operations to include in the query.</param>
        /// <param name="queryOperations2">The second operations to include in the query.</param>
        /// <param name="queryOperations3">The third operations to include in the query.</param>
        /// <param name="queryOperations4">The fourth operations to include in the query.</param>
        public void AddQueryToContext(IQueryContext context, ParsedQueryOperations queryOperations1,
            ParsedQueryOperations queryOperations2,
            ParsedQueryOperations queryOperations3,
            ParsedQueryOperations queryOperations4)
        {
            var formattedOperations = new List<object>();
            formattedOperations.AddRange(GetFormattedOperations1(context, queryOperations1));
            formattedOperations.AddRange(GetFormattedOperations2(context, queryOperations2));
            formattedOperations.AddRange(GetFormattedOperations3(context, queryOperations3));
            formattedOperations.AddRange(GetFormattedOperations4(context, queryOperations4));

            var formattedQuery = string.Format(QueryFormat, formattedOperations.ToArray());

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
        public void AddQueryToContext(IQueryContext context, TOperations1 operationObject1,
            TOperations2 operationObject2, TOperations3 operationObject3, TOperations4 operationObject4)
        {
            var operations1 = GetOperationsFromObject1(operationObject1);
            var operations2 = GetOperationsFromObject2(operationObject2);
            var operations3 = GetOperationsFromObject3(operationObject3);
            var operations4 = GetOperationsFromObject4(operationObject4);

            AddQueryToContext(context, operations1, operations2, operations3, operations4);
        }

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
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract IEnumerable<string> GetFormattedOperations1(IQueryContext context, ParsedQueryOperations operations);

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
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract IEnumerable<string> GetFormattedOperations2(IQueryContext context, ParsedQueryOperations operations);

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
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract IEnumerable<string> GetFormattedOperations3(IQueryContext context, ParsedQueryOperations operations);

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
        /// <returns>The formatted operations to be included in the finished query</returns>
        public abstract IEnumerable<string> GetFormattedOperations4(IQueryContext context, ParsedQueryOperations operations);
    }
}
