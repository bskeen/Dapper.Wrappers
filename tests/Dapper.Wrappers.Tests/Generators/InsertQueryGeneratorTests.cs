// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class InsertQueryGeneratorTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IDbConnection _sqlConnection;
        private readonly IDbConnection _postgresConnection;
        private readonly IMetadataGenerator _metadataGenerator;

        public InsertQueryGeneratorTests(DatabaseFixture databaseFixture, IEnumerable<IDbConnection> connections,
            IMetadataGenerator metadataGenerator)
        {
            _databaseFixture = databaseFixture;
            var dbConnections = connections.ToList();
            _sqlConnection = dbConnections.FirstOrDefault(c => c is SqlConnection);
            _postgresConnection = dbConnections.FirstOrDefault(c => c is NpgsqlConnection);
            _metadataGenerator = metadataGenerator;
        }

        private TestInsertQueryGenerator GetTestInstance(SupportedDatabases dbType, string query,
            IDictionary<string, MergeOperationMetadata> metadata,
            IDictionary<string, MergeOperationMetadata> requiredMetadata,
            IDictionary<string, QueryOperation> defaultOperations)
        {
            var formatter = _databaseFixture.GetFormatter(dbType);

            return new TestInsertQueryGenerator(formatter, query, metadata, requiredMetadata, defaultOperations);
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
        public void AddInsertQuery_WithNullOperations_ThrowsException(SupportedDatabases dbType)
        {
            // Arrange
            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.InsertQuery
                : SqlQueryFormatConstants.Postgres.Books.InsertQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultBookInsertMetadata;

            var requiredMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultRequiredBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultRequiredBookInsertMetadata;

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var generator = GetTestInstance(dbType, query, metadata, requiredMetadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => generator.AddInsertQuery(context, null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Insert operations cannot be null.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void AddInsertQuery_WithEmptyOperations_ThrowsException(SupportedDatabases dbType)
        {
            // Arrange
            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.InsertQuery
                : SqlQueryFormatConstants.Postgres.Books.InsertQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultBookInsertMetadata;

            var requiredMetadata = new Dictionary<string, MergeOperationMetadata>();

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var generator = GetTestInstance(dbType, query, metadata, requiredMetadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => generator.AddInsertQuery(context, new QueryOperation[] { });

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("No insert operations specified.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void AddInsertQuery_WithBogusOperations_ThrowsException(SupportedDatabases dbType)
        {
            // Arrange
            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.InsertQuery
                : SqlQueryFormatConstants.Postgres.Books.InsertQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultBookInsertMetadata;

            var requiredMetadata = new Dictionary<string, MergeOperationMetadata>();

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var generator = GetTestInstance(dbType, query, metadata, requiredMetadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => generator.AddInsertQuery(context, new []
            {
                _metadataGenerator.GetQueryOperation("Bogus1"),
                _metadataGenerator.GetQueryOperation("Bogus2")
            });

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("No insert operations specified.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void AddInsertQuery_WithMissingRequiredOperationWithoutDefault_ThrowsException(SupportedDatabases dbType)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.InsertQuery
                : SqlQueryFormatConstants.Postgres.Books.InsertQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultBookInsertMetadata;

            var requiredMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultRequiredBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultRequiredBookInsertMetadata;

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var generator = GetTestInstance(dbType, query, metadata, requiredMetadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => generator.AddInsertQuery(context, new[]
            {
                _metadataGenerator.GetQueryOperation("BookID", ("BookID", Guid.NewGuid())),
                _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
            });

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Value must be supplied for 'AuthorID'.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void AddInsertQuery_WithDuplicateOperations_ShouldThrowException(SupportedDatabases dbType)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.InsertQuery
                : SqlQueryFormatConstants.Postgres.Books.InsertQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultBookInsertMetadata;

            var requiredMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultRequiredBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultRequiredBookInsertMetadata;

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var generator = GetTestInstance(dbType, query, metadata, requiredMetadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => generator.AddInsertQuery(context, new[]
            {
                _metadataGenerator.GetQueryOperation("BookID", ("BookID", Guid.NewGuid())),
                _metadataGenerator.GetQueryOperation("BookID", ("BookID", Guid.NewGuid())),
                _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
            });

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Cannot have multiple inserts into the same column.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "The Way of Kings", 1007)]
        [InlineData(SupportedDatabases.SqlServer, "The Sign of the Four", 122)]
        [InlineData(SupportedDatabases.SqlServer, "One Fish Two Fish Red Fish Blue Fish", null)]
        [InlineData(SupportedDatabases.PostgreSQL, "The Way of Kings", 1007)]
        [InlineData(SupportedDatabases.PostgreSQL, "The Sign of the Four", 122)]
        [InlineData(SupportedDatabases.PostgreSQL, "One Fish Two Fish Red Fish Blue Fish", null)]
        public async Task AddInsertQuery_WithAllColumnsSpecified_ShouldInsertRecordIntoDatabase(
            SupportedDatabases dbType, string name, int? pageCount)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.InsertQuery
                : SqlQueryFormatConstants.Postgres.Books.InsertQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultBookInsertMetadata;

            var requiredMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultRequiredBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultRequiredBookInsertMetadata;

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var generator = GetTestInstance(dbType, query, metadata, requiredMetadata, defaultOperations);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var authorIndex = randomGenerator.Next(authors.Count);
            var bookId = Guid.NewGuid();

            // Act
            generator.AddInsertQuery(context, new[]
            {
                _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookId)),
                _metadataGenerator.GetQueryOperation("Name", ("Name", name)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authors[authorIndex].AuthorID)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCount)),
                _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
            });

            await context.ExecuteCommands();

            // Assert
            var books = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            books.Should().HaveCount(1);
            books[0].BookID.Should().Be(bookId);
            books[0].AuthorID.Should().Be(authors[authorIndex].AuthorID);
            books[0].Name.Should().Be(name);
            books[0].PageCount.Should().Be(pageCount);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, 1007)]
        [InlineData(SupportedDatabases.SqlServer, 122)]
        [InlineData(SupportedDatabases.SqlServer, null)]
        [InlineData(SupportedDatabases.PostgreSQL, 1007)]
        [InlineData(SupportedDatabases.PostgreSQL, 122)]
        [InlineData(SupportedDatabases.PostgreSQL, null)]
        public async Task AddInsertQuery_WithMissingRequiredColumnWithDefault_ShouldInsertWithDefaultValue(
            SupportedDatabases dbType, int? pageCount)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.InsertQuery
                : SqlQueryFormatConstants.Postgres.Books.InsertQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultBookInsertMetadata;

            var requiredMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultRequiredBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultRequiredBookInsertMetadata;

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var generator = GetTestInstance(dbType, query, metadata, requiredMetadata, defaultOperations);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var authorIndex = randomGenerator.Next(authors.Count);
            var bookId = Guid.NewGuid();

            // Act
            generator.AddInsertQuery(context, new[]
            {
                _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookId)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authors[authorIndex].AuthorID)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCount)),
                _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
            });

            await context.ExecuteCommands();

            // Assert
            var books = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            books.Should().HaveCount(1);
            books[0].BookID.Should().Be(bookId);
            books[0].AuthorID.Should().Be(authors[authorIndex].AuthorID);
            books[0].Name.Should().Be("This is an Unnamed Book");
            books[0].PageCount.Should().Be(pageCount);
        }
    }

    public class TestInsertQueryGenerator : InsertQueryGenerator
    {
        private readonly IDictionary<string, MergeOperationMetadata> _requiredMetadata;

        public TestInsertQueryGenerator(IQueryFormatter queryFormatter, string queryString,
            IDictionary<string, MergeOperationMetadata> metadata,
            IDictionary<string, MergeOperationMetadata> requiredMetadata,
            IDictionary<string, QueryOperation> defaultOperations) : base(queryFormatter)
        {
            InsertQueryString = queryString;
            InsertOperationMetadata = metadata;
            _requiredMetadata = requiredMetadata;
            DefaultOperations = defaultOperations;
        }

        protected override string InsertQueryString { get; }
        protected override IDictionary<string, MergeOperationMetadata> InsertOperationMetadata { get; }

        protected override IDictionary<string, MergeOperationMetadata> GetRequiredInsertOperationMetadata() =>
            new Dictionary<string, MergeOperationMetadata>(_requiredMetadata);

        protected override IDictionary<string, QueryOperation> DefaultOperations { get; }
    }
}
