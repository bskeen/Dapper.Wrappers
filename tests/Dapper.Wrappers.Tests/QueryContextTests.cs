// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Tests.DbModels;
using FluentAssertions;
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

        private IQueryContext GetDefaultQueryContext(SupportedDatabases dbType) =>
            new QueryContext(_dbFixture.GetConnection(dbType));

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

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task ExecuteNextQuery_WithNoQuery_ShouldThrowException(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            // Act
            Func<Task> act = async () => await context.ExecuteNextQuery<Genre>();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The context contains no queries to execute against the database.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task ExecuteCommands_WithNoQuery_ShouldThrowException(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            // Act
            Func<Task> act = async () => await context.ExecuteCommands();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The context contains no commands to execute against the database.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task AddQuery_WithStringInput_ShouldCombineQueriesInsideContext(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            var testGenres = new[]
            {
                new Genre
                {
                    GenreID = Guid.NewGuid(),
                    Name = "TestGenre1",
                },
                new Genre
                {
                    GenreID = Guid.NewGuid(),
                    Name = "TestGenre2",
                }
            };

            object[] variables1 =
            {
                $"@{context.AddVariable("genreid", testGenres[0].GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenres[0].Name, DbType.String)}",
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}"
            };

            object[] variables2 =
            {
                $"@{context.AddVariable("genreid", testGenres[1].GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenres[1].Name, DbType.String)}",
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}"
            };

            var insertFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerInsertQuery
                : SqlQueryFormatConstants.Genres.PostgresInsertQuery;

            string[] commands =
            {
                string.Format(insertFormat, variables1),
                string.Format(insertFormat, variables2)
            };

            // Act
            foreach (var command in commands)
            {
                context.AddQuery(command);
            }

            await context.ExecuteCommands();

            // Assert
            var verifySql = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerSelectQuery
                : SqlQueryFormatConstants.Genres.PostgresSelectQuery;

            var connection = _dbFixture.GetConnection(dbType);

            var results = await connection.QueryAsync<Genre>(string.Format(verifySql, "@testscope", "@ids"),
                new {testscope = _dbFixture.TestScope, ids = testGenres.Select(g => g.GenreID).ToList()});

            var genres = results.ToList();

            genres.Should().HaveSameCount(testGenres);
            genres.OrderBy(g => g.GenreID).Should().Equal(testGenres.OrderBy(g => g.GenreID),
                (g1, g2) => g1.GenreID == g2.GenreID && g1.Name == g2.Name);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task AddQuery_AfterExecutingOneBatch_ShouldRunAnotherBatch(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            var testGenres = new[]
            {
                new Genre
                {
                    GenreID = Guid.NewGuid(),
                    Name = "TestGenre1",
                },
                new Genre
                {
                    GenreID = Guid.NewGuid(),
                    Name = "TestGenre2",
                }
            };

            object[] variables1 =
            {
                $"@{context.AddVariable("genreid", testGenres[0].GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenres[0].Name, DbType.String)}",
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}"
            };

            var insertFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerInsertQuery
                : SqlQueryFormatConstants.Genres.PostgresInsertQuery;

            var command1 = string.Format(insertFormat, variables1);

            // Act
            context.AddQuery(command1);

            await context.ExecuteCommands();

            object[] variables2 =
            {
                $"@{context.AddVariable("genreid", testGenres[1].GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenres[1].Name, DbType.String)}",
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}"
            };

            var command2 = string.Format(insertFormat, variables2);

            context.AddQuery(command2);

            await context.ExecuteCommands();

            // Assert
            var verifySql = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerSelectQuery
                : SqlQueryFormatConstants.Genres.PostgresSelectQuery;

            var connection = _dbFixture.GetConnection(dbType);

            var results = await connection.QueryAsync<Genre>(string.Format(verifySql, "@testscope", "@ids"),
                new { testscope = _dbFixture.TestScope, ids = testGenres.Select(g => g.GenreID).ToList() });

            var genres = results.ToList();

            genres.Should().HaveSameCount(testGenres);
            genres.OrderBy(g => g.GenreID).Should().Equal(testGenres.OrderBy(g => g.GenreID),
                (g1, g2) => g1.GenreID == g2.GenreID && g1.Name == g2.Name);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task ExecuteCommands_AfterExecutingOneBatch_ShouldErrorIfCalledBeforeAdd(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            var testGenre = new Genre
            {
                GenreID = Guid.NewGuid(),
                Name = "TestGenre1",

            };

            // Act
            object[] variables =
            {
                $"@{context.AddVariable("genreid", testGenre.GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenre.Name, DbType.String)}",
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}"
            };

            var insertFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerInsertQuery
                : SqlQueryFormatConstants.Genres.PostgresInsertQuery;

            var command = string.Format(insertFormat, variables);

            context.AddQuery(command);

            await context.ExecuteCommands();

            // Assert
            Func<Task> act = async () => await context.ExecuteCommands();

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The context contains no commands to execute against the database.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task ExecuteNextQuery_WithNormalInputs_ShouldReturnResults(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            var testGenre = new Genre
            {
                GenreID = Guid.NewGuid(),
                Name = "TestGenre"
            };

            var testAuthor = new Author
            {
                AuthorID = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Author"
            };

            var connection = _dbFixture.GetConnection(dbType);

            var insertGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerInsertQuery
                : SqlQueryFormatConstants.Genres.PostgresInsertQuery;

            var insertGenre = string.Format(insertGenreFormat, "@genreid", "@name", "@testscope");

            await connection.ExecuteAsync(insertGenre,
                new {genreid = testGenre.GenreID, name = testGenre.Name, testscope = _dbFixture.TestScope});

            var insertAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerInsertQuery
                : SqlQueryFormatConstants.Authors.PostgresInsertQuery;

            var insertAuthor = string.Format(insertAuthorFormat, "@authorid", "@firstname", "@lastname", "@testscope");

            await connection.ExecuteAsync(insertAuthor,
                new
                {
                    authorid = testAuthor.AuthorID, firstname = testAuthor.FirstName, lastname = testAuthor.LastName,
                    testscope = _dbFixture.TestScope
                });

            var selectGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerSelectQuery
                : SqlQueryFormatConstants.Genres.PostgresSelectQuery;

            var selectAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerSelectQuery
                : SqlQueryFormatConstants.Authors.PostgresSelectQuery;

            // Act
            object[] selectGenreVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testGenre.GenreID})}"
            };

            var selectGenreQuery = string.Format(selectGenreFormat, selectGenreVariables); 
            context.AddQuery(selectGenreQuery);

            object[] selectAuthorVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testAuthor.AuthorID})}"
            };

            var selectAuthorQuery = string.Format(selectAuthorFormat, selectAuthorVariables);
            context.AddQuery(selectAuthorQuery);

            var genres = await context.ExecuteNextQuery<Genre>();
            var authors = await context.ExecuteNextQuery<Author>();

            // Assert
            var genreResults = genres.ToArray();
            var authorResults = authors.ToArray();

            genreResults.Should().HaveCount(1);
            authorResults.Should().HaveCount(1);

            genreResults[0].Should().BeEquivalentTo(testGenre);
            authorResults[0].Should().BeEquivalentTo(testAuthor);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task ExecuteNextQuery_CalledAfterAllQueriesRun_ShouldThrowException(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            var testGenre = new Genre
            {
                GenreID = Guid.NewGuid(),
                Name = "TestGenre"
            };

            var testAuthor = new Author
            {
                AuthorID = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Author"
            };

            var connection = _dbFixture.GetConnection(dbType);

            var insertGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerInsertQuery
                : SqlQueryFormatConstants.Genres.PostgresInsertQuery;

            var insertGenre = string.Format(insertGenreFormat, "@genreid", "@name", "@testscope");

            await connection.ExecuteAsync(insertGenre,
                new { genreid = testGenre.GenreID, name = testGenre.Name, testscope = _dbFixture.TestScope });

            var insertAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerInsertQuery
                : SqlQueryFormatConstants.Authors.PostgresInsertQuery;

            var insertAuthor = string.Format(insertAuthorFormat, "@authorid", "@firstname", "@lastname", "@testscope");

            await connection.ExecuteAsync(insertAuthor,
                new
                {
                    authorid = testAuthor.AuthorID,
                    firstname = testAuthor.FirstName,
                    lastname = testAuthor.LastName,
                    testscope = _dbFixture.TestScope
                });

            var selectGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerSelectQuery
                : SqlQueryFormatConstants.Genres.PostgresSelectQuery;

            var selectAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerSelectQuery
                : SqlQueryFormatConstants.Authors.PostgresSelectQuery;

            // Act
            object[] selectGenreVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testGenre.GenreID})}"
            };

            var selectGenreQuery = string.Format(selectGenreFormat, selectGenreVariables);
            context.AddQuery(selectGenreQuery);

            object[] selectAuthorVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testAuthor.AuthorID})}"
            };

            var selectAuthorQuery = string.Format(selectAuthorFormat, selectAuthorVariables);
            context.AddQuery(selectAuthorQuery);

            await context.ExecuteNextQuery<Genre>();
            await context.ExecuteNextQuery<Author>();

            // Assert
            Func<Task> act = async () => await context.ExecuteCommands();

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The context contains no commands to execute against the database.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task ExecuteCommands_AfterExecuteNextQuery_ShouldRunRemainingQueries(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            var testGenre = new Genre
            {
                GenreID = Guid.NewGuid(),
                Name = "TestGenre"
            };

            var testAuthor = new Author
            {
                AuthorID = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Author"
            };

            var connection = _dbFixture.GetConnection(dbType);

            var insertGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerInsertQuery
                : SqlQueryFormatConstants.Genres.PostgresInsertQuery;

            var insertGenre = string.Format(insertGenreFormat, "@genreid", "@name", "@testscope");

            await connection.ExecuteAsync(insertGenre,
                new { genreid = testGenre.GenreID, name = testGenre.Name, testscope = _dbFixture.TestScope });

            var insertAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerInsertQuery
                : SqlQueryFormatConstants.Authors.PostgresInsertQuery;

            var insertAuthor = string.Format(insertAuthorFormat, "@authorid", "@firstname", "@lastname", "@testscope");

            await connection.ExecuteAsync(insertAuthor,
                new
                {
                    authorid = testAuthor.AuthorID,
                    firstname = testAuthor.FirstName,
                    lastname = testAuthor.LastName,
                    testscope = _dbFixture.TestScope
                });

            var selectGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerSelectQuery
                : SqlQueryFormatConstants.Genres.PostgresSelectQuery;

            var selectAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerSelectQuery
                : SqlQueryFormatConstants.Authors.PostgresSelectQuery;

            // Act
            object[] selectGenreVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testGenre.GenreID})}"
            };

            var selectGenreQuery = string.Format(selectGenreFormat, selectGenreVariables);
            context.AddQuery(selectGenreQuery);

            object[] selectAuthorVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testAuthor.AuthorID})}"
            };

            var selectAuthorQuery = string.Format(selectAuthorFormat, selectAuthorVariables);
            context.AddQuery(selectAuthorQuery);

            var genres = await context.ExecuteNextQuery<Genre>();
            await context.ExecuteCommands();

            // Assert
            genres.Should().HaveCount(1);
            Func<Task> act = async () =>
            {
                var _ = await context.ExecuteNextQuery<Author>();
            };

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The context contains no queries to execute against the database.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task ExecuteCommands_WithPendingQueries_ShouldExecutePendingAndNewQueries(
            SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            var testGenres = new[]
            {
                new Genre
                {
                    GenreID = Guid.NewGuid(),
                    Name = "TestGenre1",
                },
                new Genre
                {
                    GenreID = Guid.NewGuid(),
                    Name = "TestGenre2",
                }
            };

            var testAuthor = new Author
            {
                AuthorID = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Author"
            };

            var connection = _dbFixture.GetConnection(dbType);

            var insertGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerInsertQuery
                : SqlQueryFormatConstants.Genres.PostgresInsertQuery;

            var insertGenre = string.Format(insertGenreFormat, "@genreid", "@name", "@testscope");

            await connection.ExecuteAsync(insertGenre,
                new { genreid = testGenres[0].GenreID, name = testGenres[0].Name, testscope = _dbFixture.TestScope });

            var insertAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerInsertQuery
                : SqlQueryFormatConstants.Authors.PostgresInsertQuery;

            var insertAuthor = string.Format(insertAuthorFormat, "@authorid", "@firstname", "@lastname", "@testscope");

            await connection.ExecuteAsync(insertAuthor,
                new
                {
                    authorid = testAuthor.AuthorID,
                    firstname = testAuthor.FirstName,
                    lastname = testAuthor.LastName,
                    testscope = _dbFixture.TestScope
                });

            var selectGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerSelectQuery
                : SqlQueryFormatConstants.Genres.PostgresSelectQuery;

            var selectAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerSelectQuery
                : SqlQueryFormatConstants.Authors.PostgresSelectQuery;

            // Act
            object[] selectGenreVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testGenres[0].GenreID})}"
            };

            var selectGenreQuery = string.Format(selectGenreFormat, selectGenreVariables);
            context.AddQuery(selectGenreQuery);

            object[] selectAuthorVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testAuthor.AuthorID})}"
            };

            var selectAuthorQuery = string.Format(selectAuthorFormat, selectAuthorVariables);
            context.AddQuery(selectAuthorQuery);

            var genres = await context.ExecuteNextQuery<Genre>();

            object[] insertNewGenreVariables =
            {
                $"@{context.AddVariable("genreid", testGenres[1].GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenres[1].Name, DbType.String)}",
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}"
            };

            var insertNewGenre = string.Format(insertGenreFormat, insertNewGenreVariables);

            context.AddQuery(insertNewGenre);

            await context.ExecuteCommands();

            // Assert
            var results = await connection.QueryAsync<Genre>(string.Format(selectGenreFormat, "@testscope", "@ids"),
                new { testscope = _dbFixture.TestScope, ids = testGenres.Select(g => g.GenreID).ToList() });

            var firstGenreResults = genres.ToArray();
            firstGenreResults.Should().HaveCount(1);
            firstGenreResults[0].Should().BeEquivalentTo(testGenres[0]);

            var lastGenreResults = results.ToList();

            lastGenreResults.Should().HaveSameCount(testGenres);
            lastGenreResults.OrderBy(g => g.GenreID).Should().Equal(testGenres.OrderBy(g => g.GenreID),
                (g1, g2) => g1.GenreID == g2.GenreID && g1.Name == g2.Name);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async Task ExecuteNextQuery_WithAddQueryCallInMiddle_ShouldRetrieveAllResults(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            var testGenre = new Genre
            {
                GenreID = Guid.NewGuid(),
                Name = "TestGenre1",
            };

            var testAuthor = new Author
            {
                AuthorID = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Author"
            };

            var connection = _dbFixture.GetConnection(dbType);

            var insertGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerInsertQuery
                : SqlQueryFormatConstants.Genres.PostgresInsertQuery;

            var insertGenre = string.Format(insertGenreFormat, "@genreid", "@name", "@testscope");

            await connection.ExecuteAsync(insertGenre,
                new { genreid = testGenre.GenreID, name = testGenre.Name, testscope = _dbFixture.TestScope });

            var insertAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerInsertQuery
                : SqlQueryFormatConstants.Authors.PostgresInsertQuery;

            var insertAuthor = string.Format(insertAuthorFormat, "@authorid", "@firstname", "@lastname", "@testscope");

            await connection.ExecuteAsync(insertAuthor,
                new
                {
                    authorid = testAuthor.AuthorID,
                    firstname = testAuthor.FirstName,
                    lastname = testAuthor.LastName,
                    testscope = _dbFixture.TestScope
                });

            var selectGenreFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Genres.SqlServerSelectQuery
                : SqlQueryFormatConstants.Genres.PostgresSelectQuery;

            var selectAuthorFormat = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.Authors.SqlServerSelectQuery
                : SqlQueryFormatConstants.Authors.PostgresSelectQuery;

            // Act
            object[] selectGenreVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testGenre.GenreID})}"
            };

            var selectGenreQuery = string.Format(selectGenreFormat, selectGenreVariables);
            context.AddQuery(selectGenreQuery);

            object[] selectAuthorVariables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testAuthor.AuthorID})}"
            };

            var selectAuthorQuery = string.Format(selectAuthorFormat, selectAuthorVariables);
            context.AddQuery(selectAuthorQuery);

            var genres1 = await context.ExecuteNextQuery<Genre>();

            object[] selectGenre2Variables =
            {
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("ids", new List<Guid> {testGenre.GenreID})}"
            };

            var selectGenre2Query = string.Format(selectGenreFormat, selectGenre2Variables);
            context.AddQuery(selectGenre2Query);

            var authors = await context.ExecuteNextQuery<Author>();
            var genres2 = await context.ExecuteNextQuery<Genre>();

            // Assert
            var genres1List = genres1.ToList();
            genres1List.Should().HaveCount(1);
            genres1List[0].Should().BeEquivalentTo(testGenre);

            var authorsList = authors.ToList();
            authorsList.Should().HaveCount(1);
            authorsList[0].Should().BeEquivalentTo(testAuthor);

            var genres2List = genres2.ToList();
            genres2List.Should().HaveCount(1);
            genres2List[0].Should().BeEquivalentTo(testGenre);
        }
    }
}
