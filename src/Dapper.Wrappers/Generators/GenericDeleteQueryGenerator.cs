// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Dapper.Wrappers.Formatters;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Adds a method to the Delete query generator that can create the operations
    /// for the AddDeleteQuery method from a given type.
    /// </summary>
    /// <typeparam name="TSource">The type from which to generate the query operations.</typeparam>
    public abstract class GenericDeleteQueryGenerator<TSource> : DeleteQueryGenerator, IGenericDeleteQueryGenerator<TSource>
    {
        protected GenericDeleteQueryGenerator(IQueryFormatter queryFormatter) : base(queryFormatter)
        {
        }

        /// <summary>
        /// Creates query operations to be passed to the AddDeleteQuery method.
        /// </summary>
        /// <param name="source">The object to be converted into query operations.</param>
        /// <returns>The list of QueryOperations.</returns>
        public abstract IEnumerable<QueryOperation> GetDeleteQueryOperations(TSource source);
    }
}
