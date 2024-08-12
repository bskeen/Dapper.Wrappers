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
    public class GetQueryGeneratorTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IDbConnection _sqlConnection;
        private readonly IDbConnection _postgresConnection;
        private readonly IMetadataGenerator _metadataGenerator;

        public GetQueryGeneratorTests(DatabaseFixture databaseFixture, IEnumerable<IDbConnection> connections,
            IMetadataGenerator metadataGenerator)
        {
            _databaseFixture = databaseFixture;
            var dbConnections = connections.ToList();
            _sqlConnection = dbConnections.FirstOrDefault(c => c is SqlConnection);
            _postgresConnection = dbConnections.FirstOrDefault(c => c is NpgsqlConnection);
            _metadataGenerator = metadataGenerator;
        }

        private TestGetQueryBuilder GetTestInstance(string dbType, string queryString,
            string defaultOrdering, IDictionary<string, QueryOperationMetadata> filterMetadata,
            IDictionary<string, QueryOperationMetadata> orderMetadata)
        {
            var formatter = _databaseFixture.GetFormatter(dbType);
            var filterFormatter = new FilterFormatter(formatter);
            var orderingFormatter = new OrderingFormatter(formatter);

            return new TestGetQueryBuilder(filterFormatter, orderingFormatter, queryString, defaultOrdering,
                filterMetadata, orderMetadata);
        }

        private IDbConnection GetConnection(string dbType) =>
            dbType == DatabaseFixture.SQL_SERVER_NAME ? _sqlConnection : _postgresConnection;

        private IQueryContext GetQueryContext(string dbType)
        {
            var connection = GetConnection(dbType);

            return new QueryContext(connection);
        }

        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME)]
        [InlineData(DatabaseFixture.POSTGRES_NAME)]
        public async Task AddGetQuery_WithoutFiltersOrderingOrPagination_ShouldReturnAllResults(
            string dbType)
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

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new QueryOperation[] { };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedQueryOperations
            {
                QueryOperations = orderOperations
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            results.Should().HaveCount(books.Count);
            results.OrderBy(b => b.BookID).Should().BeEquivalentTo(books.OrderBy(b => b.BookID));
        }

        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Desc)]
        public async Task AddGetQuery_WithOrderingSpecified_CorrectlySortsTheOutput(string dbType,
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

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(operationName,
                    (DapperWrappersConstants.OrderByDirectionParameter, direction.ToString()))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedQueryOperations
            {
                QueryOperations = orderOperations
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            results.Should().HaveCount(books.Count);

            List<Book> orderedBooks;
            switch (operationName)
            {
                case "Name":
                    orderedBooks = direction == OrderDirections.Asc
                        ? books.OrderBy(b => b.Name).ToList()
                        : books.OrderByDescending(b => b.Name).ToList();
                    break;
                case "PageCount":
                    // This is necessary, since Postgres and SQL Server handle null values differently when ordering.
                    var defaultPageCount = dbType == DatabaseFixture.SQL_SERVER_NAME ? -10000 : 10000;
                    orderedBooks = direction == OrderDirections.Asc
                        ? books.OrderBy(b => b.PageCount ?? defaultPageCount).ToList()
                        : books.OrderByDescending(b => b.PageCount ?? defaultPageCount).ToList();
                    break;
                default:
                    orderedBooks = books;
                    break;
            }

            for (var i = 0; i < results.Count; i++)
            {
                results[i].Should().BeEquivalentTo(orderedBooks[i]);
            }
        }

        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Asc, 0, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Asc, 0, 12)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Asc, 5, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Asc, 10, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Asc, 12, 1)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Asc, 8, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Asc, 0, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Asc, 0, 12)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Asc, 5, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Asc, 10, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Asc, 12, 1)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Asc, 8, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Desc, 0, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Desc, 0, 12)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Desc, 5, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Desc, 10, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Desc, 12, 1)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Name", OrderDirections.Desc, 8, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Desc, 0, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Desc, 0, 12)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Desc, 5, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Desc, 10, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Desc, 12, 1)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "PageCount", OrderDirections.Desc, 8, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Asc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Asc, 0, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Asc, 0, 12)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Asc, 5, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Asc, 10, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Asc, 12, 1)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Asc, 8, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Asc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Asc, 0, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Asc, 0, 12)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Asc, 5, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Asc, 10, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Asc, 12, 1)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Asc, 8, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Desc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Desc, 0, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Desc, 0, 12)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Desc, 5, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Desc, 10, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Desc, 12, 1)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Name", OrderDirections.Desc, 8, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Desc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Desc, 0, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Desc, 0, 12)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Desc, 5, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Desc, 10, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Desc, 12, 1)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "PageCount", OrderDirections.Desc, 8, 5)]
        public async Task AddGetQuery_WithOrderingAndPagination_ReturnsCorrectResults(string dbType,
            string orderOperation, OrderDirections direction, int skip, int take)
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

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(orderOperation,
                    (DapperWrappersConstants.OrderByDirectionParameter, direction.ToString()))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedOrderingQueryOperations
            {
                QueryOperations = orderOperations,
                Pagination = new Pagination
                {
                    Skip = skip,
                    Take = take
                }
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            List<Book> orderedBooks;
            switch (orderOperation)
            {
                case "Name":
                    orderedBooks = direction == OrderDirections.Asc
                        ? books.OrderBy(b => b.Name).ToList()
                        : books.OrderByDescending(b => b.Name).ToList();
                    break;
                case "PageCount":
                    // This is necessary, since Postgres and SQL Server handle null values differently when ordering.
                    var defaultPageCount = dbType == DatabaseFixture.SQL_SERVER_NAME ? -10000 : 10000;
                    orderedBooks = direction == OrderDirections.Asc
                        ? books.OrderBy(b => b.PageCount ?? defaultPageCount).ToList()
                        : books.OrderByDescending(b => b.PageCount ?? defaultPageCount).ToList();
                    break;
                default:
                    orderedBooks = books;
                    break;
            }

            orderedBooks = orderedBooks.Skip(skip).Take(take).ToList();

            results.Should().HaveCount(orderedBooks.Count);

            for (var i = 0; i < results.Count; i++)
            {
                results[i].Should().BeEquivalentTo(orderedBooks[i]);
            }
        }

        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, 0, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, 0, 12)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, 5, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, 10, 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, 12, 1)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, 8, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, 0, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, 0, 12)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, 5, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, 10, 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, 12, 1)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, 8, 5)]
        public async Task AddGetQuery_WithPaginationAndNoOrdering_AppliesDefaultOrderingToResults(
            string dbType, int skip, int take)
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

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new QueryOperation[] { };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedOrderingQueryOperations
            {
                QueryOperations = orderOperations,
                Pagination = new Pagination
                {
                    Skip = skip,
                    Take = take
                }
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            List<Book> orderedBooks = books.OrderBy(b => b.Name).Skip(skip).Take(take).ToList();

            results.Should().HaveCount(orderedBooks.Count);

            for (var i = 0; i < results.Count; i++)
            {
                results[i].Should().BeEquivalentTo(orderedBooks[i]);
            }
        }

        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "BookIDEquals")]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "BookIDNotEquals")]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "BookIDEquals")]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "BookIDNotEquals")]
        public async Task AddGetQuery_WithBookIDFilters_ShouldOnlyReturnExpectedResults(string dbType,
            string operationName)
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

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            var randomGenerator = new Random();
            var bookIndex = randomGenerator.Next(books.Count);

            // Act
            var orderOperations = new QueryOperation[] { };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(operationName, ("BookID", books[bookIndex].BookID)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedQueryOperations
            {
                QueryOperations = orderOperations
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            if (operationName == "BookIDEquals")
            {
                results.Should().HaveCount(1);
                results[0].Should().BeEquivalentTo(books[bookIndex]);
            }
            else
            {
                results.Should().HaveCount(9);
                results.Should().NotContain(b => b.BookID == books[bookIndex].BookID);
            }
        }

        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "NameEquals", "The Cat in the Hat", 1)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "NameEquals", "Green Eggs and Ham", 0)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "NameNotEquals", "The Cat in the Hat", 9)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "NameNotEquals", "Green Eggs and Ham", 10)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "NameLike", "%ea%", 2)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "NameLike", "%blah%", 0)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "NameLike", "The%", 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "NameLike", "%of%", 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "NameEquals", "The Cat in the Hat", 1)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "NameEquals", "Green Eggs and Ham", 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "NameNotEquals", "The Cat in the Hat", 9)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "NameNotEquals", "Green Eggs and Ham", 10)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "NameLike", "%ea%", 2)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "NameLike", "%blah%", 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "NameLike", "The%", 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "NameLike", "%of%", 4)]
        public async Task AddGetQuery_WithNameFilters_ReturnsExpectedRows(string dbType,
            string operationName, string filterValue, int expectedCount)
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

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new QueryOperation[] { };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(operationName, ("BookName", filterValue)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedQueryOperations
            {
                QueryOperations = orderOperations
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            results.Should().HaveCount(expectedCount);
        }
        
        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Science Fiction", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantasy Fiction", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Science Fiction", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantasy Fiction", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Science Fiction", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantasy Fiction", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Science Fiction", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantasy Fiction", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Science Fiction", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantasy Fiction", "Name", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Science Fiction", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantasy Fiction", "PageCount", OrderDirections.Asc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Science Fiction", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantasy Fiction", "Name", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Science Fiction", "PageCount", OrderDirections.Desc)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantasy Fiction", "PageCount", OrderDirections.Desc)]
        public async Task AddGetQuery_WithFilteringAndOrdering_ReturnsExpectedOrderedResults(string dbType,
            string genreFilter, string orderOperation, OrderDirections direction)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var genres = testData.Genres.ToList();
            var bookGenres = testData.BookGenres.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(orderOperation,
                    (DapperWrappersConstants.OrderByDirectionParameter, direction.ToString()))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("HasGenre", ("GenreName", genreFilter)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedQueryOperations
            {
                QueryOperations = orderOperations
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            var genreId = genres.FirstOrDefault(g => g.Name == genreFilter)?.GenreID;
            var bookIds = genreId.HasValue
                ? bookGenres.Where(bg => bg.GenreID == genreId.Value).Select(bg => bg.BookID).ToHashSet()
                : new HashSet<Guid>();

            List<Book> orderedBooks = books.Where(b => bookIds.Contains(b.BookID)).ToList();

            switch (orderOperation)
            {
                case "Name":
                    orderedBooks = direction == OrderDirections.Asc
                        ? orderedBooks.OrderBy(b => b.Name).ToList()
                        : orderedBooks.OrderByDescending(b => b.Name).ToList();
                    break;
                case "PageCount":
                    // This is necessary, since Postgres and SQL Server handle null values differently when ordering.
                    var defaultPageCount = dbType == DatabaseFixture.SQL_SERVER_NAME ? -10000 : 10000;
                    orderedBooks = direction == OrderDirections.Asc
                        ? orderedBooks.OrderBy(b => b.PageCount ?? defaultPageCount).ToList()
                        : orderedBooks.OrderByDescending(b => b.PageCount ?? defaultPageCount).ToList();
                    break;
            }

            results.Should().HaveCount(orderedBooks.Count);

            for (var i = 0; i < results.Count; i++)
            {
                results[i].Should().BeEquivalentTo(orderedBooks[i]);
            }
        }

        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Asc, 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Asc, 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Asc, 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Asc, 10, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Asc, 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Asc, 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Asc, 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Asc, 10, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Asc, 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Asc, 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Asc, 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Asc, 10, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Desc, 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Desc, 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Desc, 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", "Name", OrderDirections.Desc, 10, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Desc, 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Desc, 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Desc, 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", "Name", OrderDirections.Desc, 10, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Desc, 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Desc, 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Desc, 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", "Name", OrderDirections.Desc, 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Asc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Asc, 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Asc, 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Asc, 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Asc, 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Asc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Asc, 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Asc, 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Asc, 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Asc, 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Asc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Asc, 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Asc, 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Asc, 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Asc, 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Desc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Desc, 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Desc, 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Desc, 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", "Name", OrderDirections.Desc, 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Desc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Desc, 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Desc, 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Desc, 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", "Name", OrderDirections.Desc, 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Desc, 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Desc, 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Desc, 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Desc, 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", "Name", OrderDirections.Desc, 10, 3)]
        public async Task AddGetQuery_WithFilterOrderAndPagination_ReturnsExpectedCorrectlyOrderedResults(
            string dbType, string genreFilter, string orderOperation, OrderDirections direction, int skip,
            int take)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var genres = testData.Genres.ToList();
            var bookGenres = testData.BookGenres.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new[]
            {
                _metadataGenerator.GetQueryOperation(orderOperation,
                    (DapperWrappersConstants.OrderByDirectionParameter, direction.ToString()))
            };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("HasGenre", ("GenreName", genreFilter)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedOrderingQueryOperations
            {
                QueryOperations = orderOperations,
                Pagination = new Pagination
                {
                    Skip = skip,
                    Take = take
                }
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            var genreId = genres.FirstOrDefault(g => g.Name == genreFilter)?.GenreID;
            var bookIds = genreId.HasValue
                ? bookGenres.Where(bg => bg.GenreID == genreId.Value).Select(bg => bg.BookID).ToHashSet()
                : new HashSet<Guid>();

            List<Book> orderedBooks = books.Where(b => bookIds.Contains(b.BookID)).ToList();

            switch (orderOperation)
            {
                case "Name":
                    orderedBooks = direction == OrderDirections.Asc
                        ? orderedBooks.OrderBy(b => b.Name).ToList()
                        : orderedBooks.OrderByDescending(b => b.Name).ToList();
                    break;
                case "PageCount":
                    // This is necessary, since Postgres and SQL Server handle null values differently when ordering.
                    var defaultPageCount = dbType == DatabaseFixture.SQL_SERVER_NAME ? -10000 : 10000;
                    orderedBooks = direction == OrderDirections.Asc
                        ? orderedBooks.OrderBy(b => b.PageCount ?? defaultPageCount).ToList()
                        : orderedBooks.OrderByDescending(b => b.PageCount ?? defaultPageCount).ToList();
                    break;
            }

            orderedBooks = orderedBooks.Skip(skip).Take(take).ToList();

            results.Should().HaveCount(orderedBooks.Count);

            for (var i = 0; i < results.Count; i++)
            {
                results[i].Should().BeEquivalentTo(orderedBooks[i]);
            }
        }

        [Theory]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Novel", 10, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Fantastic", 10, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", 0, 5)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", 1, 3)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", 2, 4)]
        [InlineData(DatabaseFixture.SQL_SERVER_NAME, "Children's Literature", 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Novel", 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Fantastic", 10, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", 0, 0)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", 0, 5)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", 1, 3)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", 2, 4)]
        [InlineData(DatabaseFixture.POSTGRES_NAME, "Children's Literature", 10, 3)]
        public async Task AddGetQuery_WithFilteringAndPagination_AppliesDefaultOrderingToQuery(
            string dbType, string genreFilter, int skip, int take)
        {
            // Arrange
            var testId = Guid.NewGuid();
            var connection = GetConnection(dbType);

            var testData = GeneratorTestConstants.TestData.GetTestData();

            var books = testData.Books.ToList();
            var genres = testData.Genres.ToList();
            var bookGenres = testData.BookGenres.ToList();

            await _databaseFixture.AddGenres(connection, dbType, testId, testData.Genres);
            await _databaseFixture.AddAuthors(connection, dbType, testId, testData.Authors);
            await _databaseFixture.AddBooks(connection, dbType, testId, books);
            await _databaseFixture.AddBookGenres(connection, dbType, testId, testData.BookGenres);

            var query = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            var defaultOrdering = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrdering
                : GeneratorTestConstants.Postgres.DefaultBookOrdering;

            var orderMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookOrderMetadata
                : GeneratorTestConstants.Postgres.DefaultBookOrderMetadata;

            var filterMetadata = dbType == DatabaseFixture.SQL_SERVER_NAME
                ? GeneratorTestConstants.SqlServer.DefaultBookFilterMetadata
                : GeneratorTestConstants.Postgres.DefaultBookFilterMetadata;

            var builder = GetTestInstance(dbType, query, defaultOrdering, filterMetadata, orderMetadata);
            var context = GetQueryContext(dbType);

            // Act
            var orderOperations = new QueryOperation[] { };

            var filterOperations = new[]
            {
                _metadataGenerator.GetQueryOperation("HasGenre", ("GenreName", genreFilter)),
                _metadataGenerator.GetQueryOperation("TestIDEquals", ("TestID", testId))
            };

            builder.AddQueryToContext(context, new ParsedQueryOperations
            {
                QueryOperations = filterOperations
            }, new ParsedOrderingQueryOperations
            {
                QueryOperations = orderOperations,
                Pagination = new Pagination
                {
                    Skip = skip,
                    Take = take
                }
            });

            var results = (await context.ExecuteNextQuery<Book>()).ToList();

            // Assert
            var genreId = genres.FirstOrDefault(g => g.Name == genreFilter)?.GenreID;
            var bookIds = genreId.HasValue
                ? bookGenres.Where(bg => bg.GenreID == genreId.Value).Select(bg => bg.BookID).ToHashSet()
                : new HashSet<Guid>();

            List<Book> orderedBooks = books.Where(b => bookIds.Contains(b.BookID)).OrderBy(b => b.Name).Skip(skip)
                .Take(take).ToList();

            results.Should().HaveCount(orderedBooks.Count);

            for (var i = 0; i < results.Count; i++)
            {
                results[i].Should().BeEquivalentTo(orderedBooks[i]);
            }
        }
    }

    public class TestGetQueryBuilder : QueryBuilder<object, object>
    {
        private readonly IFilterFormatter _filterFormatter;
        private readonly IOrderingFormatter _orderingFormatter;

        public TestGetQueryBuilder(IFilterFormatter filterFormatter, IOrderingFormatter orderingFormatter,
            string queryString, string defaultOrdering, IDictionary<string, QueryOperationMetadata> filterMetadata,
            IDictionary<string, QueryOperationMetadata> orderMetadata)
        {
            _filterFormatter = filterFormatter;
            _orderingFormatter = orderingFormatter;
            QueryFormat = queryString;
            _defaultOrdering = defaultOrdering;
            _filterOperationMetadata = filterMetadata;
            _orderOperationMetadata = orderMetadata;
        }

        private readonly IDictionary<string, QueryOperationMetadata> _filterOperationMetadata;

        private readonly string _defaultOrdering;

        private readonly IDictionary<string, QueryOperationMetadata> _orderOperationMetadata;

        public override string QueryFormat { get; }

        public override ParsedQueryOperations GetOperationsFromObject1(object operationObject)
        {
            throw new NotImplementedException();
        }

        public override ParsedQueryOperations GetOperationsFromObject2(object operationObject)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetFormattedOperations2(IQueryContext context, ParsedQueryOperations operations)
        {
            if (operations is ParsedOrderingQueryOperations orderingOperations)
            {
                var paginatedResult = _orderingFormatter.FormatOrderOperations(context, _orderOperationMetadata, _defaultOrdering,
                    orderingOperations.QueryOperations, orderingOperations.Pagination);

                return new[] {paginatedResult.orderOperations, paginatedResult.pagination};
            }

            var result = _orderingFormatter.FormatOrderOperations(context, _orderOperationMetadata, _defaultOrdering,
                operations.QueryOperations);

            return new[] {result.orderOperations, string.Empty};
        }

        public override IEnumerable<string> GetFormattedOperations1(IQueryContext context, ParsedQueryOperations operations)
        {
            return new [] {_filterFormatter.FormatFilterOperations(context, _filterOperationMetadata,
                operations.QueryOperations)};
        }
    }
}
