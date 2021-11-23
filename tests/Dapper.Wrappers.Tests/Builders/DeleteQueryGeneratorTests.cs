// © 2021 by Benjamin Skeen
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
    public class DeleteQueryBuilderTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IDbConnection _sqlConnection;
        private readonly IDbConnection _postgresConnection;
        private readonly IMetadataGenerator _metadataGenerator;

        public DeleteQueryBuilderTests(DatabaseFixture databaseFixture, IEnumerable<IDbConnection> connections,
            IMetadataGenerator metadataGenerator)
        {
            _databaseFixture = databaseFixture;
            var dbConnections = connections.ToList();
            _sqlConnection = dbConnections.FirstOrDefault(c => c is SqlConnection);
            _postgresConnection = dbConnections.FirstOrDefault(c => c is NpgsqlConnection);
            _metadataGenerator = metadataGenerator;
        }

        private TestDeleteQueryBuilder GetTestInstance(SupportedDatabases dbType, string deleteQueryString,
            IDictionary<string, QueryOperationMetadata> filterOperationMetadata)
        {
            var operationFormatter = _databaseFixture.GetFormatter(dbType);

            var filterFormatter = new FilterFormatter(operationFormatter);

            return new TestDeleteQueryBuilder(filterFormatter, deleteQueryString, filterOperationMetadata);
        }

        private IDbConnection GetConnection(SupportedDatabases dbType) =>
            dbType == SupportedDatabases.SqlServer ? _sqlConnection : _postgresConnection;

        private IQueryContext GetQueryContext(SupportedDatabases dbType)
        {
            var connection = GetConnection(dbType);

            return new QueryContext(connection);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "GenreIDEquals")]
        [InlineData(SupportedDatabases.SqlServer, "GenreIDNotEquals")]
        [InlineData(SupportedDatabases.PostgreSQL, "GenreIDEquals")]
        [InlineData(SupportedDatabases.PostgreSQL, "GenreIDNotEquals")]
        public async Task AddDeleteQuery_WithGenreIDOperations_ShouldDeleteExpectedRowsFromDatabase(
            SupportedDatabases dbType, string operationName)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var genres = testData.Genres.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddGenres(connection, dbType, testId, genres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Genres.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultGenreFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultGenreFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var genreIndex = randomGenerator.Next(genres.Count);

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation(operationName, ("GenreID", genres[genreIndex].GenreID)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var genresLeft = (await _databaseFixture.GetGenres(connection, dbType, testId)).ToList();

            if (operationName == "GenreIDEquals")
            {
                genresLeft.Should().HaveCount(genres.Count - 1);
                genresLeft.Should().NotContain(g => g.GenreID == genres[genreIndex].GenreID);
            }
            else
            {
                genresLeft.Should().HaveCount(1);
                genresLeft[0].GenreID.Should().Be(genres[genreIndex].GenreID);
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, 0, 1)]
        [InlineData(SupportedDatabases.SqlServer, 0, 0)]
        [InlineData(SupportedDatabases.SqlServer, 0, 20)]
        [InlineData(SupportedDatabases.SqlServer, 3, 11)]
        [InlineData(SupportedDatabases.SqlServer, 5, 17)]
        [InlineData(SupportedDatabases.SqlServer, 19, 20)]
        [InlineData(SupportedDatabases.PostgreSQL, 0, 1)]
        [InlineData(SupportedDatabases.PostgreSQL, 0, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, 0, 20)]
        [InlineData(SupportedDatabases.PostgreSQL, 3, 11)]
        [InlineData(SupportedDatabases.PostgreSQL, 5, 17)]
        [InlineData(SupportedDatabases.PostgreSQL, 19, 20)]
        public async Task AddDeleteQuery_WithGenreIDInOperation_ShouldDeleteTheExpectedRows(SupportedDatabases dbType,
            int startIndex, int endIndex)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var genres = testData.Genres.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddGenres(connection, dbType, testId, genres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Genres.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultGenreFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultGenreFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);
            var idsToDelete = genres.Skip(startIndex).Take(endIndex - startIndex).Select(g => g.GenreID).ToList();

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("GenreIDIn", ("GenreIDs", idsToDelete)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var genresLeft = (await _databaseFixture.GetGenres(connection, dbType, testId)).ToList();

            var expectedCount = genres.Count - (endIndex - startIndex);

            genresLeft.Should().HaveCount(expectedCount);
            genresLeft.Should().NotContain(genre => idsToDelete.Contains(genre.GenreID));
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "NameEquals", "High Fantasy", 19)]
        [InlineData(SupportedDatabases.SqlServer, "NameEquals", "Detective Novel", 19)]
        [InlineData(SupportedDatabases.SqlServer, "NameEquals", "History", 19)]
        [InlineData(SupportedDatabases.SqlServer, "NameEquals", "Fake Genre", 20)]
        [InlineData(SupportedDatabases.SqlServer, "NameEquals", "", 20)]
        [InlineData(SupportedDatabases.SqlServer, "NameEquals", null, 20)]
        [InlineData(SupportedDatabases.SqlServer, "NameNotEquals", "High Fantasy", 1)]
        [InlineData(SupportedDatabases.SqlServer, "NameNotEquals", "Detective Novel", 1)]
        [InlineData(SupportedDatabases.SqlServer, "NameNotEquals", "History", 1)]
        [InlineData(SupportedDatabases.SqlServer, "NameNotEquals", "Fake Genre", 0)]
        [InlineData(SupportedDatabases.SqlServer, "NameNotEquals", "", 0)]
        [InlineData(SupportedDatabases.SqlServer, "NameNotEquals", null, 20)]
        [InlineData(SupportedDatabases.SqlServer, "NameLike", "Fantasy%", 19)]
        [InlineData(SupportedDatabases.SqlServer, "NameLike", "%Fantasy%", 18)]
        [InlineData(SupportedDatabases.SqlServer, "NameLike", "%Fiction%", 12)]
        [InlineData(SupportedDatabases.SqlServer, "NameLike", "%Not%Here%", 20)]
        [InlineData(SupportedDatabases.SqlServer, "NameLike", "", 20)]
        [InlineData(SupportedDatabases.SqlServer, "NameLike", null, 20)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameEquals", "High Fantasy", 19)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameEquals", "Detective Novel", 19)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameEquals", "History", 19)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameEquals", "Fake Genre", 20)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameEquals", "", 20)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameEquals", null, 20)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameNotEquals", "High Fantasy", 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameNotEquals", "Detective Novel", 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameNotEquals", "History", 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameNotEquals", "Fake Genre", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameNotEquals", "", 0)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameNotEquals", null, 20)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameLike", "Fantasy%", 19)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameLike", "%Fantasy%", 18)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameLike", "%Fiction%", 12)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameLike", "%Not%Here%", 20)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameLike", "", 20)]
        [InlineData(SupportedDatabases.PostgreSQL, "NameLike", null, 20)]
        public async Task AddDeleteQuery_WithGenreNameOperations_ShouldDeleteTheExpectedRows(SupportedDatabases dbType,
            string operationName, string operationValue, int expectedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var genres = testData.Genres.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddGenres(connection, dbType, testId, genres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Genres.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultGenreFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultGenreFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation(operationName, ("GenreName", operationValue)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var genresLeft = (await _databaseFixture.GetGenres(connection, dbType, testId)).ToList();

            genresLeft.Should().HaveCount(expectedCount);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, 0, 1)]
        [InlineData(SupportedDatabases.SqlServer, 0, 0)]
        [InlineData(SupportedDatabases.SqlServer, 0, 20)]
        [InlineData(SupportedDatabases.SqlServer, 3, 11)]
        [InlineData(SupportedDatabases.SqlServer, 5, 17)]
        [InlineData(SupportedDatabases.SqlServer, 19, 20)]
        [InlineData(SupportedDatabases.PostgreSQL, 0, 1)]
        [InlineData(SupportedDatabases.PostgreSQL, 0, 0)]
        [InlineData(SupportedDatabases.PostgreSQL, 0, 20)]
        [InlineData(SupportedDatabases.PostgreSQL, 3, 11)]
        [InlineData(SupportedDatabases.PostgreSQL, 5, 17)]
        [InlineData(SupportedDatabases.PostgreSQL, 19, 20)]
        public async Task AddDeleteQuery_WithGenreNameInOperations_ShouldReturnExpectedCount(SupportedDatabases dbType,
            int startIndex, int endIndex)
        {

            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var genres = testData.Genres.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddGenres(connection, dbType, testId, genres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Genres.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultGenreFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultGenreFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);
            var namesToDelete = genres.Skip(startIndex).Take(endIndex - startIndex).Select(g => g.Name).ToList();

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("NameIn", ("GenreNames", namesToDelete)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var genresLeft = (await _databaseFixture.GetGenres(connection, dbType, testId)).ToList();

            var expectedCount = genres.Count - (endIndex - startIndex);

            genresLeft.Should().HaveCount(expectedCount);
            genresLeft.Should().NotContain(genre => namesToDelete.Contains(genre.Name));
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "BookIDEquals")]
        [InlineData(SupportedDatabases.SqlServer, "BookIDNotEquals")]
        [InlineData(SupportedDatabases.PostgreSQL, "BookIDEquals")]
        [InlineData(SupportedDatabases.PostgreSQL, "BookIDNotEquals")]
        public async Task AddDeleteQuery_WithBookIdOperations_DeletesTheExpectedRows(SupportedDatabases dbType,
            string operationName)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Books.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var bookIndex = randomGenerator.Next(books.Count);

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation(operationName, ("BookID", books[bookIndex].BookID)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var booksLeft = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            if (operationName == "BookIDEquals")
            {
                booksLeft.Should().HaveCount(books.Count - 1);
                booksLeft.Should().NotContain(b => b.BookID == books[bookIndex].BookID);
            }
            else
            {
                booksLeft.Should().HaveCount(1);
                booksLeft[0].BookID.Should().Be(books[bookIndex].BookID);
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "PageCountEquals", 513, 9)]
        [InlineData(SupportedDatabases.SqlServer, "PageCountEquals", 42, 10)]
        [InlineData(SupportedDatabases.SqlServer, "PageCountNotEquals", 513, 2)]
        [InlineData(SupportedDatabases.SqlServer, "PageCountNotEquals", 42, 1)]
        [InlineData(SupportedDatabases.SqlServer, "PageCountGreater", 307, 7)]
        [InlineData(SupportedDatabases.SqlServer, "PageCountLess", 307, 5)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCountEquals", 513, 9)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCountEquals", 42, 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCountNotEquals", 513, 2)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCountNotEquals", 42, 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCountGreater", 307, 7)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCountLess", 307, 5)]
        public async Task AddDeleteQuery_WithPageCountFilters_ShouldDeleteExpectedRows(SupportedDatabases dbType,
            string operationName, int value, int expectedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Books.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation(operationName, ("PageCount", value)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var booksLeft = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            booksLeft.Should().HaveCount(expectedCount);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "PageCountNull", 9)]
        [InlineData(SupportedDatabases.SqlServer, "PageCountNotNull", 1)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCountNull", 9)]
        [InlineData(SupportedDatabases.PostgreSQL, "PageCountNotNull", 1)]
        public async Task AddDeleteQuery_WithPageCountNullFilters_ShouldDeleteExpectedRows(SupportedDatabases dbType, string operationName, int expectedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Books.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            var nullPageCountId = books.Where(b => !b.PageCount.HasValue).Select(b => b.BookID).FirstOrDefault();

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation(operationName),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var booksLeft = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            booksLeft.Should().HaveCount(expectedCount);

            if (operationName == "PageCountNull")
            {
                booksLeft.Should().NotContain(book => book.BookID == nullPageCountId);
            }
            else
            {
                booksLeft[0].BookID.Should().Be(nullPageCountId);
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, 0, 0, 10)]
        [InlineData(SupportedDatabases.SqlServer, 1, 2000, 1)]
        [InlineData(SupportedDatabases.SqlServer, 1, 100, 8)]
        [InlineData(SupportedDatabases.SqlServer, 200, 500, 6)]
        [InlineData(SupportedDatabases.PostgreSQL, 0, 0, 10)]
        [InlineData(SupportedDatabases.PostgreSQL, 1, 2000, 1)]
        [InlineData(SupportedDatabases.PostgreSQL, 1, 100, 8)]
        [InlineData(SupportedDatabases.PostgreSQL, 200, 500, 6)]
        public async Task AddDeleteQuery_WithPageCountBetweenOperator_ShouldDeleteExpectedRows(
            SupportedDatabases dbType, int start, int end, int expectedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Books.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            // Act
            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("PageCountBetween", ("PageCountStart", start), ("PageCountEnd", end)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var booksLeft = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();

            booksLeft.Should().HaveCount(expectedCount);
            booksLeft.Should().NotContain(book =>
                book.PageCount.HasValue && book.PageCount.Value >= start && book.PageCount.Value <= end);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, "Romance", 10)]
        [InlineData(SupportedDatabases.SqlServer, "Fantasy Fiction", 7)]
        [InlineData(SupportedDatabases.SqlServer, "Novel", 6)]
        [InlineData(SupportedDatabases.PostgreSQL, "Romance", 10)]
        [InlineData(SupportedDatabases.PostgreSQL, "Fantasy Fiction", 7)]
        [InlineData(SupportedDatabases.PostgreSQL, "Novel", 6)]
        public async Task AddDeleteQuery_WithHasGenreOperation_DeletesExpectedRows(SupportedDatabases dbType,
            string genreName, int expectedCount)
        {
            // Arrange
            var testId = Guid.NewGuid();

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var genres = testData.Genres.ToList();

            var connection = GetConnection(dbType);

            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddGenres(connection, dbType, testId, genres);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Books.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            var removedGenreId = genres.FirstOrDefault(g => g.Name == genreName)?.GenreID;

            // Act
            var operations = new[]
            {
                this._metadataGenerator.GetQueryOperation("HasGenre", ("GenreName", genreName)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = operations
            });

            await context.ExecuteCommands();

            // Assert
            var booksLeft = (await _databaseFixture.GetBooks(connection, dbType, testId)).ToList();
            var bookGenresLeft = (await _databaseFixture.GetBookGenres(connection, dbType, testId)).ToList();

            booksLeft.Should().HaveCount(expectedCount);
            if (removedGenreId.HasValue)
            {
                bookGenresLeft.Should().NotContain(bg => bg.GenreID == removedGenreId.Value);
            }
        }

        public void Dispose()
        {
            _sqlConnection?.Dispose();
            _postgresConnection?.Dispose();
        }
    }

    public class TestDeleteCriteria
    {
        public HashSet<string> UpdatedProperties { get; } = new HashSet<string>();
    }

    public class TestDeleteQueryBuilder : QueryBuilder<object>
    {
        public TestDeleteQueryBuilder(IFilterFormatter filterFormatter,
            string deleteQueryString, IDictionary<string, QueryOperationMetadata> filterOperationMetadata)
        {
            QueryFormat = deleteQueryString;
            _filterOperationMetadata = filterOperationMetadata;
            _filterFormatter = filterFormatter;
        }

        private readonly IDictionary<string, QueryOperationMetadata> _filterOperationMetadata;
        private readonly IFilterFormatter _filterFormatter;

        public override string QueryFormat { get; }

        public override IEnumerable<string> GetFormattedOperations(IQueryContext context, ParsedQueryOperations operations)
        {
            return new []{_filterFormatter.FormatFilterOperations(context, _filterOperationMetadata, operations.QueryOperations)};
        }

        public override ParsedQueryOperations GetOperationsFromObject(object operationObject)
        {
            throw new NotImplementedException();
        }
    }
}
