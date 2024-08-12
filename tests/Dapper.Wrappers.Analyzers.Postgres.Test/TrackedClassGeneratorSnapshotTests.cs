using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Dapper.Wrappers.Analyzers.Postgres.Test
{
    [UsesVerify]
    public class TrackedClassGeneratorSnapshotTests
    {
        [Fact]
        public Task GeneratesTrackedClassCorrectly()
        {
            var source = @"
using Dapper.Wrappers.Generation;

namespace Dapper.Wrappers.Analyzers.Postgres.Test
{
    [TrackUpdates]
    public class TestClass
    {
        public virtual bool TestBool { get; set; }

        public virtual int TestInt { get; set; }
    }
}";

            return TestHelper.Verify(source);
        }
    }
}
