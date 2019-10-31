// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers
{
    /// <summary>
    /// Used to represent an order specification for a query.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// The name of the key to order by.
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// The direction the results should be ordered.
        /// </summary>
        public OrderDirections Direction { get; set; }
    }
}
