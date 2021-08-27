// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Adds methods to the Get query generator that can create the operations
    /// for the AddGetQuery method from the given types.
    /// </summary>
    /// <typeparam name="TFilter">The type from which to generate the filter operations.</typeparam>
    /// <typeparam name="TOrder">The type from which to generate the order operations.</typeparam>
    public interface IGenericGetQueryGenerator<in TFilter, in TOrder> : IGetQueryGenerator
    {
        /// <summary>
        /// Creates filter query operations to be passed to the AddGetQuery method.
        /// </summary>
        /// <param name="source">The object to be converted into filter query operations.</param>
        /// <returns>The list of filter QueryOperations.</returns>
        IEnumerable<QueryOperation> GetFilterOperationsFromSource(TFilter source);

        /// <summary>
        /// Creates order query operations to be passed to the AddGetQuery method.
        /// </summary>
        /// <param name="source">The object to be converted into order query operations.</param>
        /// <returns>The list of order QueryOperations.</returns>
        IEnumerable<QueryOperation> GetOrderOperationsFromSource(TOrder source);

        /// <summary>
        /// Creates a pagination object given a skip and a take, optionally with upper thresholds.
        /// If neither skip nor take is specified, then null is returned. If only one of them is
        /// specified, then a default value is used for the other.
        /// </summary>
        /// <param name="skip">How many results should be skipped before returning results.</param>
        /// <param name="take">The maximum number of results to be returned.</param>
        /// <param name="skipUpperThreshold">The upper threshold for how many results can be skipped. Defaults to no threshold.</param>
        /// <param name="takeUpperThreshold">The upper threshold for how many results can be returned. Defaults to 100.</param>
        /// <returns>A pagination object to be passed to AddGetQuery.</returns>
        Pagination GetPagination(int? skip, int? take, int? skipUpperThreshold = null, int? takeUpperThreshold = 100);
    }
}
