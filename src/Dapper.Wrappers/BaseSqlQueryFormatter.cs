using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.Wrappers
{
    public abstract class BaseSqlQueryFormatter : IQueryFormatter
    {
        public string FormatFilterOperation(string filterOperationString, IEnumerable<string> variableNames, OrderDirections? orderDirection)
        {
            return DefaultFormatOperation(filterOperationString, variableNames);
        }

        public string FormatFilterOperations(IEnumerable<string> filterOperations)
        {
            return $"WHERE {string.Join(" AND ", filterOperations)}";
        }

        public string FormatOrderOperation(string orderOperationString, IEnumerable<string> variableNames, OrderDirections? orderDirection)
        {
            var orderDirectionParameter = new []
                {!orderDirection.HasValue || orderDirection.Value == OrderDirections.Asc ? "ASC" : "DESC"};
            var variableParameters = variableNames.Select(FormatVariable);
            var formatParameters = orderDirectionParameter.Concat(variableParameters).Cast<object>().ToArray();
            return string.Format(orderOperationString, formatParameters);
        }

        public string FormatOrderOperations(IEnumerable<string> orderOperations)
        {
            return $"ORDER BY {string.Join(", ", orderOperations)}";
        }

        public string FormatUpdateOperation(string updateOperationString, IEnumerable<string> variableNames, OrderDirections? orderDirection)
        {
            return DefaultFormatOperation(updateOperationString, variableNames);
        }

        public string FormatUpdateOperations(IEnumerable<string> updateOperations)
        {
            return $"SET {string.Join(", ", updateOperations)}";
        }

        public string FormatInsertOperation(string insertOperation, IEnumerable<string> variableNames, OrderDirections? orderDirection)
        {
            return DefaultFormatOperation(insertOperation, variableNames);
        }

        public string FormatInsertOperations(IEnumerable<string> insertOperations)
        {
            return string.Join(", ", insertOperations);
        }

        public string FormatInsertColumn(string insertColumn)
        {
            return FormatIdentifier(insertColumn);
        }

        public string FormatInsertColumns(IEnumerable<string> insertColumns)
        {
            return string.Join(", ", insertColumns);
        }

        public string FormatGetQuery(string baseQueryString, string filterOperations, string orderOperations, bool isPaginated,
            string skipVariableName, string takeVariableName)
        {
            var formatParameters = new object[]
            {
                filterOperations,
                orderOperations,
                isPaginated
                    ? FormatPagination(FormatVariable(skipVariableName), FormatVariable(takeVariableName))
                    : string.Empty
            };

            return string.Format(baseQueryString, formatParameters);
        }

        public string FormatDeleteQuery(string baseQueryString, string deleteCriteria)
        {
            return string.Format(baseQueryString, deleteCriteria);
        }

        public string FormatUpdateQuery(string baseQueryString, string entityUpdates, string updateCriteria)
        {
            return string.Format(baseQueryString, entityUpdates, updateCriteria);
        }

        public string FormatInsertQuery(string baseQueryString, string columnList, string insertOperations)
        {
            return string.Format(baseQueryString, columnList, insertOperations);
        }

        public abstract string FormatVariable(string variableName);
        public abstract string FormatIdentifier(string identifierName);
        public abstract string FormatPagination(string skipVariable, string takeVariable);

        protected virtual string DefaultFormatOperation(string operationString, IEnumerable<string> variableNames)
        {
            var formatParameters = variableNames.Select(FormatVariable).Cast<object>().ToArray();
            return string.Format(operationString, formatParameters);
        }
    }
}
