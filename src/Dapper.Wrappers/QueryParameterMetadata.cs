// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Contains metadata about parameters for query operations.
    /// </summary>
    public class QueryParameterMetadata
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The DbType of the parameter's value.
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// Whether or not the parameter has a default value, stored in the DefaultValue property.
        /// </summary>
        public bool HasDefault { get; set; }

        /// <summary>
        /// The default value of the parameter, used if it is not specified in the requested query.
        /// </summary>
        public object DefaultValue { get; set; }
    }
}
