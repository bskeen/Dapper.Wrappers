using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Formatters;
using Dapper.Wrappers.Generators;
using Dapper.Wrappers.Tests.DbModels;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Npgsql;
using Xunit;

namespace Dapper.Wrappers.Tests.Generators
{
    public class GetQueryGeneratorTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IDbConnection _sqlConnection;
        private readonly IDbConnection _postgresConnection;

        public GetQueryGeneratorTests(DatabaseFixture databaseFixture, IEnumerable<IDbConnection> connections)
        {
            _databaseFixture = databaseFixture;
            var dbConnections = connections.ToList();
            _sqlConnection = dbConnections.FirstOrDefault(c => c is SqlConnection);
            _postgresConnection = dbConnections.FirstOrDefault(c => c is NpgsqlConnection);
        }

        private TestGetQueryGenerator GetTestInstance(SupportedDatabases dbType, string queryString,
            string defaultOrdering, IDictionary<string, QueryOperationMetadata> filterMetadata,
            IDictionary<string, QueryOperationMetadata> orderMetadata)
        {
            var formatter = _databaseFixture.GetFormatter(dbType);

            return new TestGetQueryGenerator(formatter, queryString, defaultOrdering, filterMetadata, orderMetadata);
        }

        private IDbConnection GetConnection(SupportedDatabases dbType) =>
            dbType == SupportedDatabases.SqlServer ? _sqlConnection : _postgresConnection;

        private IQueryContext GetQueryContext(SupportedDatabases dbType)
        {
            var connection = GetConnection(dbType);

            return new QueryContext(connection);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task AddGetQuery_WithoutFiltersOrderingOrPagination_ShouldReturnAllResults(
            SupportedDatabases dbType)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new QueryOperation[] { };

            var filterOperations = new[]
            {
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddGetQuery(context, filterOperations, orderOperations);

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            results.Should().HaveCount(books.Count);
            results.OrderBy(b => b.BookID).Should().BeEquivalentTo(books.OrderBy(b => b.BookID));
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "BookID", OrderDirections.Asc)]
        [InlineData(SupportedDatabases.SqlServer, "Name", OrderDirections.Asc)]
        [InlineData(SupportedDatabases.SqlServer, "AuthorID", OrderDirections.Asc)]
        [InlineData(SupportedDatabases.SqlServer, "PageCount", OrderDirections.Asc)]
        [InlineData(SupportedDatabases.SqlServer, "BookID", OrderDirections.Desc)]
        [InlineData(SupportedDatabases.SqlServer, "Name", OrderDirections.Desc)]
        [InlineData(SupportedDatabases.SqlServer, "AuthorID", OrderDirections.Desc)]
        [InlineData(SupportedDatabases.SqlServer, "PageCount", OrderDirections.Desc)]
        [InlineData(SupportedDatabases.PostgreSQL, "BookID", OrderDirections.Asc)]
        [InlineData(SupportedDatabases.PostgreSQL, "Name", OrderDirections.Asc)]
        [InlineData(SupportedDatabases.PostgreSQL, "AuthorID", OrderDirections.Asc)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCount", OrderDirections.Asc)]
        [InlineData(SupportedDatabases.PostgreSQL, "BookID", OrderDirections.Desc)]
        [InlineData(SupportedDatabases.PostgreSQL, "Name", OrderDirections.Desc)]
        [InlineData(SupportedDatabases.PostgreSQL, "AuthorID", OrderDirections.Desc)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCount", OrderDirections.Desc)]
        public async Task AddGetQuery_WithOrderingSpecified_CorrectlySortsTheOutput(SupportedDatabases dbType,
            string operationName, OrderDirections direction)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new []
            {
                new QueryOperation
                {
                    Name = operationName,
                    Parameters = new Dictionary<string, object>
                    {
                        {DapperWrappersConstants.OrderByDirectionParameter, direction.ToString()}
                    }
                }
            };

            var filterOperations = new[]
            {
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddGetQuery(context, filterOperations, orderOperations);

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            results.Should().HaveCount(books.Count);

            List<Book> orderedBooks;
            switch (operationName)
            {
                case "BookID":
                    orderedBooks = direction == OrderDirections.Asc
                        ? books.OrderBy(b => b.BookID.ToString()).ToList()
                        : books.OrderByDescending(b => b.BookID.ToString()).ToList();
                    break;
                case "Name":
                    orderedBooks = direction == OrderDirections.Asc
                        ? books.OrderBy(b => b.Name).ToList()
                        : books.OrderByDescending(b => b.Name).ToList();
                    break;
                case "AuthorID":
                    orderedBooks = direction == OrderDirections.Asc
                        ? books.OrderBy(b => b.AuthorID).ToList()
                        : books.OrderByDescending(b => b.AuthorID).ToList();
                    break;
                case "PageCount":
                    orderedBooks = direction == OrderDirections.Asc
                        ? books.OrderBy(b => b.PageCount).ToList()
                        : books.OrderByDescending(b => b.PageCount).ToList();
                    break;
                default:
                    orderedBooks = books;
                    break;
            }

            for (var i = 0; i < books.Count; i++)
            {
                results[i].Should().BeEquivalentTo(orderedBooks[i]);
            }
        }
    }

    public class TestGetQueryGenerator : GetQueryGenerator
    {
        public TestGetQueryGenerator(IQueryFormatter queryFormatter, string queryString, string defaultOrdering,
            IDictionary<string, QueryOperationMetadata> filterMetadata,
            IDictionary<string, QueryOperationMetadata> orderMetadata) : base(queryFormatter)
        {
            GetQueryString = queryString;
            DefaultOrdering = defaultOrdering;
            FilterOperationMetadata = filterMetadata;
            OrderOperationMetadata = orderMetadata;
        }

        protected override IDictionary<string, QueryOperationMetadata> FilterOperationMetadata { get; }
        protected override string GetQueryString { get; }
        protected override string DefaultOrdering { get; }
        protected override IDictionary<string, QueryOperationMetadata> OrderOperationMetadata { get; }
    }
}
