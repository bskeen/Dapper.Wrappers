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
        }
    }
}
