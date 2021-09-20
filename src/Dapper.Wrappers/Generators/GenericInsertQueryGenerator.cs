// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Adds a method to the Insert query generator that can create the operations
    /// for the AddInsertQuery method from a given type.
    /// </summary>
    /// <typeparam name="TSource">The type from which to generate the insert operations.</typeparam>
    public abstract class GenericInsertQueryGenerator<TSource> : InsertQueryGenerator, IGenericInsertQueryGenerator<TSource>
    {
        protected GenericInsertQueryGenerator(IQueryOperationFormatter queryFormatter) : base(queryFormatter)
        {
        }

        /// <summary>
        /// Creates insert query operations to be passed to the AddInsertQuery method.
        /// </summary>
        /// <param name="source">The object to be converted into insert query operations.</param>
        /// <returns>The list of insert QueryOperations.</returns>
        public abstract IEnumerable<QueryOperation> GetInsertQueryOperationsFromSource(TSource source);
    }
}
