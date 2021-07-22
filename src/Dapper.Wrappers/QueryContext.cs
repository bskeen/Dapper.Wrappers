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
        private readonly IDbConnection _connection;
        private IList<string> _currentQuery;
        private bool _disposed;
        private int _currentVariableCounter;
        private DynamicParameters _parameters;
        private IDbTransaction _currentTransaction;
        private SqlMapper.GridReader _currentGridReader;

        public QueryContext(IDbConnection connection)
        {
            _connection = connection;
            ResetQuery();
        }

        /// <summary>
        /// Adds a given query to the context, along with its result handler.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
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
        public string AddVariable(string name, object value, DbType? type = null, bool isUnique = true)
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
        /// Executes the queries against the database.
        /// </summary>
        public async Task<IEnumerable<T>> ExecuteNextQuery<T>()
        {
            if (_currentGridReader is null)
            {
                if (_currentQuery.Count == 0)
                {
                    throw new InvalidOperationException("The context contains no queries to execute against the database.");
                }

                await InitTransaction();
                var query = string.Join("\n", _currentQuery);
                _currentGridReader = await _connection.QueryMultipleAsync(query, _parameters, _currentTransaction);
                ResetQuery();
            }

            var results = await _currentGridReader.ReadAsync<T>();

            if (!_currentGridReader.IsConsumed)
            {
                return results;
            }

            CommitTransaction();

            return results;
        }

        public async Task ExecuteCommands()
        {
            var nullGridReader = _currentGridReader is null;
            if (!nullGridReader)
            {
                while (!_currentGridReader.IsConsumed)
                {
                    var _ = await _currentGridReader.ReadAsync();
                }

                CommitTransaction();
            }

            if (_currentQuery.Count > 0)
            {
                await InitTransaction();

                var commands = string.Join("\n", _currentQuery);
                await _connection.ExecuteAsync(commands, _parameters, _currentTransaction);
                ResetQuery();
                CommitTransaction();
            }
            else if (nullGridReader)
            {
                throw new InvalidOperationException(
                    "The context contains no commands to execute against the database.");
            }
        }

        private async Task InitTransaction()
        {
            if (!(_currentTransaction is null))
            {
                return;
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
        }

        private void CommitTransaction()
        {
            _currentTransaction.Commit();
            _currentTransaction.Dispose();
            _currentTransaction = null;

            _connection.Close();
            _currentGridReader = null;
        }

        /// <summary>
        /// Resets the context to its initial state.
        /// </summary>
        private void ResetQuery()
        {
            _currentQuery = new List<string>();
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
