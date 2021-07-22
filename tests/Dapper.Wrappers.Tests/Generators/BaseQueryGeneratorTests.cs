// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

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
