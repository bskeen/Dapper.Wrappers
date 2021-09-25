﻿// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Wrappers.Builders;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.QueryFormatters;
using Dapper.Wrappers.Tests.DbModels;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Npgsql;
using Xunit;

namespace Dapper.Wrappers.Tests.Builders
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

        private TestInsertQueryBuilder GetTestInstance(SupportedDatabases dbType, string query,
            IDictionary<string, MergeOperationMetadata> metadata,
            IDictionary<string, QueryOperation> defaultOperations)
        {
            var formatter = _databaseFixture.GetFormatter(dbType);
            var insertColumnsFormatter = new InsertColumnsFormatter(formatter);
            var valuesListFormatter = new ValuesListFormatter(formatter);

            return new TestInsertQueryBuilder(insertColumnsFormatter, valuesListFormatter, query, metadata,
                defaultOperations);
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

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations(), null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Insert operations cannot be empty.");
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

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = new QueryOperation[] { }
            }, null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("No values list operations specified.");
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

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = new[]
                {
                    _metadataGenerator.GetQueryOperation("Bogus1"),
                    _metadataGenerator.GetQueryOperation("Bogus2")
                }
            }, null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("No values list operations specified.");
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
                ? GeneratorTestConstants.SqlServer.DefaultRequiredBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultRequiredBookInsertMetadata;

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations
            {
                QueryOperations = new[]
                {
                    _metadataGenerator.GetQueryOperation("BookID", ("BookID", Guid.NewGuid())),
                    _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                    _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                }
            }, null);

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

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            // Act
            Action act = () => builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations
            {
                QueryOperations = new[]
                {
                    _metadataGenerator.GetQueryOperation("BookID", ("BookID", Guid.NewGuid())),
                    _metadataGenerator.GetQueryOperation("BookID", ("BookID", Guid.NewGuid())),
                    _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                    _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                }
            }, null);

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

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var authorIndex = randomGenerator.Next(authors.Count);
            var bookId = Guid.NewGuid();

            // Act
            builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations
            {
                QueryOperations = new[]
                {
                    _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookId)),
                    _metadataGenerator.GetQueryOperation("Name", ("Name", name)),
                    _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authors[authorIndex].AuthorID)),
                    _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCount)),
                    _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                    _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                }
            }, null);

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
                ? GeneratorTestConstants.SqlServer.DefaultRequiredBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultRequiredBookInsertMetadata;

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var authorIndex = randomGenerator.Next(authors.Count);
            var bookId = Guid.NewGuid();

            // Act
            builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations
            {
                QueryOperations = new[]
                {
                    _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookId)),
                    _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", authors[authorIndex].AuthorID)),
                    _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", pageCount)),
                    _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                    _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                }
            }, null);

            await context.ExecuteCommands();

            // Assert
            var books = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            books.Should().HaveCount(1);
            books[0].BookID.Should().Be(bookId);
            books[0].AuthorID.Should().Be(authors[authorIndex].AuthorID);
            books[0].Name.Should().Be("This is an Unnamed Book");
            books[0].PageCount.Should().Be(pageCount);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, new [] {"The Way of Kings", "Arcanum Unbounded"}, new [] {1007, 672})]
        [InlineData(SupportedDatabases.SqlServer, new [] {"The Sign of the Four", "The Return of Sherlock Holmes"}, new [] {122, 403})]
        [InlineData(SupportedDatabases.SqlServer, new [] {"One Fish Two Fish Red Fish Blue Fish", "Green Eggs and Ham"}, new [] {62, 62})]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "The Way of Kings", "Arcanum Unbounded" }, new[] { 1007, 672 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "The Sign of the Four", "The Return of Sherlock Holmes" }, new[] { 122, 403 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "One Fish Two Fish Red Fish Blue Fish", "Green Eggs and Ham" }, new[] { 62, 62 })]
        public async Task AddMultipleInsertQuery_WithMultipleInserts_ShouldInsertRecordsIntoDatabase(
            SupportedDatabases dbType, string[] names, int[] pageCounts)
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

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();

            var booksToCreate = new List<Book>();

            var queryOperations = new List<IEnumerable<QueryOperation>>();

            var nullIndex = randomGenerator.Next(names.Length);

            for (var i = 0; i < names.Length; i++)
            {
                var authorIndex = randomGenerator.Next(authors.Count);

                var bookToCreate = new Book
                {
                    AuthorID = authors[authorIndex].AuthorID,
                    Name = names[i],
                    PageCount = i != nullIndex ? (int?)pageCounts[i] : null
                };

                booksToCreate.Add(bookToCreate);

                queryOperations.Add(new []
                {
                    _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookToCreate.BookID)),
                    _metadataGenerator.GetQueryOperation("Name", ("Name", bookToCreate.Name)),
                    _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", bookToCreate.AuthorID)),
                    _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", bookToCreate.PageCount)),
                    _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                    _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                });
            }

            // Act
            builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations
            {
                MultiQueryOperations = queryOperations
            }, null);

            await context.ExecuteCommands();

            // Assert
            var books = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            books.Should().HaveCount(names.Length);

            foreach (var book in books)
            {
                var bookToCheckAgainst = booksToCreate.FirstOrDefault(b => b.BookID == book.BookID);
                bookToCheckAgainst.Should().NotBeNull();
                if (bookToCheckAgainst != null)
                {
                    book.BookID.Should().Be(bookToCheckAgainst.BookID);
                    book.AuthorID.Should().Be(bookToCheckAgainst.AuthorID);
                    book.Name.Should().Be(bookToCheckAgainst.Name);
                    book.PageCount.Should().Be(bookToCheckAgainst.PageCount);
                }
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, new[] { "The Way of Kings", "Arcanum Unbounded" }, new[] { 1007, 672 })]
        [InlineData(SupportedDatabases.SqlServer, new[] { "The Sign of the Four", "The Return of Sherlock Holmes" }, new[] { 122, 403 })]
        [InlineData(SupportedDatabases.SqlServer, new[] { "One Fish Two Fish Red Fish Blue Fish", "Green Eggs and Ham" }, new[] { 62, 62 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "The Way of Kings", "Arcanum Unbounded" }, new[] { 1007, 672 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "The Sign of the Four", "The Return of Sherlock Holmes" }, new[] { 122, 403 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "One Fish Two Fish Red Fish Blue Fish", "Green Eggs and Ham" }, new[] { 62, 62 })]
        public async Task AddMultipleInsertQuery_WithInsertOperationsInDifferentOrders_ShouldInsertRecordsIntoDatabase(
            SupportedDatabases dbType, string[] names, int[] pageCounts)
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

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();

            var booksToCreate = new List<Book>();

            var queryOperations = new List<IEnumerable<QueryOperation>>();

            var nullIndex = randomGenerator.Next(names.Length);

            for (var i = 0; i < names.Length; i++)
            {
                var authorIndex = randomGenerator.Next(authors.Count);

                var bookToCreate = new Book
                {
                    AuthorID = authors[authorIndex].AuthorID,
                    Name = names[i],
                    PageCount = i != nullIndex ? (int?)pageCounts[i] : null
                };

                booksToCreate.Add(bookToCreate);

                queryOperations.Add(new[]
                {
                    _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookToCreate.BookID)),
                    _metadataGenerator.GetQueryOperation("Name", ("Name", bookToCreate.Name)),
                    _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", bookToCreate.AuthorID)),
                    _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", bookToCreate.PageCount)),
                    _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                    _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                }.OrderBy(_ => new Guid()));
            }

            // Act
            builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations
            {
                MultiQueryOperations = queryOperations
            }, null);

            await context.ExecuteCommands();

            // Assert
            var books = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            books.Should().HaveCount(names.Length);

            foreach (var book in books)
            {
                var bookToCheckAgainst = booksToCreate.FirstOrDefault(b => b.BookID == book.BookID);
                bookToCheckAgainst.Should().NotBeNull();
                if (bookToCheckAgainst != null)
                {
                    book.BookID.Should().Be(bookToCheckAgainst.BookID);
                    book.AuthorID.Should().Be(bookToCheckAgainst.AuthorID);
                    book.Name.Should().Be(bookToCheckAgainst.Name);
                    book.PageCount.Should().Be(bookToCheckAgainst.PageCount);
                }
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, new[] { "The Way of Kings", "Arcanum Unbounded" }, new[] { 1007, 672 })]
        [InlineData(SupportedDatabases.SqlServer, new[] { "The Sign of the Four", "The Return of Sherlock Holmes" }, new[] { 122, 403 })]
        [InlineData(SupportedDatabases.SqlServer, new[] { "One Fish Two Fish Red Fish Blue Fish", "Green Eggs and Ham" }, new[] { 62, 62 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "The Way of Kings", "Arcanum Unbounded" }, new[] { 1007, 672 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "The Sign of the Four", "The Return of Sherlock Holmes" }, new[] { 122, 403 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "One Fish Two Fish Red Fish Blue Fish", "Green Eggs and Ham" }, new[] { 62, 62 })]
        public async Task AddMultipleInsertQuery_WithMissingOperations_ShouldInsertRecordsIntoDatabase(
            SupportedDatabases dbType, string[] names, int[] pageCounts)
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

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();

            var booksToCreate = new List<Book>();

            var queryOperations = new List<IEnumerable<QueryOperation>>();

            var nullIndex = randomGenerator.Next(names.Length);
            Guid nullBookId = Guid.NewGuid();

            for (var i = 0; i < names.Length; i++)
            {
                var authorIndex = randomGenerator.Next(authors.Count);

                var bookToCreate = new Book
                {
                    AuthorID = authors[authorIndex].AuthorID,
                    Name = names[i],
                    PageCount = i != nullIndex ? (int?)pageCounts[i] : null
                };

                booksToCreate.Add(bookToCreate);

                if (i == nullIndex)
                {
                    nullBookId = bookToCreate.BookID;
                    queryOperations.Add(new[]
                    {
                        _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookToCreate.BookID)),
                        _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", bookToCreate.AuthorID)),
                        _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                        _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                    }.OrderBy(_ => new Guid()));
                }
                else
                {
                    queryOperations.Add(new[]
                    {
                        _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookToCreate.BookID)),
                        _metadataGenerator.GetQueryOperation("Name", ("Name", bookToCreate.Name)),
                        _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", bookToCreate.AuthorID)),
                        _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", bookToCreate.PageCount)),
                        _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                        _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                    }.OrderBy(_ => new Guid()));
                }
            }

            // Act
            builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations
            {
                MultiQueryOperations = queryOperations
            }, null);

            await context.ExecuteCommands();

            // Assert
            var books = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            books.Should().HaveCount(names.Length);

            foreach (var book in books)
            {
                var bookToCheckAgainst = booksToCreate.FirstOrDefault(b => b.BookID == book.BookID);
                bookToCheckAgainst.Should().NotBeNull();
                if (bookToCheckAgainst != null)
                {
                    var nameToCheck = bookToCheckAgainst.BookID == nullBookId
                        ? "This is a Default Book"
                        : bookToCheckAgainst.Name;

                    book.BookID.Should().Be(bookToCheckAgainst.BookID);
                    book.AuthorID.Should().Be(bookToCheckAgainst.AuthorID);
                    book.Name.Should().Be(nameToCheck);
                    book.PageCount.Should().Be(bookToCheckAgainst.PageCount);
                }
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, new[] { "The Way of Kings", "Arcanum Unbounded" }, new[] { 1007, 672 })]
        [InlineData(SupportedDatabases.SqlServer, new[] { "The Sign of the Four", "The Return of Sherlock Holmes" }, new[] { 122, 403 })]
        [InlineData(SupportedDatabases.SqlServer, new[] { "One Fish Two Fish Red Fish Blue Fish", "Green Eggs and Ham" }, new[] { 62, 62 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "The Way of Kings", "Arcanum Unbounded" }, new[] { 1007, 672 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "The Sign of the Four", "The Return of Sherlock Holmes" }, new[] { 122, 403 })]
        [InlineData(SupportedDatabases.PostgreSQL, new[] { "One Fish Two Fish Red Fish Blue Fish", "Green Eggs and Ham" }, new[] { 62, 62 })]
        public async Task AddMultipleInsertQuery_WithMissingNonDefaultOperation_ShouldThrowException(
            SupportedDatabases dbType, string[] names, int[] pageCounts)
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
                ? GeneratorTestConstants.SqlServer.DefaultRequiredBookInsertMetadata
                : GeneratorTestConstants.Postgres.DefaultRequiredBookInsertMetadata;

            var defaultOperations = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookInsertOperations
                : GeneratorTestConstants.Postgres.DefaultBookInsertOperations;

            var builder = GetTestInstance(dbType, query, metadata, defaultOperations);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();

            var queryOperations = new List<IEnumerable<QueryOperation>>();

            var nullIndex = randomGenerator.Next(names.Length);

            for (var i = 0; i < names.Length; i++)
            {
                var authorIndex = randomGenerator.Next(authors.Count);

                var bookToCreate = new Book
                {
                    AuthorID = authors[authorIndex].AuthorID,
                    Name = names[i],
                    PageCount = i != nullIndex ? (int?)pageCounts[i] : null
                };

                if (i == nullIndex)
                {
                    queryOperations.Add(new[]
                    {
                        _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookToCreate.BookID)),
                        _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                        _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                    }.OrderBy(_ => new Guid()));
                }
                else
                {
                    queryOperations.Add(new[]
                    {
                        _metadataGenerator.GetQueryOperation("BookID", ("BookID", bookToCreate.BookID)),
                        _metadataGenerator.GetQueryOperation("Name", ("Name", bookToCreate.Name)),
                        _metadataGenerator.GetQueryOperation("AuthorID", ("AuthorID", bookToCreate.AuthorID)),
                        _metadataGenerator.GetQueryOperation("PageCount", ("PageCount", bookToCreate.PageCount)),
                        _metadataGenerator.GetQueryOperation("TestScope", ("TestScope", _databaseFixture.TestScope)),
                        _metadataGenerator.GetQueryOperation("TestID", ("TestID", testId))
                    }.OrderBy(_ => new Guid()));
                }
            }

            // Act
            Action act = () => builder.AddQueryToContext(context, new ParsedValuesListsQueryOperations
            {
                MultiQueryOperations = queryOperations
            }, null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Value must be supplied for 'AuthorID'.");
        }
    }

    public class TestInsertQueryBuilderContext
    {
        public IEnumerable<string> OrderedColumnNames { get; set; }
    }

    public class TestInsertQueryBuilder : QueryBuilder<TestInsertQueryBuilderContext, object, object>
    {
        private readonly IInsertColumnsFormatter _insertColumnsFormatter;
        private readonly IValuesListFormatter _valuesListFormatter;

        public TestInsertQueryBuilder(IInsertColumnsFormatter insertColumnsFormatter,
            IValuesListFormatter valuesListFormatter, string queryString,
            IDictionary<string, MergeOperationMetadata> metadata,
            IDictionary<string, QueryOperation> defaultOperations)
        {
            QueryFormat = queryString;
            _insertOperationMetadata = metadata;
            _defaultOperations = defaultOperations;
            _insertColumnsFormatter = insertColumnsFormatter;
            _valuesListFormatter = valuesListFormatter;
        }
        
        private readonly IDictionary<string, MergeOperationMetadata> _insertOperationMetadata;

        private readonly IDictionary<string, QueryOperation> _defaultOperations;
        public override string QueryFormat { get; }

        public override TestInsertQueryBuilderContext InitializeContext()
        {
            return new TestInsertQueryBuilderContext();
        }

        public override ParsedQueryOperations GetOperationsFromObject1(object operationObject)
        {
            throw new NotImplementedException();
        }

        public override string GetFormattedOperations1(IQueryContext context, ParsedQueryOperations operations,
            TestInsertQueryBuilderContext builderContext)
        {
            string formattedValuesLists;
            IEnumerable<MergeOperationMetadata> orderedMetadata;

            if (operations is ParsedValuesListsQueryOperations valuesListsOperations)
            {
                (formattedValuesLists, orderedMetadata) = _valuesListFormatter.FormatValuesLists(context,
                    valuesListsOperations.MultiQueryOperations, _insertOperationMetadata, _defaultOperations);
            }
            else
            {
                (formattedValuesLists, orderedMetadata) = _valuesListFormatter.FormatValuesLists(context,
                    new [] {operations.QueryOperations}, _insertOperationMetadata, _defaultOperations);
            }

            builderContext.OrderedColumnNames = orderedMetadata.Select(m => m.ReferencedColumn);

            return formattedValuesLists;
        }

        public override ParsedQueryOperations GetOperationsFromObject2(object operationObject)
        {
            throw new NotImplementedException();
        }

        public override string GetFormattedOperations2(IQueryContext context, ParsedQueryOperations operations,
            TestInsertQueryBuilderContext builderContext)
        {
            if (builderContext?.OrderedColumnNames is null)
            {
                return _insertColumnsFormatter.FormatInsertColumns(operations.QueryOperations, _insertOperationMetadata);
            }

            return _insertColumnsFormatter.FormatInsertColumns(builderContext.OrderedColumnNames);
        }
    }
}