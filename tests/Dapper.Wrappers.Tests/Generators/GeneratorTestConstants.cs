using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Wrappers.Generators;

namespace Dapper.Wrappers.Tests.Generators
{
    public static class GeneratorTestConstants
    {
        public static class SqlServer
        {
            private static readonly IMetadataGenerator _metadataGenerator = new MetadataGenerator();

            public static readonly IDictionary<string, QueryOperationMetadata> DefaultBookMetadata =
                new Dictionary<string, QueryOperationMetadata>
                {
                    {
                        "BookIDEquals",
                        _metadataGenerator.GetDefaultOperation<Guid>("BookIDEquals", "[BookID] = {0}")
                    },
                    {
                        "BookIDNotEquals",
                        _metadataGenerator.GetDefaultOperation<Guid>("BookIDNotEquals", "[BookID] <> {0}")
                    },
                    {
                        "BookIDIn",
                        _metadataGenerator.GetOperation("BookIDIn", "BookID IN {0}",
                            new[] {_metadataGenerator.GetParameter("BookID", null)})
                    },
                    {
                        "NameEquals",
                        _metadataGenerator.GetDefaultOperation<string>("NameEquals", "[Name] = {0}")
                    },
                    {
                        "NameNotEquals",
                        _metadataGenerator.GetDefaultOperation<string>("NameNotEquals", "[Name] <> {0}")
                    },
                    {
                        "NameIn",
                        _metadataGenerator.GetOperation("NameIn", "[Name] IN {0}",
                            new[] {_metadataGenerator.GetParameter("Name", null)})
                    },
                    {
                        "NameLike",
                        _metadataGenerator.GetDefaultOperation<string>("NameLike", "[Name] LIKE {0}")
                    },
                    {
                        "AuthorIDEquals",
                        _metadataGenerator.GetDefaultOperation<Guid>("AuthorIDEquals", "[AuthorID] = {0}")
                    },
                    {
                        "AuthorIDNotEquals",
                        _metadataGenerator.GetDefaultOperation<Guid>("AuthorIDNotEquals", "[AuthorID] <> {0}")
                    },
                    {
                        "AuthorIDIn",
                        _metadataGenerator.GetOperation("AuthorIDIn", "[AuthorID] IN {0}",
                            new[] {_metadataGenerator.GetParameter("AuthorID", null)})
                    },
                    {
                        "PageCountEquals",
                        _metadataGenerator.GetDefaultOperation<int>("PageCountEquals", "[PageCount] = {0}")
                    },
                    {
                        "PageCountNotEquals",
                        _metadataGenerator.GetDefaultOperation<int>("PageCountNotEquals", "[PageCount] <> {0}")
                    },
                    {
                        "PageCountGreater",
                        _metadataGenerator.GetDefaultOperation<int>("PageCountGreater", "[PageCount] > {0}")
                    },
                    {
                        "PageCountLess",
                        _metadataGenerator.GetDefaultOperation<int>("PageCountLess", "[PageCount] < {0}")
                    },
                    {
                        "PageCountBetween",
                        _metadataGenerator.GetOperation("PageCountBetween", "[PageCount] BETWEEN {0} AND {1}",
                            new[]
                            {
                                _metadataGenerator.GetParameter<int>("Start"),
                                _metadataGenerator.GetParameter<int>("End")
                            })
                    }
                };
        }
    }
}
