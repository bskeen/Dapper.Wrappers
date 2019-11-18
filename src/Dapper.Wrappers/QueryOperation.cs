using System;
using System.Collections.Generic;

namespace Dapper.Wrappers
{
    public class QueryOperation
    {
        public string Name { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
    }
}
