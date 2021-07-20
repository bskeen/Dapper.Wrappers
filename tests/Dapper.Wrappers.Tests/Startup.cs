using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Dapper.Wrappers.DependencyInjection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;

namespace Dapper.Wrappers.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var postgresConnectionString = config.GetConnectionString("Postgres");

            if (!string.IsNullOrWhiteSpace(postgresConnectionString))
            {
                services.AddTransient<IDbConnection, NpgsqlConnection>(_ => new NpgsqlConnection(postgresConnectionString));

                MigrateDatabase(SupportedDatabases.PostgreSQL, postgresConnectionString);
            }

            var sqlConnectionString = config.GetConnectionString("SqlServer");

            if (!string.IsNullOrWhiteSpace(sqlConnectionString))
            {
                services.AddTransient<IDbConnection, SqlConnection>(_ => new SqlConnection(sqlConnectionString));

                MigrateDatabase(SupportedDatabases.SqlServer, sqlConnectionString);
            }
        }

        private void MigrateDatabase(SupportedDatabases dbType, string connectionString)
        {
            var migrateServiceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                {
                    IMigrationRunnerBuilder builder;
                    if (dbType == SupportedDatabases.SqlServer)
                    {
                        builder = rb.AddSqlServer();
                    }
                    else
                    {
                        builder = rb.AddPostgres();
                    }

                    builder.WithGlobalConnectionString(connectionString)
                        .ScanIn(typeof(Startup).Assembly).For.Migrations();
                })
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);

            using var scope = migrateServiceProvider.CreateScope();

            var runner = migrateServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }
    }
}
