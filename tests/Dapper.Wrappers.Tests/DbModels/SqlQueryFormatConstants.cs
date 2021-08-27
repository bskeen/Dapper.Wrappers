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
{0}
VALUES
{1};";

                public const string DefaultInsertQuery = @"
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

                public const string BasicSelectQuery = @"
SELECT
   [BookID]
  ,[Name]
  ,[AuthorID]
  ,[PageCount]
FROM
  [Books]
WHERE
  [TestID] = {0}";

                public const string SelectQuery = @"
SELECT
   [BookID]
  ,[Name]
  ,[AuthorID]
  ,[PageCount]
FROM
  [Books] b
{0}
{1}
{2};";

                public const string DeleteQuery = @"
SELECT
  [BookID]
INTO
  #BooksToDelete
FROM
  [Books] b
{0};

DELETE FROM
  [BookGenres]
WHERE
  [BookID] IN (SELECT [BookID] FROM #BooksToDelete);

DELETE FROM
  [Books]
WHERE
  [BookID] IN (SELECT [BookID] FROM #BooksToDelete);

DROP TABLE
  #BooksToDelete;";

                public const string UpdateQuery = @"
UPDATE
  b
{0}
FROM
  [Books] b
{1};";
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

                public const string DeleteQuery = @"
DELETE FROM
  [Genres]
{0}";
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

                public const string DeleteQuery = @"
DELETE FROM
  ""Authors""
{0}";
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
{0}
VALUES
{1};";

                public const string DefaultInsertQuery = @"
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

                public const string BasicSelectQuery = @"
SELECT
   ""BookID""
  ,""Name""
  ,""AuthorID""
  ,""PageCount""
FROM
  ""Books""
WHERE
  ""TestID"" = {0}";

                public const string SelectQuery = @"
SELECT
   ""BookID""
  ,""Name""
  ,""AuthorID""
  ,""PageCount""
FROM
  ""Books"" b
{0}
{1}
{2};";

                public const string DeleteQuery = @"
CREATE TEMPORARY TABLE
  books_to_delete
    (""BookID"" UUID NOT NULL);

INSERT INTO
  books_to_delete
    (""BookID"")
SELECT
  ""BookID""
FROM
  ""Books"" b
{0};

DELETE FROM
  ""BookGenres""
WHERE
  ""BookID"" IN (SELECT ""BookID"" FROM books_to_delete);

DELETE FROM
  ""Books""
WHERE
  ""BookID"" IN (SELECT ""BookID"" FROM books_to_delete);";

                public const string UpdateQuery = @"
UPDATE
  ""Books"" b
{0}
{1}";
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

                public const string DeleteQuery = @"
DELETE FROM
  ""Genres""
{0}";
            }
        }
    }
}
