using System.Data;
using Dapper.Wrappers.DependencyInjection;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dapper.Wrappers.Tests
{
    public class QueryContextTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _dbFixture;

        public QueryContextTests(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        private QueryContext GetDefaultQueryContext(SupportedDatabases dbType) => dbType switch
        {
            SupportedDatabases.SqlServer => new QueryContext(_dbFixture.SqlConnection),
            SupportedDatabases.PostgreSQL => new QueryContext(_dbFixture.PostgresConnection),
            _ => null
        };

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void Constructor_WithExistingParameters_ShouldCreateContext(SupportedDatabases dbType)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = GetDefaultQueryContext(dbType);

            // Assert
            result.Should().NotBeNull();
        }
    }
}
