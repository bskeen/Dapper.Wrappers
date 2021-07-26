using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Tests.Formatters
{
    public static class FormatterTestConstants
    {
        public static class SqlServer
        {
            public const string BaseGetQuery = "SELECT * FROM Genres {0} {1} {2}";

            public const string TestGetWhere = "WHERE GenreID IN @GenreIDs";

            public const string TestGetOrder = "ORDER BY Name ASC";

            public const string GetWithoutAnyAddonsQuery = "SELECT * FROM Genres   ";
            public const string GetWithFilterQuery = "SELECT * FROM Genres WHERE GenreID IN @GenreIDs  ";
            public const string GetWithOrderingQuery = "SELECT * FROM Genres  ORDER BY Name ASC ";

            public const string GetWithOrderingPaginationQuery =
                "SELECT * FROM Genres  ORDER BY Name ASC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

            public const string GetWithFilterAndOrderingQuery = "SELECT * FROM Genres WHERE GenreID IN @GenreIDs ORDER BY Name ASC ";

            public const string GetWithAllPieces =
                "SELECT * FROM Genres WHERE GenreID IN @GenreIDs ORDER BY Name ASC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

            public const string BaseInsertQuery = "INSERT INTO Genres ({0}) VALUES ({1});";

            public const string TestColumns = "[Column1], [Column2], [Column3]";
            public const string TestValues = "@Value1, @Value2, @Value3";

            public const string TestSubqueries =
                "(SELECT 1 FROM TestTable), (SELECT 2 FROM TestTable), (SELECT 3 FROM TestTable)";

            public const string InsertWithColumnsAndValues =
                "INSERT INTO Genres ([Column1], [Column2], [Column3]) VALUES (@Value1, @Value2, @Value3);";

            public const string InsertWithColumnsAndSubqueries =
                "INSERT INTO Genres ([Column1], [Column2], [Column3]) VALUES ((SELECT 1 FROM TestTable), (SELECT 2 FROM TestTable), (SELECT 3 FROM TestTable));";
        }
    }
}
