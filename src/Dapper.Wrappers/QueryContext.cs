// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Used to store the current queries to be run, along with a results
    /// handler. It also tracks variables and their values to be used
    /// when the queries are executed, and allows for query execution.
    /// Results are populated into the provided IQueryResultsHandlers.
    /// </summary>
    public class QueryContext : IQueryContext
    {
        private IDbConnection _connection;
        private IList<string> _currentQuery;
        private bool _disposed = false;
        private int _currentVariableCounter;
        private DynamicParameters _parameters;
        private IDbTransaction _currentTransaction;

        public QueryContext(IDbConnection connection)
        {
            _connection = connection;
            Reset();
        }

        /// <summary>
        /// Adds a given query to the context, along with its result handler.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
        /// A class used to retrieve results from the GridReader resulting from
        /// the executed query.
        /// </param>
        public void AddQuery(string query)
        {
            _currentQuery.Add(query);
        }

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
        public string AddVariable(string name, object value, DbType type, bool isUnique = true)
        {
            var variableName = isUnique ? GetUniqueVariableName(name) : name;

            _parameters.Add(variableName, value, type, ParameterDirection.Input);

            return variableName;
        }

        private string GetUniqueVariableName(string name)
        {
            return $"{name}{GetNextCounterValue()}";
        }

        private int GetNextCounterValue()
        {
            return _currentVariableCounter++;
        }

        /// <summary>
        /// Executes the queries against the database, sending the results to the
        /// registered query handlers.
        /// </summary>
        public async Task<IEnumerable<T>> ExecuteQuery<T>()
        {
            var query = string.Join(" ", _currentQuery);

            // This solution is a slightly different version of similar code from Dapper .Net,
            // Retrieved on 1/23/2020 from https://github.com/StackExchange/Dapper/blob/master/Dapper/SqlMapper.Async.cs
            if (_connection is DbConnection dbConn)
            {
                await dbConn.OpenAsync(CancellationToken.None);
            }
            else
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                var resultsReader = await _connection.QueryMultipleAsync(query, _parameters);

                foreach (var handler in _resultsHandlers)
                {
                    await handler.ReadResults(resultsReader);
                }

                transaction.Commit();
            }

            Reset();
        }

        private async Task<IDbTransaction> GetTransaction()
        {
            if (!(_currentTransaction is null))
            {
                return _currentTransaction;
            }

            // This solution is a slightly different version of similar code from Dapper .Net,
            // Retrieved on 1/23/2020 from https://github.com/StackExchange/Dapper/blob/master/Dapper/SqlMapper.Async.cs
            if (_connection is DbConnection dbConn)
            {
                await dbConn.OpenAsync(CancellationToken.None);
            }
            else
            {
                _connection.Open();
            }

            _currentTransaction = _connection.BeginTransaction();

            return _currentTransaction;
        }

        /// <summary>
        /// Resets the context to its initial state.
        /// </summary>
        public void Reset()
        {
            _currentQuery = new List<string>();
            _currentTransaction?.Dispose();
            _currentTransaction = null;
            _parameters = new DynamicParameters();
            _currentVariableCounter = 1;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _connection?.Dispose();
                _connection = null;

                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
