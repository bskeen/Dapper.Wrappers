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
    /// <typeparam name="TOperations7">The seventh type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations8">The eighth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations9">The ninth type used to get the query operations.</typeparam>
    /// <typeparam name="TOperations10">The tenth type used to get the query operations.</typeparam>
    public interface IQueryBuilder<in TOperations1, in TOperations2, in TOperations3, in TOperations4, in TOperations5, in TOperations6,
        in TOperations7, in TOperations8, in TOperations9, in TOperations10> : IQueryBuilder
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
        /// <param name="queryOperations10">The tenth operations to include in the query.</param>
        void AddQueryToContext(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> queryOperations1,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations2,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations3,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations4,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations5,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations6,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations7,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations8,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations9,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations10);

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
        /// <param name="operationObject10">The tenth operations to include in the query.</param>
        void AddQueryToContext(IQueryContext context, TOperations1 operationObject1,
            TOperations2 operationObject2, TOperations3 operationObject3, TOperations4 operationObject4,
            TOperations5 operationObject5, TOperations6 operationObject6, TOperations7 operationObject7,
            TOperations8 operationObject8, TOperations9 operationObject9, TOperations10 operationObject10);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations1 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations1(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations2 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations2(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations3 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations3(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations4 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations4(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations5 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations5(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations6 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations6(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations7 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations7(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations8 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations8(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations9 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations9(IEnumerable<IEnumerable<QueryOperation>> operations);

        /// <summary>
        /// Given a query operations object, constructs the query operations to be used.
        /// </summary>
        /// <param name="operationObject">The object to use when constructing the operations.</param>
        /// <returns>The object converted into query operations.</returns>
        IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations10 operationObject);

        /// <summary>
        /// Formats the given operations into a string ready to be inserted into the finished query.
        /// </summary>
        /// <param name="operations">The operations to include in the formatted query piece.</param>
        /// <returns>The formatted operations to be included in the finished query</returns>
        string GetFormattedOperations10(IEnumerable<IEnumerable<QueryOperation>> operations);
    }
}
