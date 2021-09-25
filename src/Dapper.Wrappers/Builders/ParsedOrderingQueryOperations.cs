using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Builders
{
    public class ParsedOrderingQueryOperations : ParsedQueryOperations
    {
        public Pagination Pagination { get; set; }
    }
}
