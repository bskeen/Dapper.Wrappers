// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Wrappers.Generators;
using Dapper.Wrappers.Tests.DbModels;

namespace Dapper.Wrappers.Tests.Generators
{
    public static class GeneratorTestConstants
    {
        private static readonly IMetadataGenerator MetadataGenerator = new MetadataGenerator();

        public static class SqlServer
        {
            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "BookIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "[BookID] = {0}", "BookID")
                    },
                    {
                        "BookIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "[BookID] <> {0}", "BookID")
                    },
                    {
                        "BookIDIn",
                        MetadataGenerator.GetOperation("BookIDIn", "[BookID] IN {0}",
                            new[] {MetadataGenerator.GetParameter("BookIDs", null)})
                    },
                    {
                        "NameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameEquals", "[Name] = {0}", "BookName")
                    },
                    {
                        "NameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameNotEquals", "[Name] <> {0}", "BookName")
                    },
                    {
                        "NameIn",
                        MetadataGenerator.GetOperation("NameIn", "[Name] IN {0}",
                            new[] {MetadataGenerator.GetParameter("BookNames", null)})
                    },
                    {
                        "NameLike",
                        MetadataGenerator.GetDefaultOperation<string>("NameLike", "[Name] LIKE {0}", "BookName")
                    },
                    {
                        "AuthorIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "[AuthorID] = {0}", "AuthorID")
                    },
                    {
                        "AuthorIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "[AuthorID] <> {0}", "AuthorID")
                    },
                    {
                        "AuthorIDIn",
                        MetadataGenerator.GetOperation("AuthorIDIn", "[AuthorID] IN {0}",
                            new[] {MetadataGenerator.GetParameter("AuthorIDs", null)})
                    },
                    {
                        "PageCountEquals",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountEquals", "[PageCount] = {0}", "PageCount")
                    },
                    {
                        "PageCountNotEquals",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountNotEquals", "[PageCount] <> {0}", "PageCount")
                    },
                    {
                        "PageCountGreater",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountGreater", "[PageCount] > {0}", "PageCount")
                    },
                    {
                        "PageCountLess",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountLess", "[PageCount] < {0}", "PageCount")
                    },
                    {
                        "PageCountNull",
                        MetadataGenerator.GetOperation("PageCountNull", "[PageCount] IS NULL", new QueryParameterMetadata[]{})
                    },
                    {
                        "PageCountNotNull",
                        MetadataGenerator.GetOperation("PageCountNotNull", "[PageCount] IS NOT NULL", new QueryParameterMetadata[]{})
                    },
                    {
                        "PageCountBetween",
                        MetadataGenerator.GetOperation("PageCountBetween", "[PageCount] BETWEEN {0} AND {1}",
                            new[]
                            {
                                MetadataGenerator.GetParameter<int>("PageCountStart"),
                                MetadataGenerator.GetParameter<int>("PageCountEnd")
                            })
                    },
                    {
                        "HasGenre",
                        MetadataGenerator.GetOperation("HasGenre",
                            "EXISTS (SELECT 1 FROM [BookGenres] bg JOIN [Genres] g ON bg.[GenreID] = g.[GenreID] WHERE bg.[BookID] = b.[BookID] AND g.[Name] = {0})",
                            new[] {MetadataGenerator.GetParameter<string>("GenreName")})
                    },
                    {
                        "TestIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("TestIDEquals", "[TestID] = {0}", "TestID")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookGetMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {"BookID", MetadataGenerator.GetDefaultOrderOperation("BookID", "[BookID] {0}")},
                    {"Name", MetadataGenerator.GetDefaultOrderOperation("Name", "[Name] {0}")},
                    {"AuthorID", MetadataGenerator.GetDefaultOrderOperation("AuthorID", "[AuthorID] {0}")},
                    {"PageCount", MetadataGenerator.GetDefaultOrderOperation("PageCount", "[PageCount] {0}")},
                };

            public static readonly IDictionary<string, MergeOperationMetadata> DefaultBookUpdateMetadata =
                new Dictionary<string, MergeOperationMetadata>
                {
                    {"BookID", MetadataGenerator.GetDefaultMergeOperation<Guid>("BookID", "[BookID] = {0}")},
                    {"Name", MetadataGenerator.GetDefaultMergeOperation<string>("Name", "[Name] = {0}")},
                    {"AuthorID", MetadataGenerator.GetDefaultMergeOperation<Guid>("AuthorID", "[AuthorID] = {0}")},
                    {"PageCount", MetadataGenerator.GetDefaultMergeOperation<int>("PageCount", "[PageCount] = {0}")}
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultGenreFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "GenreIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDEquals", "[GenreID] = {0}", "GenreID")
                    },
                    {
                        "GenreIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDNotEquals", "[GenreID] <> {0}", "GenreID")
                    },
                    {
                        "GenreIDIn",
                        MetadataGenerator.GetOperation("GenreIDIn", "[GenreID] IN {0}", new []
                        {
                            MetadataGenerator.GetParameter("GenreIDs", null)
                        })
                    },
                    {
                        "NameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameEquals", "[Name] = {0}", "GenreName")
                    },
                    {
                        "NameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameNotEquals", "[Name] <> {0}", "GenreName")
                    },
                    {
                        "NameIn",
                        MetadataGenerator.GetOperation("NameIn", "[Name] IN {0}",
                            new[] {MetadataGenerator.GetParameter("GenreNames", null)})
                    },
                    {
                        "NameLike",
                        MetadataGenerator.GetDefaultOperation<string>("NameLike", "[Name] LIKE {0}", "GenreName")
                    },
                    {
                        "TestIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("TestIDEquals", "[TestID] = {0}", "TestID")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultAuthorFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "AuthorIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "[AuthorID] = {0}", "AuthorID")
                    },
                    {
                        "AuthorIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "[AuthorID] <> {0}", "AuthorID")
                    },
                    {
                        "AuthorIDIn",
                        MetadataGenerator.GetOperation("AuthorIDIn", "[AuthorID] IN {0}", new []
                        {
                            MetadataGenerator.GetParameter("AuthorIDs", null)
                        })
                    },
                    {
                        "FirstNameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameEquals", "[FirstName] = {0}", "FirstName")
                    },
                    {
                        "FirstNameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameNotEquals", "[FirstName] <> {0}", "FirstName")
                    },
                    {
                        "FirstNameIn",
                        MetadataGenerator.GetOperation("FirstNameIn", "[FirstName] IN {0}",
                            new[] {MetadataGenerator.GetParameter("FirstNames", null)})
                    },
                    {
                        "FirstNameLike",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameLike", "[FirstName] LIKE {0}", "FirstName")
                    },
                    {
                        "LastNameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameEquals", "[LastName] = {0}", "LastName")
                    },
                    {
                        "LastNameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameNotEquals", "[LastName] <> {0}", "LastName")
                    },
                    {
                        "LastNameIn",
                        MetadataGenerator.GetOperation("LastNameIn", "[LastName] IN {0}",
                            new[] {MetadataGenerator.GetParameter("LastNames", null)})
                    },
                    {
                        "LastNameLike",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameLike", "[LastName] LIKE {0}", "LastName")
                    },
                    {
                        "TestIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("TestIDEquals", "[TestID] = {0}", "TestID")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookGenreMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "BookIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "[BookID] = {0}", "BookID")
                    },
                    {
                        "BookIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "[BookID] <> {0}", "BookID")
                    },
                    {
                        "BookIDIn",
                        MetadataGenerator.GetOperation("BookIDIn", "BookID IN {0}",
                            new[] {MetadataGenerator.GetParameter("BookIDs", null)})
                    },
                    {
                        "GenreIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDEquals", "[GenreID] = {0}", "GenreID")
                    },
                    {
                        "GenreIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDNotEquals", "[GenreID] <> {0}", "GenreID")
                    },
                    {
                        "GenreIDIn",
                        MetadataGenerator.GetOperation("GenreIDIn", "[GenreID] IN {0}", new []
                        {
                            MetadataGenerator.GetParameter("GenreIDs", null)
                        })
                    },
                    {
                        "TestIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("TestIDEquals", "[TestID] = {0}", "TestID")
                    }
                };
        }

        public static class Postgres
        {
            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "BookIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "\"BookID\" = {0}", "BookID")
                    },
                    {
                        "BookIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "\"BookID\" <> {0}", "BookID")
                    },
                    {
                        "BookIDIn",
                        MetadataGenerator.GetOperation("BookIDIn", "\"BookID\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("BookIDs", null)})
                    },
                    {
                        "NameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameEquals", "\"Name\" = {0}", "BookName")
                    },
                    {
                        "NameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameNotEquals", "\"Name\" <> {0}", "BookName")
                    },
                    {
                        "NameIn",
                        MetadataGenerator.GetOperation("NameIn", "\"Name\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("BookNames", null)})
                    },
                    {
                        "NameLike",
                        MetadataGenerator.GetDefaultOperation<string>("NameLike", "\"Name\" ILIKE {0}", "BookName")
                    },
                    {
                        "AuthorIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "\"AuthorID\" = {0}", "AuthorID")
                    },
                    {
                        "AuthorIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "\"AuthorID\" <> {0}", "AuthorID")
                    },
                    {
                        "AuthorIDIn",
                        MetadataGenerator.GetOperation("AuthorIDIn", "\"AuthorID\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("AuthorIDs", null)})
                    },
                    {
                        "PageCountEquals",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountEquals", "\"PageCount\" = {0}", "PageCount")
                    },
                    {
                        "PageCountNotEquals",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountNotEquals", "\"PageCount\" <> {0}", "PageCount")
                    },
                    {
                        "PageCountGreater",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountGreater", "\"PageCount\" > {0}", "PageCount")
                    },
                    {
                        "PageCountLess",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountLess", "\"PageCount\" < {0}", "PageCount")
                    },
                    {
                        "PageCountNull",
                        MetadataGenerator.GetOperation("PageCountNull", "\"PageCount\" IS NULL", new QueryParameterMetadata[]{})
                    },
                    {
                        "PageCountNotNull",
                        MetadataGenerator.GetOperation("PageCountNotNull", "\"PageCount\" IS NOT NULL", new QueryParameterMetadata[]{})
                    },
                    {
                        "PageCountBetween",
                        MetadataGenerator.GetOperation("PageCountBetween", "\"PageCount\" BETWEEN {0} AND {1}",
                            new[]
                            {
                                MetadataGenerator.GetParameter<int>("PageCountStart"),
                                MetadataGenerator.GetParameter<int>("PageCountEnd")
                            })
                    },
                    {
                        "HasGenre",
                        MetadataGenerator.GetOperation("HasGenre",
                            "EXISTS (SELECT 1 FROM \"BookGenres\" bg JOIN \"Genres\" g ON bg.\"GenreID\" = g.\"GenreID\" WHERE bg.\"BookID\" = b.\"BookID\" AND g.\"Name\" = {0})",
                            new[] {MetadataGenerator.GetParameter<string>("GenreName")})
                    },
                    {
                        "TestIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("TestIDEquals", "\"TestID\" = {0}", "TestID")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookGetMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {"BookID", MetadataGenerator.GetDefaultOrderOperation("BookID", "\"BookID\" {0}")},
                    {"Name", MetadataGenerator.GetDefaultOrderOperation("Name", "\"Name\" {0}")},
                    {"AuthorID", MetadataGenerator.GetDefaultOrderOperation("AuthorID", "\"AuthorID\" {0}")},
                    {"PageCount", MetadataGenerator.GetDefaultOrderOperation("PageCount", "\"PageCount\" {0}")},
                };

            public static readonly IDictionary<string, MergeOperationMetadata> DefaultBookUpdateMetadata =
                new Dictionary<string, MergeOperationMetadata>
                {
                    {"BookID", MetadataGenerator.GetDefaultMergeOperation<Guid>("BookID", "\"BookID\" = {0}")},
                    {"Name", MetadataGenerator.GetDefaultMergeOperation<string>("Name", "\"Name\" = {0}")},
                    {"AuthorID", MetadataGenerator.GetDefaultMergeOperation<Guid>("AuthorID", "\"AuthorID\" = {0}")},
                    {"PageCount", MetadataGenerator.GetDefaultMergeOperation<int>("PageCount", "\"PageCount\" = {0}")}
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultGenreFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "GenreIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDEquals", "\"GenreID\" = {0}", "GenreID")
                    },
                    {
                        "GenreIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDNotEquals", "\"GenreID\" <> {0}", "GenreID")
                    },
                    {
                        "GenreIDIn",
                        MetadataGenerator.GetOperation("GenreIDIn", "\"GenreID\" = ANY({0})", new []
                        {
                            MetadataGenerator.GetParameter("GenreIDs", null)
                        })
                    },
                    {
                        "NameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameEquals", "\"Name\" = {0}", "GenreName")
                    },
                    {
                        "NameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameNotEquals", "\"Name\" <> {0}", "GenreName")
                    },
                    {
                        "NameIn",
                        MetadataGenerator.GetOperation("NameIn", "\"Name\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("GenreNames", null)})
                    },
                    {
                        "NameLike",
                        MetadataGenerator.GetDefaultOperation<string>("NameLike", "\"Name\" ILIKE {0}", "GenreName")
                    },
                    {
                        "TestIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("TestIDEquals", "\"TestID\" = {0}", "TestID")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultAuthorFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "AuthorIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "\"AuthorID\" = {0}", "AuthorID")
                    },
                    {
                        "AuthorIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "\"AuthorID\" <> {0}", "AuthorID")
                    },
                    {
                        "AuthorIDIn",
                        MetadataGenerator.GetOperation("AuthorIDIn", "\"AuthorID\" = ANY({0})", new []
                        {
                            MetadataGenerator.GetParameter("AuthorIDs", null)
                        })
                    },
                    {
                        "FirstNameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameEquals", "\"FirstName\" = {0}", "FirstName")
                    },
                    {
                        "FirstNameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameNotEquals", "\"FirstName\" <> {0}", "FirstName")
                    },
                    {
                        "FirstNameIn",
                        MetadataGenerator.GetOperation("FirstNameIn", "\"FirstName\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("FirstNames", null)})
                    },
                    {
                        "FirstNameLike",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameLike", "\"FirstName\" ILIKE {0}", "FirstName")
                    },
                    {
                        "LastNameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameEquals", "\"LastName\" = {0}", "LastName")
                    },
                    {
                        "LastNameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameNotEquals", "\"LastName\" <> {0}", "LastName")
                    },
                    {
                        "LastNameIn",
                        MetadataGenerator.GetOperation("LastNameIn", "\"LastName\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("LastNames", null)})
                    },
                    {
                        "LastNameLike",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameLike", "\"LastName\" ILIKE {0}", "LastName")
                    },
                    {
                        "TestIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("TestIDEquals", "\"TestID\" = {0}", "TestID")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookGenreMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "BookIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "\"BookID\" = {0}", "BookID")
                    },
                    {
                        "BookIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "\"BookID\" <> {0}", "BookID")
                    },
                    {
                        "BookIDIn",
                        MetadataGenerator.GetOperation("BookIDIn", "BookID = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("BookIDs", null)})
                    },
                    {
                        "GenreIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDEquals", "\"GenreID\" = {0}", "GenreID")
                    },
                    {
                        "GenreIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDNotEquals", "\"GenreID\" <> {0}", "GenreID")
                    },
                    {
                        "GenreIDIn",
                        MetadataGenerator.GetOperation("GenreIDIn", "\"GenreID\" = ANY({0})", new []
                        {
                            MetadataGenerator.GetParameter("GenreIDs", null)
                        })
                    },
                    {
                        "TestIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("TestIDEquals", "\"TestID\" = {0}", "TestID")
                    }
                };
        }

        public static class TestData
        {
            public class TestDataResults
            {
                public IEnumerable<Author> Authors { get; set; }
                public IEnumerable<BookGenre> BookGenres { get; set; }
                public IEnumerable<Book> Books { get; set; }
                public IEnumerable<Genre> Genres { get; set; }
            }

            public static TestDataResults GetTestData()
            {
                var authorLookup = new Dictionary<string, Author>();
                var genreLookup = new Dictionary<string, Genre>();
                var books = new List<Book>();
                var bookGenres = new List<BookGenre>();

                var author = GetAuthor(authorLookup, "Brandon", "Sanderson");
                var genres = GetGenres(genreLookup, new[] {"High Fantasy", "Fantasy Fiction"});
                var (nextBook, nextBookGenres) = GetBook("Rhythm of War", author, genres, 1230);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Brandon", "Sanderson");
                genres = GetGenres(genreLookup, new[] {"Science Fiction"});
                (nextBook, nextBookGenres) = GetBook("Skyward", author, genres, 513);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Brandon", "Sanderson");
                genres = GetGenres(genreLookup, new[] {"Science Fiction", "Fantasy Fiction", "Young Adult Fiction"});
                (nextBook, nextBookGenres) = GetBook("Steelheart", author, genres, 386);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Arthur Conan", "Doyle");
                genres = GetGenres(genreLookup,
                    new[] {"Short Story", "Mystery", "Novel", "Detective Novel", "Noir Fiction", "Crime Fiction"});
                (nextBook, nextBookGenres) = GetBook("The Adventures of Sherlock Holmes", author, genres, 307);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Arthur Conan", "Doyle");
                genres = GetGenres(genreLookup, new[] {"Mystery", "Novel", "Detective Novel", "Crime Fiction"});
                (nextBook, nextBookGenres) = GetBook("The Hound of the Baskervilles", author, genres, 226);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Arthur Conan", "Doyle");
                genres = GetGenres(genreLookup,
                    new[]
                    {
                        "Novel", "Science Fiction", "Lost World", "Adventure Fiction", "Fantasy Fiction",
                        "Scientific Romance", "Fantastic"
                    });
                (nextBook, nextBookGenres) = GetBook("The Lost World", author, genres, 240);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Arthur Conan", "Doyle");
                genres = GetGenres(genreLookup, new[] {"Mystery", "Novel", "Detective Novel", "Locked-Room Mystery"});
                (nextBook, nextBookGenres) = GetBook("The Valley of Fear", author, genres, 164);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Dr.", "Seuss");
                genres = GetGenres(genreLookup, new[] {"Children's Literature", "Picture Book", "Fiction"});
                (nextBook, nextBookGenres) = GetBook("The Cat in the Hat", author, genres, 61);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Dr.", "Seuss");
                genres = GetGenres(genreLookup, new[] {"Children's Literature", "Fiction"});
                (nextBook, nextBookGenres) = GetBook("Fox in Socks", author, genres, 72);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                author = GetAuthor(authorLookup, "Titus", "Livius");
                genres = GetGenres(genreLookup, new[] {"Non-Fiction", "History"});
                (nextBook, nextBookGenres) = GetBook("Ab Urbe Condita", author, genres);
                books.Add(nextBook);
                bookGenres.AddRange(nextBookGenres);

                return new TestDataResults
                {
                    Authors = authorLookup.Values,
                    BookGenres = bookGenres,
                    Books = books,
                    Genres = genreLookup.Values
                };
            }

            private static (Book, IEnumerable<BookGenre>) GetBook(string name, Author author, IEnumerable<Genre> genres,
                int? pageCount = null)
            {
                var result = new Book
                {
                    Name = name,
                    AuthorID = author.AuthorID,
                    PageCount = pageCount
                };

                var genreMappings = genres.Select(g => new BookGenre
                {
                    BookID = result.BookID,
                    GenreID = g.GenreID
                });

                return (result, genreMappings);
            }

            private static Author GetAuthor(Dictionary<string, Author> currentAuthors, string firstName,
                string lastName)
            {
                var authorKey = $"{firstName} {lastName}";
                if (currentAuthors.ContainsKey(authorKey))
                {
                    return currentAuthors[authorKey];
                }

                var result = new Author
                {
                    FirstName = firstName,
                    LastName = lastName
                };

                currentAuthors[authorKey] = result;

                return result;
            }

            private static IEnumerable<Genre> GetGenres(Dictionary<string, Genre> currentGenres, string[] names)
            {
                var results = new List<Genre>();

                foreach (var name in names)
                {
                    if (currentGenres.ContainsKey(name))
                    {
                        results.Add(currentGenres[name]);
                        continue;
                    }

                    var genre = new Genre
                    {
                        Name = name
                    };

                    currentGenres[name] = genre;

                    results.Add(genre);
                }

                return results;
            }
        }
    }
}
