// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run an insert query.
    /// </summary>
    public interface IInsertQueryGenerator
    {
        /// <summary>
        /// Adds an insert query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="insertOperations">
        /// The operations corresponding to the object to be inserted into the database.
        /// </param>
        void AddInsertQuery(IQueryContext context, IEnumerable<QueryOperation> insertOperations);

        /// <summary>
        /// Adds an insert query with multiple values lists to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="insertOperations">The operations corresponding to the objects to be inserted into the database.</param>
        void AddMultipleInsertQuery(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> insertOperations);
    }
}
