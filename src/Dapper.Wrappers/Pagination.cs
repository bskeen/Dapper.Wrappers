// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers
{
    /// <summary>
    /// Used to represent a pagination specification for a query.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Declares how many results to skip before returning results.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Declares how many results to include in the result set.
        /// </summary>
        public int Take { get; set; }
    }
}
