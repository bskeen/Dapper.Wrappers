// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Represents a filter specification for a query.
    /// </summary>
    public class VariableFilter : IFilterItem
    {
        /// <summary>
        /// The name of the key associated with the filter.
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// The value associated with the key.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The type of the value.
        /// </summary>
        public DbType ValueType { get; set; }

        /// <summary>
        /// The operation to be used with the filter specification.
        /// </summary>
        public FilterOperations Operation { get; set; }
    }
}
