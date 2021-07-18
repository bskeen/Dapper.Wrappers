using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Castle.Components.DictionaryAdapter;
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
