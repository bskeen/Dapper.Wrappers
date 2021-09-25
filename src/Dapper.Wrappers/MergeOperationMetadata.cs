// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers
{
    /// <summary>
    /// Contains metadata for a merge operation (either an insert or update).
    /// </summary>
    public class MergeOperationMetadata : QueryOperationMetadata
    {
        /// <summary>
        /// The column referenced by the insert or update operation.
        /// </summary>
        public string ReferencedColumn { get; set; }

        public bool IsRequired { get; set; }
    }
}
