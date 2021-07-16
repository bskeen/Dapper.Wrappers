using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using FluentMigrator;

namespace Dapper.Wrappers.Tests.DbMigrations
{
    [Migration(0)]
    public class InitDbMigration : Migration
    {
        public override void Up()
        {
            Create.Table("Authors")
                .WithColumn("AuthorID").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                .WithColumn("FirstName").AsString(256).NotNullable()
                .WithColumn("LastName").AsString(256).Nullable()
                .WithColumn("TestScope").AsGuid().NotNullable();

            Create.Table("Books")
                .WithColumn("BookID").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                .WithColumn("Name").AsString(256).NotNullable()
                .WithColumn("AuthorID").AsGuid().ForeignKey("Authors", "AuthorID").NotNullable()
                .WithColumn("PageCount").AsInt32().Nullable()
                .WithColumn("TestScope").AsGuid().NotNullable();

            Create.Table("Genres")
                .WithColumn("GenreID").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
                .WithColumn("Name").AsString(64).NotNullable()
                .WithColumn("TestScope").AsGuid().NotNullable();

            Create.Table("BookGenres")
                .WithColumn("BookID").AsGuid().NotNullable().ForeignKey("Books", "BookID")
                .WithColumn("GenreID").AsGuid().NotNullable().ForeignKey("Genres", "GenreID")
                .WithColumn("TestScope").AsGuid().NotNullable();

            Create.PrimaryKey("PK_BookGenres").OnTable("BookGenres").Columns("BookID", "GenreID");
        }

        public override void Down()
        {
            Delete.PrimaryKey("PK_BookGenres").FromTable("BookGenres");

            Delete.Table("BookGenres");

            Delete.Table("Genres");

            Delete.Table("Books");

            Delete.Table("Authors");
        }
    }
}
