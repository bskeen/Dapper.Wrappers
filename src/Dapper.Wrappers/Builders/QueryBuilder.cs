// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers.Builders
{
    /// <summary>
    /// The base query builder type.
    /// </summary>
    public abstract class QueryBuilder : IQueryBuilder
    {
        /// <summary>
        /// The string format to be used to build the query.
        /// </summary>
        public abstract string QueryFormat { get; }
    }
}
