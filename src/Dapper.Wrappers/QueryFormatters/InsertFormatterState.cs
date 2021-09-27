using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.QueryFormatters
{
    public class InsertFormatterState
    {
        public List<string> ColumnOrder { get; set; }
        public IDictionary<string, MergeOperationMetadata> RequiredMetadata { get; set; }
        public HashSet<string> AlreadyReferencedColumns { get; set; }

        public bool IsFirstList { get; set; }
    }
}
