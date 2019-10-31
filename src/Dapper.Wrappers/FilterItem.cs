// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers
{
    /// <summary>
    /// Represents a filter specification for a query.
    /// </summary>
    public class FilterItem : QueryKeyValue
    {
        /// <summary>
        /// The operation to be used with the filter specification.
        /// </summary>
        public FilterOperations Operation { get; set; }
    }
}
