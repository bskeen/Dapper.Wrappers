using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Builders
{
    public class QueryBuilder<T>
    {


        public void AddQueryToContext(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> queryOperations)
        {

        }

        public void AddQueryToContext(IQueryContext context, T operationObject)
        {

        }
    }
}
