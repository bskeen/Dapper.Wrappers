// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers.Tests.DbModels
{
    public static class SqlQueryFormatConstants
    {
        public static class SqlServer
        {
            public static class Authors
            {
                public const string InsertQuery = @"
INSERT INTO
  [Authors]
    ([AuthorID]
    ,[FirstName]
    ,[LastName]
    ,[TestScope]
    ,[TestID])
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3}
  ,{4});";

                public const string SelectQuery = @"
SELECT
   [AuthorID]
  ,[FirstName]
  ,[LastName]
FROM
  [Authors]
WHERE
  [TestID] = {0};";
            }

            public static class BookGenres
            {
                public const string InsertQuery = @"
INSERT INTO
  [BookGenres]
    ([BookID]
    ,[GenreID]
    ,[TestScope]
    ,[TestID])
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3});";

                public const string SelectQuery = @"
SELECT
   [BookID]
  ,[GenreID]
FROM
  [BookGenres]
WHERE
  [TestID] = {0};";
            }

            public static class Books
            {
                public const string InsertQuery = @"
INSERT INTO
  [Books]
    ([BookID]
    ,[Name]
    ,[AuthorID]
    ,[PageCount]
    ,[TestScope]
    ,[TestID])
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3}
  ,{4}
  ,{5});";

                public const string SelectQuery = @"
SELECT
   [BookID]
  ,[Name]
  ,[AuthorID]
  ,[PageCount]
FROM
  [Books]
WHERE
  [TestID] = {0}";
            }

            public static class Genres
            {
                public const string InsertQuery = @"
INSERT INTO
  [Genres]
    ([GenreID]
    ,[Name]
    ,[TestScope]
    ,[TestID])
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3});";

                public const string SelectQuery = @"
SELECT
   [GenreID]
  ,[Name]
FROM
  [Genres]
WHERE
  [TestID] = {0};";
            }
        }

        public static class Postgres
        {
            public static class Authors
            {
                public const string InsertQuery = @"
INSERT INTO
  ""Authors""
    (""AuthorID""
    ,""FirstName""
    ,""LastName""
    ,""TestScope""
    ,""TestID"")
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3}
  ,{4});";

                public const string SelectQuery = @"
SELECT
   ""AuthorID""
  ,""FirstName""
  ,""LastName""
FROM
  ""Authors""
WHERE
  ""TestID"" = {0};";
            }

            public static class BookGenres
            {
                public const string InsertQuery = @"
INSERT INTO
  ""BookGenres""
    (""BookID""
    ,""GenreID""
    ,""TestScope""
    ,""TestID"")
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3});";

                public const string SelectQuery = @"
SELECT
   ""BookID""
  ,""GenreID""
FROM
  ""BookGenres""
WHERE
  ""TestID"" = {0};";
            }

            public static class Books
            {
                public const string InsertQuery = @"
INSERT INTO
  ""Books""
    (""BookID""
    ,""Name""
    ,""AuthorID""
    ,""PageCount""
    ,""TestScope""
    ,""TestID"")
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3}
  ,{4}
  ,{5});";

                public const string SelectQuery = @"
SELECT
   ""BookID""
  ,""Name""
  ,""AuthorID""
  ,""PageCount""
FROM
  ""Books""
WHERE
  ""TestID"" = {0}";
            }

            public static class Genres
            {
                public const string InsertQuery = @"
INSERT INTO
  ""Genres""
    (""GenreID""
    ,""Name""
    ,""TestScope""
    ,""TestID"")
VALUES
  ({0}
  ,{1}
  ,{2}
  ,{3});";

                public const string SelectQuery = @"
SELECT
   ""GenreID""
  ,""Name""
FROM
  ""Genres""
WHERE
  ""TestID"" = {0};";
            }
        }
    }
}
