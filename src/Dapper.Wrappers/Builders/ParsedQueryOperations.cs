using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Builders
{
    public class ParsedQueryOperations
    {
        public virtual IEnumerable<QueryOperation> QueryOperations { get; set; }
    }
}
