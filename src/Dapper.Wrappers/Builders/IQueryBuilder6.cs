// © 2021 by Benjamin Skeen
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
    /// <typeparam name="TOperations5">The fifth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations6">The sixth type used to get the query operations.</typeparam>
    public interface IQueryBuilder<in TOperations1, in TOperations2, in TOperations3, in TOperations4, in TOperations5,
        in TOperations6> : IQueryBuilder
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
        void AddQueryToContext(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> queryOperations1,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations2,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations3,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations4,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations5,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations6);

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
        void AddQueryToContext(IQueryContext context, TOperations1 operationObject1,
            TOperations2 operationObject2, TOperations3 operationObject3, TOperations4 operationObject4,
            TOperations5 operationObject5, TOperations6 operationObject6);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations1 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations1(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations2 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations2(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations3 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations3(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations4 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations4(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations5 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations5(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations6 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="context">The query context to be updated.</param>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations6(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> operations);
    }
}
