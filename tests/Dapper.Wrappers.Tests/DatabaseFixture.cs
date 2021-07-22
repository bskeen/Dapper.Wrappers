using System;
using System.Collections.Generic;
using System.Data;
// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Dapper.Wrappers.DependencyInjection;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace Dapper.Wrappers.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public IDbConnection SqlConnection { get; private set; }
        public IDbConnection PostgresConnection { get; private set; }
        public Guid TestScope { get; }

        public DatabaseFixture(IEnumerable<IDbConnection> connections)
        {
            var connectionList = connections.ToList();
            SqlConnection = connectionList.FirstOrDefault(c => c is SqlConnection);
            PostgresConnection = connectionList.FirstOrDefault(c => c is NpgsqlConnection);
            TestScope = Guid.NewGuid();
        }

        public IDbConnection GetConnection(SupportedDatabases dbType) =>
            dbType == SupportedDatabases.SqlServer ? SqlConnection : PostgresConnection;

        public void Dispose()
        {
            RemoveSqlServerEntities();
            RemovePostgresEntities();
        }

        private void RemoveSqlServerEntities()
        {
            if (!(SqlConnection is null))
            {
                var deleteQuery = @"DELETE FROM [BookGenres] WHERE [TestScope] = @testScope;
                                    DELETE FROM [Genres] WHERE [TestScope] = @testScope;
                                    DELETE FROM [Books] WHERE [TestScope] = @testScope;
                                    DELETE FROM [BookGenres] WHERE [TestScope] = @testScope;";
                    
                SqlConnection.Execute(deleteQuery, new {testScope = TestScope});
                SqlConnection = null;
            }
        }

        private void RemovePostgresEntities()
        {
            if (!(PostgresConnection is null))
            {
                var deleteQuery = @"DELETE FROM ""BookGenres"" WHERE ""TestScope"" = :testscope;
                                    DELETE FROM ""Genres"" WHERE ""TestScope"" = :testscope;
                                    DELETE FROM ""Books"" WHERE ""TestScope"" = :testscope;
                                    DELETE FROM ""BookGenres"" WHERE ""TestScope"" = :testscope;";

                PostgresConnection.Execute(deleteQuery, new {testscope = TestScope});
            }
        }
    }
}
