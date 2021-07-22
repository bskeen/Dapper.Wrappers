// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Used to store the current queries to be run, along with a results
    /// handler. It also tracks variables and their values to be used
    /// when the queries are executed, and allows for query execution.
    /// Results are populated into the provided IQueryResultsHandlers.
    /// </summary>
    public interface IQueryContext : IDisposable
    {
        /// <summary>
        /// Adds a given query to the context, along with its result handler.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
        void AddQuery(string query);

        /// <summary>
        /// Adds a variable, along with its value, to the query execution context.
        /// </summary>
        /// <param name="name">The name of the variable to be added.</param>
        /// <param name="value">The value of the variable to be added.</param>
        /// <param name="type">The type of the variable to be added.</param>
        /// <param name="isUnique">
        /// Whether or not the variable should be made unique within the context.
        /// </param>
        /// <returns>The name of the added variable(possibly with a unique suffix).</returns>
        string AddVariable(string name, object value, DbType? type = null, bool isUnique = true);

        /// <summary>
        /// Executes the next query against the database, returning the results.
        /// </summary>
        Task<IEnumerable<T>> ExecuteNextQuery<T>();

        Task ExecuteCommands();
    }
}
