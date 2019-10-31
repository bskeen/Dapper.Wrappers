// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Wrappers
{
    public class QueryContext : IQueryContext
    {
        private readonly IDbConnection _connection;
        private IList<string> _currentQuery;
        private IList<IQueryResultsHandler> _resultsHandlers;
        private DynamicParameters _parameters;
        private int _currentVariableCounter;

        public QueryContext(IDbConnection connection)
        {
            _connection = connection;
            ResetContext();
        }

        public void AddQuery(string query, IQueryResultsHandler resultsHandler)
        {
            _currentQuery.Add(query);
            _resultsHandlers.Add(resultsHandler);
        }

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

        public async Task ExecuteQueries()
        {
            var query = string.Join(" ", _currentQuery);

            using (var transaction = _connection.BeginTransaction())
            {
                var resultsReader = await _connection.QueryMultipleAsync(query, _parameters);

                foreach (var handler in _resultsHandlers)
                {
                    handler.ReadResults(resultsReader);
                }

                transaction.Commit();
            }

            ResetContext();
        }

        public void ResetContext()
        {
            _currentQuery = new List<string>();
            _resultsHandlers = new List<IQueryResultsHandler>();
            _parameters = new DynamicParameters();
            _currentVariableCounter = 1;
        }
    }
}
