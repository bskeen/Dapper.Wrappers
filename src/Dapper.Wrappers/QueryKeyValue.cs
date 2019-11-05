// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;

namespace Dapper.Wrappers
{
    public class QueryKeyValue : QueryKey
    {
        /// <summary>
        /// The value associated with the key.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The type of the value.
        /// </summary>
        public DbType ValueType { get; set; }
    }
}
