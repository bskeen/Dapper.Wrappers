// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers.DependencyInjection
{
    /// <summary>
    /// Options for selecting the database to be used by the query builder.
    /// </summary>
    public enum SupportedDatabases
    {
        /// <summary>
        /// Represents MS SQL Server.
        /// </summary>
        SqlServer,

        /// <summary>
        /// Represents PostgreSQL.
        /// </summary>
        PostgreSQL
    }
}
