// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Contains metadata for a query operation.
    /// </summary>
    public class QueryOperationMetadata
    {
        /// <summary>
        /// The name of the query operation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The base query string to be used for this operation.
        /// </summary>
        public string BaseQueryString { get; set; }

        /// <summary>
        /// The parameters required in the query string.
        /// </summary>
        public IEnumerable<QueryParameterMetadata> Parameters { get; set; }
    }
}
