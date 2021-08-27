﻿// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Dapper.Wrappers.Formatters;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Adds methods to the Update query generator that can create the operations
    /// for the AddUpdateQuery method from the given types.
    /// </summary>
    /// <typeparam name="TFilter">The type from which to generate the filter operations.</typeparam>
    /// <typeparam name="TUpdate">The type from which to generate the update operations.</typeparam>
    public abstract class GenericUpdateQueryGenerator<TFilter, TUpdate> : UpdateQueryGenerator,
        IGenericUpdateQueryGenerator<TFilter, TUpdate>
    {
        protected GenericUpdateQueryGenerator(IQueryFormatter queryFormatter) : base(queryFormatter)
        {
        }

        /// <summary>
        /// Creates filter query operations to be passed to the AddUpdateQuery method.
        /// </summary>
        /// <param name="source">The object to be converted into filter query operations.</param>
        /// <returns>The list of filter QueryOperations.</returns>
        public abstract IEnumerable<QueryOperation> GetFilterOperationsFromSource(TFilter source);

        /// <summary>
        /// Creates update query operations to be passed to the AddUpdateQuery method.
        /// </summary>
        /// <param name="source">The object to be converted into update query operations.</param>
        /// <returns>The list of update QueryOperations.</returns>
        public abstract IEnumerable<QueryOperation> GetUpdateOperationsFromSource(TUpdate source);
    }
}
