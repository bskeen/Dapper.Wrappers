using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Wrappers.Generators;

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
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "[BookID] = {0}")
                    },
                    {
                        "BookIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "[BookID] <> {0}")
                    },
                    {
                        "BookIDIn",
                        MetadataGenerator.GetOperation("BookIDIn", "[BookID] IN {0}",
                            new[] {MetadataGenerator.GetParameter("BookID", null)})
                    },
                    {
                        "NameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameEquals", "[Name] = {0}")
                    },
                    {
                        "NameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameNotEquals", "[Name] <> {0}")
                    },
                    {
                        "NameIn",
                        MetadataGenerator.GetOperation("NameIn", "[Name] IN {0}",
                            new[] {MetadataGenerator.GetParameter("Name", null)})
                    },
                    {
                        "NameLike",
                        MetadataGenerator.GetDefaultOperation<string>("NameLike", "[Name] LIKE {0}")
                    },
                    {
                        "AuthorIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "[AuthorID] = {0}")
                    },
                    {
                        "AuthorIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "[AuthorID] <> {0}")
                    },
                    {
                        "AuthorIDIn",
                        MetadataGenerator.GetOperation("AuthorIDIn", "[AuthorID] IN {0}",
                            new[] {MetadataGenerator.GetParameter("AuthorID", null)})
                    },
                    {
                        "PageCountEquals",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountEquals", "[PageCount] = {0}")
                    },
                    {
                        "PageCountNotEquals",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountNotEquals", "[PageCount] <> {0}")
                    },
                    {
                        "PageCountGreater",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountGreater", "[PageCount] > {0}")
                    },
                    {
                        "PageCountLess",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountLess", "[PageCount] < {0}")
                    },
                    {
                        "PageCountBetween",
                        MetadataGenerator.GetOperation("PageCountBetween", "[PageCount] BETWEEN {0} AND {1}",
                            new[]
                            {
                                MetadataGenerator.GetParameter<int>("Start"),
                                MetadataGenerator.GetParameter<int>("End")
                            })
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultGenreFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "GenreIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDEquals", "[GenreID] = {0}")
                    },
                    {
                        "GenreIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDNotEquals", "[GenreID] <> {0}")
                    },
                    {
                        "GenreIDIn",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDIn", "[GenreID] IN {0}")
                    },
                    {
                        "NameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameEquals", "[Name] = {0}")
                    },
                    {
                        "NameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameNotEquals", "[Name] <> {0}")
                    },
                    {
                        "NameIn",
                        MetadataGenerator.GetOperation("NameIn", "[Name] IN {0}",
                            new[] {MetadataGenerator.GetParameter("Name", null)})
                    },
                    {
                        "NameLike",
                        MetadataGenerator.GetDefaultOperation<string>("NameLike", "[Name] LIKE {0}")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultAuthorFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "AuthorIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "[AuthorID] = {0}")
                    },
                    {
                        "AuthorIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "[AuthorID] <> {0}")
                    },
                    {
                        "AuthorIDIn",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDIn", "[AuthorID] IN {0}")
                    },
                    {
                        "FirstNameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameEquals", "[FirstName] = {0}")
                    },
                    {
                        "FirstNameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameNotEquals", "[FirstName] <> {0}")
                    },
                    {
                        "FirstNameIn",
                        MetadataGenerator.GetOperation("FirstNameIn", "[FirstName] IN {0}",
                            new[] {MetadataGenerator.GetParameter("FirstName", null)})
                    },
                    {
                        "FirstNameLike",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameLike", "[FirstName] LIKE {0}")
                    },
                    {
                        "LastNameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameEquals", "[LastName] = {0}")
                    },
                    {
                        "LastNameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameNotEquals", "[LastName] <> {0}")
                    },
                    {
                        "LastNameIn",
                        MetadataGenerator.GetOperation("LastNameIn", "[LastName] IN {0}",
                            new[] {MetadataGenerator.GetParameter("LastName", null)})
                    },
                    {
                        "LastNameLike",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameLike", "[LastName] LIKE {0}")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookGenreMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "BookIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "[BookID] = {0}")
                    },
                    {
                        "BookIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "[BookID] <> {0}")
                    },
                    {
                        "BookIDIn",
                        MetadataGenerator.GetOperation("BookIDIn", "BookID IN {0}",
                            new[] {MetadataGenerator.GetParameter("BookID", null)})
                    },
                    {
                        "GenreIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDEquals", "[GenreID] = {0}")
                    },
                    {
                        "GenreIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDNotEquals", "[GenreID] <> {0}")
                    },
                    {
                        "GenreIDIn",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDIn", "[GenreID] IN {0}")
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
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "\"BookID\" = {0}")
                    },
                    {
                        "BookIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "\"BookID\" <> {0}")
                    },
                    {
                        "BookIDIn",
                        MetadataGenerator.GetOperation("BookIDIn", "\"BookID\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("BookID", null)})
                    },
                    {
                        "NameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameEquals", "\"Name\" = {0}")
                    },
                    {
                        "NameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameNotEquals", "\"Name\" <> {0}")
                    },
                    {
                        "NameIn",
                        MetadataGenerator.GetOperation("NameIn", "\"Name\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("Name", null)})
                    },
                    {
                        "NameLike",
                        MetadataGenerator.GetDefaultOperation<string>("NameLike", "\"Name\" ILIKE {0}")
                    },
                    {
                        "AuthorIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "\"AuthorID\" = {0}")
                    },
                    {
                        "AuthorIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "\"AuthorID\" <> {0}")
                    },
                    {
                        "AuthorIDIn",
                        MetadataGenerator.GetOperation("AuthorIDIn", "\"AuthorID\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("AuthorID", null)})
                    },
                    {
                        "PageCountEquals",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountEquals", "\"PageCount\" = {0}")
                    },
                    {
                        "PageCountNotEquals",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountNotEquals", "\"PageCount\" <> {0}")
                    },
                    {
                        "PageCountGreater",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountGreater", "\"PageCount\" > {0}")
                    },
                    {
                        "PageCountLess",
                        MetadataGenerator.GetDefaultOperation<int>("PageCountLess", "\"PageCount\" < {0}")
                    },
                    {
                        "PageCountBetween",
                        MetadataGenerator.GetOperation("PageCountBetween", "\"PageCount\" BETWEEN {0} AND {1}",
                            new[]
                            {
                                MetadataGenerator.GetParameter<int>("Start"),
                                MetadataGenerator.GetParameter<int>("End")
                            })
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultGenreFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "GenreIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDEquals", "\"GenreID\" = {0}")
                    },
                    {
                        "GenreIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDNotEquals", "\"GenreID\" <> {0}")
                    },
                    {
                        "GenreIDIn",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDIn", "\"GenreID\" = ANY({0})")
                    },
                    {
                        "NameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameEquals", "\"Name\" = {0}")
                    },
                    {
                        "NameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("NameNotEquals", "\"Name\" <> {0}")
                    },
                    {
                        "NameIn",
                        MetadataGenerator.GetOperation("NameIn", "\"Name\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("Name", null)})
                    },
                    {
                        "NameLike",
                        MetadataGenerator.GetDefaultOperation<string>("NameLike", "\"Name\" ILIKE {0}")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultAuthorFilterMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "AuthorIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "\"AuthorID\" = {0}")
                    },
                    {
                        "AuthorIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "\"AuthorID\" <> {0}")
                    },
                    {
                        "AuthorIDIn",
                        MetadataGenerator.GetDefaultOperation<Guid>("AuthorIDIn", "\"AuthorID\" = ANY({0})")
                    },
                    {
                        "FirstNameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameEquals", "\"FirstName\" = {0}")
                    },
                    {
                        "FirstNameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameNotEquals", "\"FirstName\" <> {0}")
                    },
                    {
                        "FirstNameIn",
                        MetadataGenerator.GetOperation("FirstNameIn", "\"FirstName\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("FirstName", null)})
                    },
                    {
                        "FirstNameLike",
                        MetadataGenerator.GetDefaultOperation<string>("FirstNameLike", "\"FirstName\" ILIKE {0}")
                    },
                    {
                        "LastNameEquals",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameEquals", "\"LastName\" = {0}")
                    },
                    {
                        "LastNameNotEquals",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameNotEquals", "\"LastName\" <> {0}")
                    },
                    {
                        "LastNameIn",
                        MetadataGenerator.GetOperation("LastNameIn", "\"LastName\" = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("LastName", null)})
                    },
                    {
                        "LastNameLike",
                        MetadataGenerator.GetDefaultOperation<string>("LastNameLike", "\"LastName\" ILIKE {0}")
                    }
                };

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookGenreMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "BookIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "\"BookID\" = {0}")
                    },
                    {
                        "BookIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "\"BookID\" <> {0}")
                    },
                    {
                        "BookIDIn",
                        MetadataGenerator.GetOperation("BookIDIn", "BookID = ANY({0})",
                            new[] {MetadataGenerator.GetParameter("BookID", null)})
                    },
                    {
                        "GenreIDEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDEquals", "\"GenreID\" = {0}")
                    },
                    {
                        "GenreIDNotEquals",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDNotEquals", "\"GenreID\" <> {0}")
                    },
                    {
                        "GenreIDIn",
                        MetadataGenerator.GetDefaultOperation<Guid>("GenreIDIn", "\"GenreID\" = ANY({0})")
                    }
                };
        }
    }
}
