using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers
{
    public class MergeOperationMetadata : QueryOperationMetadata
    {
        public string ReferencedColumn { get; set; }
    }
}
