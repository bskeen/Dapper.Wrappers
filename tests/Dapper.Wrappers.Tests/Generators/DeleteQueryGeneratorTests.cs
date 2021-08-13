using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Formatters;
using Dapper.Wrappers.Generators;
using Dapper.Wrappers.Tests.DbModels;
using FluentAssertions;
using Xunit;

namespace Dapper.Wrappers.Tests.Generators
{
    public class DeleteQueryGeneratorTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;

        public DeleteQueryGeneratorTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        private TestDeleteQueryGenerator GetTestInstance(SupportedDatabases dbType, string deleteQueryString,
            IDictionary<string, QueryOperationMetadata> filterOperationMetadata)
        {
            var formatter = _databaseFixture.GetFormatter(dbType);

            return new TestDeleteQueryGenerator(formatter, deleteQueryString, filterOperationMetadata);
        }

        private IQueryContext GetQueryContext(SupportedDatabases dbType)
        {
            var connection = _databaseFixture.GetConnection(dbType);

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

            await _databaseFixture.AddGenres(dbType, testId, genres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Genres.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultGenreFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultGenreFilterMetadata;

            var generator = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var genreIndex = randomGenerator.Next(genres.Count);

            // Act
            var operations = new[]
            {
                new QueryOperation
                {
                    Name = operationName,
                    Parameters = new Dictionary<string, object>
                    {
                        {"GenreID", genres[genreIndex].GenreID}
                    }
                },
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddDeleteQuery(context, operations);
            await context.ExecuteCommands();

            // Assert
            var genresLeft = (await _databaseFixture.GetGenres(dbType, testId)).ToList();

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

            await _databaseFixture.AddGenres(dbType, testId, genres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Genres.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultGenreFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultGenreFilterMetadata;

            var generator = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);
            var idsToDelete = genres.Skip(startIndex).Take(endIndex - startIndex).Select(g => g.GenreID).ToList();

            // Act
            var operations = new[]
            {
                new QueryOperation
                {
                    Name = "GenreIDIn",
                    Parameters = new Dictionary<string, object>
                    {
                        {"GenreIDs", idsToDelete}
                    }
                },
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddDeleteQuery(context, operations);
            await context.ExecuteCommands();

            // Assert
            var genresLeft = (await _databaseFixture.GetGenres(dbType, testId)).ToList();

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

            await _databaseFixture.AddGenres(dbType, testId, genres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Genres.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultGenreFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultGenreFilterMetadata;

            var generator = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            // Act
            var operations = new[]
            {
                new QueryOperation
                {
                    Name = operationName,
                    Parameters = new Dictionary<string, object>
                    {
                        {"GenreName", operationValue}
                    }
                },
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddDeleteQuery(context, operations);
            await context.ExecuteCommands();

            // Assert
            var genresLeft = (await _databaseFixture.GetGenres(dbType, testId)).ToList();

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

            await _databaseFixture.AddGenres(dbType, testId, genres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Genres.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultGenreFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultGenreFilterMetadata;

            var generator = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);
            var namesToDelete = genres.Skip(startIndex).Take(endIndex - startIndex).Select(g => g.Name).ToList();

            // Act
            var operations = new[]
            {
                new QueryOperation
                {
                    Name = "NameIn",
                    Parameters = new Dictionary<string, object>
                    {
                        {"GenreNames", namesToDelete}
                    }
                },
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddDeleteQuery(context, operations);
            await context.ExecuteCommands();

            // Assert
            var genresLeft = (await _databaseFixture.GetGenres(dbType, testId)).ToList();

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

            await _databaseFixture.AddAuthors(dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(dbType, testId, books);
            await _databaseFixture.AddGenres(dbType, testId, testData.Genres);
            await _databaseFixture.AddBookGenres(dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Books.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var bookIndex = randomGenerator.Next(books.Count);

            // Act
            var operations = new[]
            {
                new QueryOperation
                {
                    Name = operationName,
                    Parameters = new Dictionary<string, object>
                    {
                        {"BookID", books[bookIndex].BookID}
                    }
                },
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddDeleteQuery(context, operations);
            await context.ExecuteCommands();

            // Assert
            var booksLeft = (await _databaseFixture.GetBooks(dbType, testId)).ToList();

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

            await _databaseFixture.AddAuthors(dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(dbType, testId, books);
            await _databaseFixture.AddGenres(dbType, testId, testData.Genres);
            await _databaseFixture.AddBookGenres(dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Books.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            // Act
            var operations = new[]
            {
                new QueryOperation
                {
                    Name = operationName,
                    Parameters = new Dictionary<string, object>
                    {
                        {"PageCount", value}
                    }
                },
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddDeleteQuery(context, operations);
            await context.ExecuteCommands();

            // Assert
            var booksLeft = (await _databaseFixture.GetBooks(dbType, testId)).ToList();

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

            await _databaseFixture.AddAuthors(dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(dbType, testId, books);
            await _databaseFixture.AddGenres(dbType, testId, testData.Genres);
            await _databaseFixture.AddBookGenres(dbType, testId, testData.BookGenres);

            var query = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.DeleteQuery
                : SqlQueryFormatConstants.Postgres.Books.DeleteQuery;

            var metadata = dbType == SupportedDatabases.SqlServer
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var generator = GetTestInstance(dbType, query, metadata);
            var context = GetQueryContext(dbType);

            var nullPageCountId = books.Where(b => !b.PageCount.HasValue).Select(b => b.BookID).FirstOrDefault();

            // Act
            var operations = new[]
            {
                new QueryOperation
                {
                    Name = operationName,
                    Parameters = new Dictionary<string, object>()
                },
                new QueryOperation
                {
                    Name = "TestIDEquals",
                    Parameters = new Dictionary<string, object>
                    {
                        {"TestID", testId}
                    }
                }
            };

            generator.AddDeleteQuery(context, operations);
            await context.ExecuteCommands();

            // Assert
            var booksLeft = (await _databaseFixture.GetBooks(dbType, testId)).ToList();

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
    }

    public class TestDeleteQueryGenerator : DeleteQueryGenerator
    {
        public TestDeleteQueryGenerator(IQueryFormatter queryFormatter, string deleteQueryString,
            IDictionary<string, QueryOperationMetadata> filterOperationMetadata) : base(queryFormatter)
        {
            FilterOperationMetadata = filterOperationMetadata;
            DeleteQueryString = deleteQueryString;
        }

        protected override IDictionary<string, QueryOperationMetadata> FilterOperationMetadata { get; }
        protected override string DeleteQueryString { get; }
    }
}
