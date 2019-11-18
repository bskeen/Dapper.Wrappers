using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dapper.Wrappers
{
    public class QueryParameterMetadata
    {
        public string Name { get; set; }
        public DbType DbType { get; set; }
        public bool HasDefault { get; set; }
        public object DefaultValue { get; set; }
    }
}
