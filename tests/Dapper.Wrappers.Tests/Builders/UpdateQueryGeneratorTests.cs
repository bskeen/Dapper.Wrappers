using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Generators;
using Dapper.Wrappers.OperationFormatters;
using Dapper.Wrappers.Tests.DbModels;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Npgsql;
using Xunit;

namespace Dapper.Wrappers.Tests.Generators
{
    public class UpdateQueryGeneratorTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IDbConnection _sqlConnection;
        private readonly IDbConnection _postgresConnection;
        private readonly IMetadataGenerator _metadataGenerator;

        public UpdateQueryGeneratorTests(DatabaseFixture databaseFixture, IEnumerable<IDbConnection> connections,
            IMetadataGenerator metadataGenerator)
        {
            _databaseFixture = databaseFixture;
            var dbConnections = connections.ToList();
            _sqlConnection = dbConnections.FirstOrDefault(c => c is SqlConnection);
            _postgresConnection = dbConnections.FirstOrDefault(c => c is NpgsqlConnection);
            _metadataGenerator = metadataGenerator;
        }

        private TestUpdateQueryGenerator GetTestInstance(SupportedDatabases dbType, string updateQueryString,
            IDictionary<string, MergeOperationMetadata> updateMetadata,
            IDictionary<string, QueryOperationMetadata> filterMetadata)
        {
            var formatter = _databaseFixture.GetFormatter(dbType);

            return new TestUpdateQueryGenerator(formatter, updateQueryString, updateMetadata, filterMetadata);
        }

        private IDbConnection GetConnection(SupportedDatabases dbType) =>
            dbType == SupportedDatabases.SqlServer ? _sqlConnection : _postgresConnection;

        private IQueryContext GetQueryContext(SupportedDatabases dbType)
        {
            var connection = GetConnection(dbType);

            return new QueryContext(connection);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson")]
        [InlineData(SupportedDatabases.SqlServer, "Arcanum Unbounded", 672, "Brandon Sanderson")]
        [InlineData(SupportedDatabases.SqlServer, "The Sign of the Four", 122, "Arthur Conan Doyle")]
        [InlineData(SupportedDatabases.SqlServer, "The Memoirs of Sherlock Holmes", 208, "Arthur Conan Doyle")]
        [InlineData(SupportedDatabases.SqlServer, "One Fish Two Fish Red Fish Blue Fish", 62, "Dr. Seuss")]
        [InlineData(SupportedDatabases.SqlServer, "Green Eggs and Ham", 62, "Dr. Seuss")]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson")]
        [InlineData(SupportedDatabases.PostgreSQL, "Arcanum Unbounded", 672, "Brandon Sanderson")]
        [InlineData(SupportedDatabases.PostgreSQL, "The Sign of the Four", 122, "Arthur Conan Doyle")]
        [InlineData(SupportedDatabases.PostgreSQL, "The Memoirs of Sherlock Holmes", 208, "Arthur Conan Doyle")]
        [InlineData(SupportedDatabases.PostgreSQL, "One Fish Two Fish Red Fish Blue Fish", 62, "Dr. Seuss")]
        [InlineData(SupportedDatabases.PostgreSQL, "Green Eggs and Ham", 62, "Dr. Seuss")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson")]
        public async Task AddUpdateQuery_WithoutFilters_UpdatesAllRows(SupportedDatabases dbType, string nameValue,
            int? pageCountValue, string authorName)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            var authorIdValue = authors.First(a => $"{a.FirstName} {a.LastName}" == authorName).AuthorID;

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name", ("Name", nameValue)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authorIdValue)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCountValue))
            };

            var filterOperations = new []
            {
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            generator.AddUpdateQuery(context, operations, filterOperations);

            await context.ExecuteCommands();

            // Assert
            var updatedBooks = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            updatedBooks.Should().NotContain(book =>
                book.Name != nameValue || book.AuthorID != authorIdValue || book.PageCount != pageCountValue);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, true)]
        [InlineData(SupportedDatabases.SqlServer, false)]
        [InlineData(SupportedDatabases.PostgreSQL, true)]
        [InlineData(SupportedDatabases.PostgreSQL, false)]
        public void AddUpdateQuery_WithNoUpdateOperations_ShouldThrowException(SupportedDatabases dbType, bool isEmpty)
        {
            // Arrange
            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var operations = isEmpty ? new QueryOperation[] { } : null;

            Action act = () => generator.AddUpdateQuery(context, operations, operations);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("No update operations specified.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void AddUpdateQuery_WithOnlyBogusOperations_ShouldThrowException(SupportedDatabases dbType)
        {
            // Arrange
            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var operations = new []
            {
                _metadataGenerator.GetQueryOperation("Bogus1", ("BogusValue1", 1), ("BogusValue2", "Really Bogus!")),
                _metadataGenerator.GetQueryOperation("Bogus2", ("BogusValue3", Guid.NewGuid()), ("BogusValue4", null))
            };

            Action act = () => generator.AddUpdateQuery(context, operations, operations);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("No update operations specified.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void AddUpdateQuery_WithDuplicateOperations_ShouldThrowException(SupportedDatabases dbType)
        {
            // Arrange
            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name", ("Name", "Test Name 1")),
                _metadataGenerator.GetQueryOperation("Name", ("Name", "Test Name 2"))
            };

            Action act = () => generator.AddUpdateQuery(context, operations, operations);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Cannot have multiple updates of the same column.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "BookIDEquals")]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "BookIDNotEquals")]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "BookIDEquals")]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "BookIDNotEquals")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "BookIDEquals")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "BookIDNotEquals")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "BookIDEquals")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "BookIDNotEquals")]
        public async Task AddUpdateQuery_WithBookIDFilters_UpdatesExpectedRows(SupportedDatabases dbType, string nameValue,
            int? pageCountValue, string authorName, string filterOperationName)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            var authorIdValue = authors.First(a => $"{a.FirstName} {a.LastName}" == authorName).AuthorID;

            var randomGenerator = new Random();
            var bookIndex = randomGenerator.Next(books.Count);

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name", ("Name", nameValue)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authorIdValue)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCountValue))
            };

            var filterOperations = new []
            {
                _metadataGenerator.GetQueryOperation(filterOperationName, ("BookID", books[bookIndex].BookID)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            generator.AddUpdateQuery(context, operations, filterOperations);

            await context.ExecuteCommands();

            // Assert
            var updatedBooks = (await _databaseFixture.GetBooks(connection, dbType, testId)).Where(b =>
                b.Name == nameValue && b.AuthorID == authorIdValue && b.PageCount == pageCountValue).ToList();

            if (filterOperationName == "BookIDEquals")
            {
                updatedBooks.Should().HaveCount(1);
            }
            else
            {
                updatedBooks.Should().HaveCount(books.Count - 1);
            }
        }
        
        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 0, 1)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 0, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 0, 10)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 3, 7)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 5, 8)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 9, 10)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 0, 1)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 0, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 0, 10)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 3, 7)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 5, 8)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 9, 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 0, 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 0, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 0, 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 3, 7)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 5, 8)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 9, 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 0, 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 0, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 0, 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 3, 7)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 5, 8)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 9, 10)]
        public async Task AddUpdateQuery_WithBookIDInFilters_UpdatesExpectedRows(SupportedDatabases dbType, string nameValue,
            int? pageCountValue, string authorName, int startIndex, int endIndex)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            var authorIdValue = authors.First(a => $"{a.FirstName} {a.LastName}" == authorName).AuthorID;

            var idsToUpdate = books.OrderBy(b => b.BookID).Skip(startIndex).Take(endIndex - startIndex)
                .Select(b => b.BookID).ToList();

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name", ("Name", nameValue)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authorIdValue)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCountValue))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("BookIDIn", ("BookIDs", idsToUpdate)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            generator.AddUpdateQuery(context, operations, filterOperations);

            await context.ExecuteCommands();

            // Assert
            var updatedBooks = (await _databaseFixture.GetBooks(connection, dbType, testId)).Where(b =>
                b.Name == nameValue && b.AuthorID == authorIdValue && b.PageCount == pageCountValue).ToList();

            updatedBooks.Should().HaveCount(endIndex - startIndex);
            updatedBooks.Should().NotContain(b => !idsToUpdate.Contains(b.BookID));
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameEquals", "The Hound of the Baskervilles", 1)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameEquals", "BogusName", 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameEquals", "", 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameEquals", null, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameNotEquals", "The Hound of the Baskervilles", 9)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameNotEquals", "BogusName", 10)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameNotEquals", "", 10)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameNotEquals", null, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameLike", "s%a%", 2)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameLike", "The%", 5)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameLike", "", 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameLike", null, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameEquals", "The Hound of the Baskervilles", 1)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameEquals", "BogusName", 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameEquals", "", 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameEquals", null, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameNotEquals", "The Hound of the Baskervilles", 9)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameNotEquals", "BogusName", 10)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameNotEquals", "", 10)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameNotEquals", null, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameLike", "s%a%", 2)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameLike", "The%", 5)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameLike", "", 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameLike", null, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameEquals", "The Hound of the Baskervilles", 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameEquals", "BogusName", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameEquals", "", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameEquals", null, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameNotEquals", "The Hound of the Baskervilles", 9)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameNotEquals", "BogusName", 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameNotEquals", "", 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameNotEquals", null, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameLike", "s%a%", 2)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameLike", "The%", 5)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameLike", "", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "NameLike", null, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameEquals", "The Hound of the Baskervilles", 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameEquals", "BogusName", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameEquals", "", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameEquals", null, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameNotEquals", "The Hound of the Baskervilles", 9)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameNotEquals", "BogusName", 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameNotEquals", "", 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameNotEquals", null, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameLike", "s%a%", 2)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameLike", "The%", 5)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameLike", "", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "NameLike", null, 0)]
        public async Task AddUpdateQuery_WithBookNameFilters_UpdatesExpectedRows(SupportedDatabases dbType,
            string nameValue, int? pageCountValue, string authorName, string filterOperationName,
            string filterOperationValue, int updatedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            var authorIdValue = authors.First(a => $"{a.FirstName} {a.LastName}" == authorName).AuthorID;

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name",("Name", nameValue)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authorIdValue)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCountValue))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(filterOperationName, ("BookName", filterOperationValue)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            generator.AddUpdateQuery(context, operations, filterOperations);

            await context.ExecuteCommands();

            // Assert
            var updatedBooks = (await _databaseFixture.GetBooks(connection, dbType, testId)).Where(b =>
                b.Name == nameValue && b.AuthorID == authorIdValue && b.PageCount == pageCountValue).ToList();

            updatedBooks.Should().HaveCount(updatedCount);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountEquals", 513, 1)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountEquals", 42, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountNotEquals", 513, 8)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountNotEquals", 42, 9)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountGreater", 307, 3)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountLess", 307, 5)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountEquals", 513, 1)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountEquals", 42, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountNotEquals", 513, 8)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountNotEquals", 42, 9)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountGreater", 307, 3)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountLess", 307, 5)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountEquals", 513, 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountEquals", 42, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountNotEquals", 513, 8)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountNotEquals", 42, 9)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountGreater", 307, 3)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountLess", 307, 5)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountEquals", 513, 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountEquals", 42, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountNotEquals", 513, 8)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountNotEquals", 42, 9)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountGreater", 307, 3)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountLess", 307, 5)]
        public async Task AddUpdateQuery_WithPageCountFilters_UpdatesExpectedRows(SupportedDatabases dbType,
            string nameValue, int? pageCountValue, string authorName, string filterOperationName,
            int filterOperationValue, int updatedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            var authorIdValue = authors.First(a => $"{a.FirstName} {a.LastName}" == authorName).AuthorID;

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name", ("Name", nameValue)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authorIdValue)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCountValue))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(filterOperationName, ("PageCount", filterOperationValue)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            generator.AddUpdateQuery(context, operations, filterOperations);

            await context.ExecuteCommands();

            // Assert
            var updatedBooks = (await _databaseFixture.GetBooks(connection, dbType, testId)).Where(b =>
                b.Name == nameValue && b.AuthorID == authorIdValue && b.PageCount == pageCountValue).ToList();

            updatedBooks.Should().HaveCount(updatedCount);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 0, 0, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 1, 2000, 9)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 1, 100, 2)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 200, 500, 4)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 0, 0, 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 1, 2000, 9)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 1, 100, 2)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", 200, 500, 4)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 0, 0, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 1, 2000, 9)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 1, 100, 2)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", 200, 500, 4)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 0, 0, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 1, 2000, 9)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 1, 100, 2)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", 200, 500, 4)]
        public async Task AddUpdateQuery_WithPageCountBetweenFilters_UpdatesExpectedRows(SupportedDatabases dbType,
            string nameValue, int? pageCountValue, string authorName, int start, int end, int updatedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            var authorIdValue = authors.First(a => $"{a.FirstName} {a.LastName}" == authorName).AuthorID;

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name", ("Name", nameValue)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authorIdValue)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCountValue))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("PageCountBetween", ("PageCountStart", start),
                    ("PageCountEnd", end)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            generator.AddUpdateQuery(context, operations, filterOperations);

            await context.ExecuteCommands();

            // Assert
            var updatedBooks = (await _databaseFixture.GetBooks(connection, dbType, testId)).Where(b =>
                b.Name == nameValue && b.AuthorID == authorIdValue && b.PageCount == pageCountValue).ToList();

            updatedBooks.Should().HaveCount(updatedCount);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountNull")]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountNotNull")]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountNull")]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountNotNull")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountNull")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "PageCountNotNull")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountNull")]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "PageCountNotNull")]
        public async Task AddUpdateQuery_WithPageCountNullFilters_UpdatesExpectedRows(SupportedDatabases dbType,
            string nameValue, int? pageCountValue, string authorName, string filterOperationName)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            var authorIdValue = authors.First(a => $"{a.FirstName} {a.LastName}" == authorName).AuthorID;

            var nullPageCountId = books.Where(b => !b.PageCount.HasValue).Select(b => b.BookID).First();

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name", ("Name", nameValue)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authorIdValue)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCountValue))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(filterOperationName),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            generator.AddUpdateQuery(context, operations, filterOperations);

            await context.ExecuteCommands();

            // Assert
            var updatedBooks = (await _databaseFixture.GetBooks(connection, dbType, testId)).Where(b =>
                b.Name == nameValue && b.AuthorID == authorIdValue && b.PageCount == pageCountValue).ToList();

            if (filterOperationName == "PageCountNull")
            {
                updatedBooks.Should().HaveCount(1);
                updatedBooks[0].BookID.Should().Be(nullPageCountId);
            }
            else
            {
                updatedBooks.Should().HaveCount(9);
                updatedBooks.Should().NotContain(b => b.BookID == nullPageCountId);
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "Romance", 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "Fantasy Fiction", 3)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "Novel", 4)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "Romance", 0)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "Fantasy Fiction", 3)]
        [InlineData(SupportedDatabases.SqlServer, "Mistborn: The Final Empire", null, "Brandon Sanderson", "Novel", 4)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "Romance", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "Fantasy Fiction", 3)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", 647, "Brandon Sanderson", "Novel", 4)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "Romance", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "Fantasy Fiction", 3)]
        [InlineData(SupportedDatabases.PostgreSQL, "Mistborn: The Final Empire", null, "Brandon Sanderson", "Novel", 4)]
        public async Task AddUpdateQuery_WithHasGenreFilter_UpdatesExpectedRows(SupportedDatabases dbType,
            string nameValue, int? pageCountValue, string authorName, string genreName, int updatedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var authors = testData.Authors.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.UpdateQuery
                : SqlQueryFormatConstants.Postgres.Books.UpdateQuery;

            var operationMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookUpdateMetadata
                : GeneratorTestConstants.Postgres.DefaultBookUpdateMetadata;

            var filterMetadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, operationMetadata, filterMetadata);
            var context = GetQueryContext(dbType);

            var authorIdValue = authors.First(a => $"{a.FirstName} {a.LastName}" == authorName).AuthorID;

            var nullPageCountId = books.Where(b => !b.PageCount.HasValue).Select(b => b.BookID).First();

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Name", ("Name", nameValue)),
                _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authorIdValue)),
                _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCountValue))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("HasGenre", ("GenreName", genreName)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            generator.AddUpdateQuery(context, operations, filterOperations);

            await context.ExecuteCommands();

            // Assert
            var updatedBooks = (await _databaseFixture.GetBooks(connection, dbType, testId)).Where(b =>
                b.Name == nameValue && b.AuthorID == authorIdValue && b.PageCount == pageCountValue).ToList();

            updatedBooks.Should().HaveCount(updatedCount);
        }

        public void Dispose()
        {
            _sqlConnection?.Dispose();
            _postgresConnection?.Dispose();
        }
    }

    public class TestUpdateQueryGenerator : UpdateQueryGenerator
    {
        public TestUpdateQueryGenerator(IQueryOperationFormatter queryFormatter, string updateQueryString,
            IDictionary<string, MergeOperationMetadata> updateMetadata,
            IDictionary<string, QueryOperationMetadata> filterMetadata) : base(queryFormatter)
        {
            UpdateQueryString = updateQueryString;
            UpdateOperationMetadata = updateMetadata;
            FilterOperationMetadata = filterMetadata;
        }

        protected override IDictionary<string, QueryOperationMetadata> FilterOperationMetadata { get; }
        protected override string UpdateQueryString { get; }
        protected override IDictionary<string, MergeOperationMetadata> UpdateOperationMetadata { get; }
    }
}
