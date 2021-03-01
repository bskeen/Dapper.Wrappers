// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers
{
    /// <summary>
    /// Used as an argument to the various query building methods to customize the operations included in the query.
    /// </summary>
    public class QueryOperation
    {
        /// <summary>
        /// The name of the operation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A map of parameter names to their values.
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }
    }
}
