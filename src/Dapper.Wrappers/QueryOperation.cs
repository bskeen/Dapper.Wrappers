using System;
using System.Collections.Generic;

namespace Dapper.Wrappers
{
    public class QueryOperation
    {
        public string Name { get; set; }
        public IEnumerable<QueryParameter> Parameters { get; set; }
    }
}
