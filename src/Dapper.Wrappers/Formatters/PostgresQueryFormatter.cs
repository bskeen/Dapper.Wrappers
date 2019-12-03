// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers.Formatters
{
    /// <summary>
    /// Provides methods to format a PostgreSQL query given the operations.
    /// </summary>
    public class PostgresQueryFormatter : BaseSqlQueryFormatter
    {
        /// <summary>
        /// Formats an identifier (such as a column name) for use in a query.
        /// </summary>
        /// <param name="identifierName">The identifier to format.</param>
        /// <returns>The formatted identifier.</returns>
        public override string FormatIdentifier(string identifierName)
        {
            return $"\"{identifierName}\"";
        }

        /// <summary>
        /// Formats the variables containing the skip and take parameters into a pagination statement.
        /// </summary>
        /// <param name="skipVariable">The name of the skip variable.</param>
        /// <param name="takeVariable">The name of the take variable.</param>
        /// <returns>A correctly formatted pagination statement.</returns>
        public override string FormatPagination(string skipVariable, string takeVariable)
        {
            return $"LIMIT {takeVariable} OFFSET {skipVariable}";
        }

        /// <summary>
        /// Formats an identifier as a variable to be used by the query.
        /// </summary>
        /// <param name="variableName">The identifier to be formatted as a variable.</param>
        /// <returns>A correctly formatted variable.</returns>
        public override string FormatVariable(string variableName)
        {
            return $"@{variableName}";
        }
    }
}
