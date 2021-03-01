using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Wrappers.Formatters;
using Dapper.Wrappers.Generators;

namespace Dapper.Wrappers.Tests.Generators
{
    public class BaseQueryGeneratorTests
    {
        private static BaseQueryGenerator GetDefaultTestInstance(IQueryFormatter queryFormatter)
        {
            return new TestBaseQueryGenerator(queryFormatter);
        }
    }

    public class TestBaseQueryGenerator : BaseQueryGenerator
    {
        public TestBaseQueryGenerator(IQueryFormatter queryFormatter) : base(queryFormatter)
        {
        }
    }
}
