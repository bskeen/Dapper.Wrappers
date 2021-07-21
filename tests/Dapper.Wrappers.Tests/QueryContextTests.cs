﻿using System;
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

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async void ExecuteNextQuery_WithNoQuery_ShouldThrowException(SupportedDatabases dbType)
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
        public async void ExecuteCommands_WithNoQuery_ShouldThrowException(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);

            // Act
            Func<Task> act = async () => await context.ExecuteCommands();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The context contains no commands to execute against the database.");
        }

        private const string TestCombineStatements = @"INSERT INTO
                                                         [Genres]
                                                           ([GenreID]
                                                           ,[Name]
                                                           ,[TestScope])
                                                       VALUES
                                                         ({0}
                                                         ,{1}
                                                         ,{2});|INSERT INTO
                                                         [Genres]
                                                           ([GenreID]
                                                           ,[Name]
                                                           ,[TestScope])
                                                       VALUES
                                                         ({3}
                                                         ,{4}
                                                         ,{2});";

        private const string VerifySqlTestCombineStatements = @"SELECT
                                                                   [GenreID]
                                                                  ,[Name]
                                                                FROM
                                                                  [Genres]
                                                                WHERE
                                                                  [TestScope] = @testscope
                                                                  AND [GenreID] IN @ids";

        private const string VerifyPostgresTestCombineStatements = @"SELECT
                                                                      ""GenreID""
                                                                     ,""Name""
                                                                   FROM
                                                                     ""Genres""
                                                                   WHERE
                                                                     ""TestScope"" = @testscope
                                                                     AND ""GenreID"" = ANY(@ids)";

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async void AddQuery_WithStringInput_ShouldCombineQueriesInsideContext(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);
            var commandsToAdd = TestCombineStatements;
            if (dbType == SupportedDatabases.PostgreSQL)
            {
                commandsToAdd = commandsToAdd.Replace('[', '"');
                commandsToAdd = commandsToAdd.Replace(']', '"');
            }

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

            object[] variables =
            {
                $"@{context.AddVariable("genreid", testGenres[0].GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenres[0].Name, DbType.String)}",
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}",
                $"@{context.AddVariable("genreid", testGenres[1].GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenres[1].Name, DbType.String)}"
            };

            var formattedCommands = string.Format(commandsToAdd, variables);

            var splitCommands = formattedCommands.Split('|');

            // Act
            foreach (var command in splitCommands)
            {
                context.AddQuery(command);
            }

            await context.ExecuteCommands();

            // Assert
            var verifySql = dbType == SupportedDatabases.SqlServer
                ? VerifySqlTestCombineStatements
                : VerifyPostgresTestCombineStatements;

            var connection = dbType == SupportedDatabases.SqlServer
                ? _dbFixture.SqlConnection
                : _dbFixture.PostgresConnection;

            var results = await connection.QueryAsync<Genre>(verifySql,
                new {testscope = _dbFixture.TestScope, ids = testGenres.Select(g => g.GenreID).ToList()});

            var genres = results.ToList();

            genres.Should().HaveSameCount(testGenres);
            genres.OrderBy(g => g.GenreID).Should().Equal(testGenres.OrderBy(g => g.GenreID),
                (g1, g2) => g1.GenreID == g2.GenreID && g1.Name == g2.Name);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public async void AddQuery_AfterExecutingOneBatch_ShouldRunAnotherBatch(SupportedDatabases dbType)
        {
            // Arrange
            var context = GetDefaultQueryContext(dbType);
            var commandsToAdd = TestCombineStatements;
            if (dbType == SupportedDatabases.PostgreSQL)
            {
                commandsToAdd = commandsToAdd.Replace('[', '"');
                commandsToAdd = commandsToAdd.Replace(']', '"');
            }

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

            var splitCommands = commandsToAdd.Split('|');
            var command1 = string.Format(splitCommands[0], variables1);

            // Act
            context.AddQuery(command1);

            await context.ExecuteCommands();

            object[] variables2 =
            {
                $"@{context.AddVariable("genreid", testGenres[1].GenreID, DbType.Guid)}",
                $"@{context.AddVariable("genrename", testGenres[1].Name, DbType.String)}",
                $"@{context.AddVariable("testscope", _dbFixture.TestScope, DbType.Guid)}"
            };

            var command2 = string.Format(splitCommands[0], variables2);

            context.AddQuery(command2);

            await context.ExecuteCommands();

            // Assert
            var verifySql = dbType == SupportedDatabases.SqlServer
                ? VerifySqlTestCombineStatements
                : VerifyPostgresTestCombineStatements;

            var connection = dbType == SupportedDatabases.SqlServer
                ? _dbFixture.SqlConnection
                : _dbFixture.PostgresConnection;

            var results = await connection.QueryAsync<Genre>(verifySql,
                new { testscope = _dbFixture.TestScope, ids = testGenres.Select(g => g.GenreID).ToList() });

            var genres = results.ToList();

            genres.Should().HaveSameCount(testGenres);
            genres.OrderBy(g => g.GenreID).Should().Equal(testGenres.OrderBy(g => g.GenreID),
                (g1, g2) => g1.GenreID == g2.GenreID && g1.Name == g2.Name);
        }
    }
}
