using System;
using System.Collections.Generic;
using System.Data;
// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Formatters;
using Dapper.Wrappers.Tests.DbModels;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace Dapper.Wrappers.Tests
{
    public class DatabaseFixture : IDisposable
    {
        private IDbConnection _sqlConnection;
        private readonly IQueryFormatter _sqlQueryFormatter;

        private IDbConnection _postgresConnection;
        private readonly IQueryFormatter _postgresQueryFormatter;

        public Guid TestScope { get; }

        public DatabaseFixture(IEnumerable<IDbConnection> connections, IEnumerable<IQueryFormatter> formatters)
        {
            var connectionList = connections.ToList();
            _sqlConnection = connectionList.FirstOrDefault(c => c is SqlConnection);
            _postgresConnection = connectionList.FirstOrDefault(c => c is NpgsqlConnection);

            var formatterList = formatters.ToList();
            _sqlQueryFormatter = formatterList.FirstOrDefault(f => f is SqlServerQueryFormatter);
            _postgresQueryFormatter = formatterList.FirstOrDefault(f => f is PostgresQueryFormatter);

            TestScope = Guid.NewGuid();
        }

        public IQueryFormatter GetFormatter(SupportedDatabases dbType) => dbType == SupportedDatabases.SqlServer
            ? _sqlQueryFormatter
            : _postgresQueryFormatter;

        public void Dispose()
        {
            RemoveSqlServerEntities();
            RemovePostgresEntities();
        }

        private void EnsureConnectionOpen(IDbConnection connection)
        {
            if ((connection?.State ?? ConnectionState.Open) == ConnectionState.Closed)
            {
                connection?.Open();
            }
        }

        public async Task AddAuthors(IDbConnection connection, SupportedDatabases dbType, Guid testId, IEnumerable<Author> authors)
        {
            EnsureConnectionOpen(connection);
            var queryParams = authors.Select(a => new
            {
                a.AuthorID,
                a.FirstName,
                a.LastName,
                TestScope,
                TestID = testId
            });

            string rawQuery = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Authors.InsertQuery
                : SqlQueryFormatConstants.Postgres.Authors.InsertQuery;

            string query = string.Format(rawQuery, "@AuthorID", "@FirstName", "@LastName", "@TestScope", "@TestID");

            using var transaction = connection.BeginTransaction();
            await connection.ExecuteAsync(query, queryParams, transaction);
            transaction.Commit();
        }

        public async Task<IEnumerable<Author>> GetAuthors(IDbConnection connection, SupportedDatabases dbType, Guid testId)
        {
            EnsureConnectionOpen(connection);
            string rawQuery = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Authors.SelectQuery
                : SqlQueryFormatConstants.Postgres.Authors.SelectQuery;

            string query = string.Format(rawQuery, "@TestID");

            return await connection.QueryAsync<Author>(query, new {TestID = testId});
        }

        public async Task AddBooks(IDbConnection connection, SupportedDatabases dbType, Guid testId, IEnumerable<Book> books)
        {
            EnsureConnectionOpen(connection);
            var queryParams = books.Select(b => new
            {
                b.BookID,
                b.Name,
                b.AuthorID,
                b.PageCount,
                TestScope,
                TestID = testId
            });

            string rawQuery = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.InsertQuery
                : SqlQueryFormatConstants.Postgres.Books.InsertQuery;

            string query = string.Format(rawQuery, "@BookID", "@Name", "@AuthorID", "@PageCount", "@TestScope",
                "@TestID");

            using var transaction = connection.BeginTransaction();
            await connection.ExecuteAsync(query, queryParams, transaction);
            transaction.Commit();
        }

        public async Task<IEnumerable<Book>> GetBooks(IDbConnection connection, SupportedDatabases dbType, Guid testId)
        {
            EnsureConnectionOpen(connection);
            string rawQuery = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Books.SelectQuery
                : SqlQueryFormatConstants.Postgres.Books.SelectQuery;

            string query = string.Format(rawQuery, "@TestID");

            return await connection.QueryAsync<Book>(query, new { TestID = testId });
        }

        public async Task AddGenres(IDbConnection connection, SupportedDatabases dbType, Guid testId, IEnumerable<Genre> genres)
        {
            EnsureConnectionOpen(connection);
            var queryParams = genres.Select(g => new
            {
                g.GenreID,
                g.Name,
                TestScope,
                TestID = testId
            });

            string rawQuery = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.InsertQuery
                : SqlQueryFormatConstants.Postgres.Genres.InsertQuery;

            string query = string.Format(rawQuery, "@GenreID", "@Name", "@TestScope", "@TestID");

            using var transaction = connection.BeginTransaction();
            await connection.ExecuteAsync(query, queryParams, transaction);
            transaction.Commit();
        }

        public async Task<IEnumerable<Genre>> GetGenres(IDbConnection connection, SupportedDatabases dbType, Guid testId)
        {
            EnsureConnectionOpen(connection);
            string rawQuery = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.Genres.SelectQuery
                : SqlQueryFormatConstants.Postgres.Genres.SelectQuery;

            string query = string.Format(rawQuery, "@TestID");

            return await connection.QueryAsync<Genre>(query, new { TestID = testId });
        }

        public async Task AddBookGenres(IDbConnection connection, SupportedDatabases dbType, Guid testId, IEnumerable<BookGenre> ids)
        {
            EnsureConnectionOpen(connection);
            var queryParams = ids.Select(id => new
            {
                id.BookID,
                id.GenreID,
                TestScope,
                TestID = testId
            });

            string rawQuery = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.BookGenres.InsertQuery
                : SqlQueryFormatConstants.Postgres.BookGenres.InsertQuery;

            string query = string.Format(rawQuery, "@BookID", "@GenreID", "@TestScope", "@TestID");

            using var transaction = connection.BeginTransaction();
            await connection.ExecuteAsync(query, queryParams, transaction);
            transaction.Commit();
        }

        public async Task<IEnumerable<BookGenre>> GetBookGenres(IDbConnection connection, SupportedDatabases dbType, Guid testId)
        {
            EnsureConnectionOpen(connection);
            string rawQuery = dbType == SupportedDatabases.SqlServer
                ? SqlQueryFormatConstants.SqlServer.BookGenres.SelectQuery
                : SqlQueryFormatConstants.Postgres.BookGenres.SelectQuery;

            string query = string.Format(rawQuery, "@TestID");

            return await connection.QueryAsync<BookGenre>(query, new { TestID = testId });
        }

        private void RemoveSqlServerEntities()
        {
            if (!(_sqlConnection is null))
            {
                EnsureConnectionOpen(_sqlConnection);

                var deleteQuery = @"DELETE FROM [BookGenres] WHERE [TestScope] = @TestScope;
                                    DELETE FROM [Genres] WHERE [TestScope] = @TestScope;
                                    DELETE FROM [Books] WHERE [TestScope] = @TestScope;
                                    DELETE FROM [BookGenres] WHERE [TestScope] = @TestScope;
                                    DELETE FROM [Authors] WHERE [TestScope] = @TestScope";

                using var transaction = _sqlConnection.BeginTransaction();
                    
                _sqlConnection.Execute(deleteQuery, new {TestScope}, transaction);
                transaction.Commit();
                _sqlConnection = null;
            }
        }

        private void RemovePostgresEntities()
        {
            if (!(_postgresConnection is null))
            {
                EnsureConnectionOpen(_postgresConnection);

                var deleteQuery = @"DELETE FROM ""BookGenres"" WHERE ""TestScope"" = @TestScope;
                                    DELETE FROM ""Genres"" WHERE ""TestScope"" = @TestScope;
                                    DELETE FROM ""Books"" WHERE ""TestScope"" = @TestScope;
                                    DELETE FROM ""BookGenres"" WHERE ""TestScope"" = @TestScope;
                                    DELETE FROM ""Authors"" WHERE ""TestScope"" = @TestScope";

                using var transaction = _postgresConnection.BeginTransaction();
                
                _postgresConnection.Execute(deleteQuery, new {TestScope}, transaction);
                transaction.Commit();
                _postgresConnection = null;
            }
        }
    }
}
