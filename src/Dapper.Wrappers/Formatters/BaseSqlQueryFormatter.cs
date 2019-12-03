// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Dapper.Wrappers.Formatters
{
    /// <summary>
    /// Provides methods to format a SQL query given the operations.
    /// </summary>
    public abstract class BaseSqlQueryFormatter : IQueryFormatter
    {
        /// <summary>
        /// Formats a delete query given the different pieces.
        /// </summary>
        /// <param name="baseQueryString">
        /// The base query string to use for the delete query.
        /// </param>
        /// <param name="deleteCriteria">The criteria to add to the delete query.</param>
        /// <returns>The formatted delete query.</returns>
        public string FormatDeleteQuery(string baseQueryString, string deleteCriteria)
        {
            return string.Format(baseQueryString, deleteCriteria);
        }

        /// <summary>
        /// Formats one filter operation with the given variables.
        /// </summary>
        /// <param name="filterOperationString">Base filter operation string.</param>
        /// <param name="variableNames">Variable names to add to the filter operation string.</param>
        /// <returns>The formatted filter operation.</returns>
        public string FormatFilterOperation(string filterOperationString, IEnumerable<string> variableNames)
        {
            return DefaultFormatOperation(filterOperationString, variableNames);
        }

        /// <summary>
        /// Combines multiple formatted filter operations into a complete filter statement.
        /// </summary>
        /// <param name="filterOperations">The formatted filter operations to combine.</param>
        /// <returns>The complete filter statement.</returns>
        public string FormatFilterOperations(IEnumerable<string> filterOperations)
        {
            return $"WHERE {string.Join(" AND ", filterOperations)}";
        }

        /// <summary>
        /// Formats a get query given the different pieces.
        /// </summary>
        /// <param name="baseQueryString">The base query string to use for the get query.</param>
        /// <param name="filterOperations">The criteria to add to the get query.</param>
        /// <param name="orderOperations">The order operations to apply to the get query.</param>
        /// <param name="isPaginated">If the query is paginated or not.</param>
        /// <param name="skipVariableName">The variable name where the pagination skip value is stored.</param>
        /// <param name="takeVariableName">The variable name where the pagination take value is stored.</param>
        /// <returns>The formatted get query.</returns>
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

        /// <summary>
        /// Formats a column name for use in an insert query's column list.
        /// </summary>
        /// <param name="insertColumn">The name of the column to format.</param>
        /// <returns>The formatted column name.</returns>
        public string FormatInsertColumn(string insertColumn)
        {
            return FormatIdentifier(insertColumn);
        }

        /// <summary>
        /// Combines multiple formatted column names nto an insert query's column list.
        /// </summary>
        /// <param name="insertColumns">Formatted column names to combine into the column list.</param>
        /// <returns>The formatted column list.</returns>
        public string FormatInsertColumns(IEnumerable<string> insertColumns)
        {
            return string.Join(", ", insertColumns);
        }

        /// <summary>
        /// Formats an insert operation with the given variable names.
        /// </summary>
        /// <param name="insertOperation">The operation to format.</param>
        /// <param name="variableNames">The variables to place in the operation.</param>
        /// <returns>The formatted insert operation.</returns>
        public string FormatInsertOperation(string insertOperation, IEnumerable<string> variableNames)
        {
            return DefaultFormatOperation(insertOperation, variableNames);
        }

        /// <summary>
        /// Combines multiple formatted insert operations into an insert query's values list.
        /// </summary>
        /// <param name="insertOperations">Formatted insert operations to combine into an insert query's value list.</param>
        /// <returns>The formatted value list.</returns>
        public string FormatInsertOperations(IEnumerable<string> insertOperations)
        {
            return string.Join(", ", insertOperations);
        }

        /// <summary>
        /// Formats an insert query given the different pieces.
        /// </summary>
        /// <param name="baseQueryString">The base query string to use for the insert query.</param>
        /// <param name="columnList">The formatted column list to include in the query.</param>
        /// <param name="insertOperations">The formatted value list to include in the query.</param>
        /// <returns>The formatted insert query.</returns>
        public string FormatInsertQuery(string baseQueryString, string columnList, string insertOperations)
        {
            return string.Format(baseQueryString, columnList, insertOperations);
        }

        /// <summary>
        /// Formats an order operation with the given variable names.
        /// </summary>
        /// <param name="orderOperationString">The operation to format.</param>
        /// <param name="variableNames">The variables to place in the operation.</param>
        /// <param name="orderDirection">The order direction for the operation.</param>
        /// <returns>The formatted order operation.</returns>
        public string FormatOrderOperation(string orderOperationString, IEnumerable<string> variableNames, OrderDirections? orderDirection)
        {
            var orderDirectionParameter = new []
                {!orderDirection.HasValue || orderDirection.Value == OrderDirections.Asc ? "ASC" : "DESC"};
            var variableParameters = variableNames.Select(FormatVariable);
            var formatParameters = orderDirectionParameter.Concat(variableParameters).Cast<object>().ToArray();
            return string.Format(orderOperationString, formatParameters);
        }

        /// <summary>
        /// Combines multiple formatted order operations into a get query's order by statement.
        /// </summary>
        /// <param name="orderOperations">Formatted order operations to combine into a get query's order by statement.</param>
        /// <returns>The formatted order by statement.</returns>
        public string FormatOrderOperations(IEnumerable<string> orderOperations)
        {
            return $"ORDER BY {string.Join(", ", orderOperations)}";
        }

        /// <summary>
        /// Formats an update operation with the given variable names.
        /// </summary>
        /// <param name="updateOperationString">The operation to format.</param>
        /// <param name="variableNames">The variables to place in the operation.</param>
        /// <returns>The formatted update operation.</returns>
        public string FormatUpdateOperation(string updateOperationString, IEnumerable<string> variableNames)
        {
            return DefaultFormatOperation(updateOperationString, variableNames);
        }

        /// <summary>
        /// Combines multiple formatted update operations into an update query's set statement.
        /// </summary>
        /// <param name="updateOperations">Formatted update operations to combine into an update query's set statement.</param>
        /// <returns>The formatted set statement.</returns>
        public string FormatUpdateOperations(IEnumerable<string> updateOperations)
        {
            return $"SET {string.Join(", ", updateOperations)}";
        }

        /// <summary>
        /// Formats an update query given the different pieces.
        /// </summary>
        /// <param name="baseQueryString">The base query string to use for the insert query.</param>
        /// <param name="updateOperations">The formatted set statement for the query.</param>
        /// <param name="updateCriteria">The formatted criteria to add to the query.</param>
        /// <returns>The formatted update query.</returns>
        public string FormatUpdateQuery(string baseQueryString, string updateOperations, string updateCriteria)
        {
            return string.Format(baseQueryString, updateOperations, updateCriteria);
        }

        /// <summary>
        /// Formats an identifier (such as a column name) for use in a query.
        /// </summary>
        /// <param name="identifierName">The identifier to format.</param>
        /// <returns>The formatted identifier.</returns>
        public abstract string FormatIdentifier(string identifierName);

        /// <summary>
        /// Formats the variables containing the skip and take parameters into a pagination statement.
        /// </summary>
        /// <param name="skipVariable">The name of the skip variable.</param>
        /// <param name="takeVariable">The name of the take variable.</param>
        /// <returns>A correctly formatted pagination statement.</returns>
        public abstract string FormatPagination(string skipVariable, string takeVariable);

        /// <summary>
        /// Formats an identifier as a variable to be used by the query.
        /// </summary>
        /// <param name="variableName">The identifier to be formatted as a variable.</param>
        /// <returns>A correctly formatted variable.</returns>
        public abstract string FormatVariable(string variableName);

        /// <summary>
        /// Converts the variable names to an array, which is then passed as a parameter to the format function.
        /// </summary>
        /// <param name="operationString">The operation to format.</param>
        /// <param name="variableNames">The variable names to include in the operation.</param>
        /// <returns>The formatted operation.</returns>
        protected virtual string DefaultFormatOperation(string operationString, IEnumerable<string> variableNames)
        {
            var formatParameters = variableNames.Select(FormatVariable).Cast<object>().ToArray();
            return string.Format(operationString, formatParameters);
        }
    }
}
