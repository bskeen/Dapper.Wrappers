// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentMigrator;

namespace Dapper.Wrappers.Tests.DbMigrations
{
    [Migration(2021080900)]
    public class AddTestID : Migration
    {
        public override void Up()
        {
            Alter.Table("Authors")
                .AddColumn("TestID").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid);

            Alter.Table("Books")
                .AddColumn("TestID").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid);

            Alter.Table("Genres")
                .AddColumn("TestID").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid);

            Alter.Table("BookGenres")
                .AddColumn("TestID").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid);
        }

        public override void Down()
        {
            Delete.Column("TestID").FromTable("Authors");
            Delete.Column("TestID").FromTable("Books");
            Delete.Column("TestID").FromTable("Genres");
            Delete.Column("TestID").FromTable("BookGenres");
        }
    }
}
