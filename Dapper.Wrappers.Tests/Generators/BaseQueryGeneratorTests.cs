using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Wrappers.Formatters;
using Dapper.Wrappers.Generators;

namespace Dapper.Wrappers.Tests.Generators
{
    public class BaseQueryGeneratorTests
    {
        private static QueryGenerator GetDefaultTestInstance(IQueryFormatter queryFormatter)
        {
            return new TestQueryGenerator(queryFormatter);
        }
    }

    public class TestQueryGenerator : QueryGenerator
    {
        public TestQueryGenerator(IQueryFormatter queryFormatter) : base(queryFormatter)
        {
        }
    }
}
