// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Formatters
{
    /// <summary>
    /// Provides methods to format a query given the operations.
    /// </summary>
    public interface IQueryFormatter
    {
        /// <summary>
        /// Formats a delete query given the different pieces.
        /// </summary>
        /// <param name="baseQueryString">
        /// The base query string to use for the delete query.
        /// </param>
        /// <param name="deleteCriteria">The criteria to add to the delete query.</param>
        /// <returns>The formatted delete query.</returns>
        string FormatDeleteQuery(string baseQueryString, string deleteCriteria);

        /// <summary>
        /// Formats one filter operation with the given variables.
        /// </summary>
        /// <param name="filterOperationString">Base filter operation string.</param>
        /// <param name="variableNames">Variable names to add to the filter operation string.</param>
        /// <returns>The formatted filter operation.</returns>
        string FormatFilterOperation(string filterOperationString, IEnumerable<string> variableNames);

        /// <summary>
        /// Combines multiple formatted filter operations into a complete filter statement.
        /// </summary>
        /// <param name="filterOperations">The formatted filter operations to combine.</param>
        /// <returns>The complete filter statement.</returns>
        string FormatFilterOperations(IEnumerable<string> filterOperations);

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
        string FormatGetQuery(string baseQueryString, string filterOperations, string orderOperations, bool isPaginated,
            string skipVariableName, string takeVariableName);

        /// <summary>
        /// Formats a column name for use in an insert query's column list.
        /// </summary>
        /// <param name="insertColumn">The name of the column to format.</param>
        /// <returns>The formatted column name.</returns>
        string FormatInsertColumn(string insertColumn);

        /// <summary>
        /// Combines multiple formatted column names nto an insert query's column list.
        /// </summary>
        /// <param name="insertColumns">Formatted column names to combine into the column list.</param>
        /// <returns>The formatted column list.</returns>
        string FormatInsertColumns(IEnumerable<string> insertColumns);

        /// <summary>
        /// Formats an insert operation with the given variable names.
        /// </summary>
        /// <param name="insertOperation">The operation to format.</param>
        /// <param name="variableNames">The variables to place in the operation.</param>
        /// <returns>The formatted insert operation.</returns>
        string FormatInsertOperation(string insertOperation, IEnumerable<string> variableNames);

        /// <summary>
        /// Combines multiple formatted insert operations into an insert query's values list.
        /// </summary>
        /// <param name="insertOperations">Formatted insert operations to combine into an insert query's value list.</param>
        /// <returns>The formatted value list.</returns>
        string FormatInsertOperations(IEnumerable<string> insertOperations);

        /// <summary>
        /// Combines multiple formatted values lists to allow multiple inserts to be performed.
        /// </summary>
        /// <param name="valuesLists">Each item contains a distinct values list to be included in the insert.</param>
        /// <returns>The formatted list of values lists.</returns>
        string FormatMultipleInsertValuesLists(IEnumerable<string> valuesLists);

        /// <summary>
        /// Formats an insert query given the different pieces.
        /// </summary>
        /// <param name="baseQueryString">The base query string to use for the insert query.</param>
        /// <param name="columnList">The formatted column list to include in the query.</param>
        /// <param name="insertOperations">The formatted value list to include in the query.</param>
        /// <returns>The formatted insert query.</returns>
        string FormatInsertQuery(string baseQueryString, string columnList, string insertOperations);

        /// <summary>
        /// Formats an order operation with the given variable names.
        /// </summary>
        /// <param name="orderOperationString">The operation to format.</param>
        /// <param name="variableNames">The variables to place in the operation.</param>
        /// <param name="orderDirection">The order direction for the operation.</param>
        /// <returns>The formatted order operation.</returns>
        string FormatOrderOperation(string orderOperationString, IEnumerable<string> variableNames, OrderDirections? orderDirection);

        /// <summary>
        /// Combines multiple formatted order operations into a get query's order by statement.
        /// </summary>
        /// <param name="orderOperations">Formatted order operations to combine into a get query's order by statement.</param>
        /// <returns>The formatted order by statement.</returns>
        string FormatOrderOperations(IEnumerable<string> orderOperations);

        /// <summary>
        /// Formats an update operation with the given variable names.
        /// </summary>
        /// <param name="updateOperationString">The operation to format.</param>
        /// <param name="variableNames">The variables to place in the operation.</param>
        /// <returns>The formatted update operation.</returns>
        string FormatUpdateOperation(string updateOperationString, IEnumerable<string> variableNames);

        /// <summary>
        /// Combines multiple formatted update operations into an update query's set statement.
        /// </summary>
        /// <param name="updateOperations">Formatted update operations to combine into an update query's set statement.</param>
        /// <returns>The formatted set statement.</returns>
        string FormatUpdateOperations(IEnumerable<string> updateOperations);

        /// <summary>
        /// Formats an update query given the different pieces.
        /// </summary>
        /// <param name="baseQueryString">The base query string to use for the insert query.</param>
        /// <param name="updateOperations">The formatted set statement for the query.</param>
        /// <param name="updateCriteria">The formatted criteria to add to the query.</param>
        /// <returns>The formatted update query.</returns>
        string FormatUpdateQuery(string baseQueryString, string updateOperations, string updateCriteria);
    }
}
