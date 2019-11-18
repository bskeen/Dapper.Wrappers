using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers
{
    public class QueryOperationMetadata
    {
        public string Name { get; set; }

        public string BaseQueryString { get; set; }

        public IEnumerable<QueryParameterMetadata> Parameters { get; set; }
    }
}
