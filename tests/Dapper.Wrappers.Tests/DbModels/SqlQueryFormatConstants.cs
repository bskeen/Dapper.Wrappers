using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Tests.DbModels
{
    public static class SqlQueryFormatConstants
    {
        public static class Genres
        {
            public const string SqlServerInsertQuery = @"
INSERT INTO
  [Genres]
    ([GenreID]
    ,[Name]
    ,[TestScope])
VALUES
  ({0}
  ,{1}
  ,{2});";

            public const string PostgresInsertQuery = @"
INSERT INTO
  ""Genres""
    (""GenreID""
    ,""Name""
    ,""TestScope"")
VALUES
  ({0}
  ,{1}
  ,{2});";

            public const string SqlServerSelectQuery = @"
SELECT
   [GenreID]
  ,[Name]
FROM
  [Genres]
WHERE
  [TestScope] = {0}
  AND [GenreID] IN {1};";

            public const string PostgresSelectQuery = @"
SELECT
   ""GenreID""
  ,""Name""
FROM
  ""Genres""
WHERE
  ""TestScope"" = {0}
  AND ""GenreID"" = ANY({1});";
        }

        public static class Authors
        {
            public const string SqlServerInsertQuery = @"
INSERT INTO
  [Authors]
    ([AuthorID]
    ,[FirstName]
    ,[LastName]
    ,[TestScope])
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3});";
            
            public const string PostgresInsertQuery = @"
INSERT INTO
  ""Authors""
    (""AuthorID""
    ,""FirstName""
    ,""LastName""
    ,""TestScope"")
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3});";

            public const string SqlServerSelectQuery = @"
SELECT
   [AuthorID]
  ,[FirstName]
  ,[LastName]
FROM
  [Authors]
WHERE
  [TestScope] = {0}
  AND [AuthorID] IN {1};";

            public const string PostgresSelectQuery = @"
SELECT
   ""AuthorID""
  ,""FirstName""
  ,""LastName""
FROM
  ""Authors""
WHERE
  ""TestScope"" = {0}
  AND ""AuthorID"" = ANY({1});";
        }
    }
}
