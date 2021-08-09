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
                        "NameLike",
                        _metadataGenerator.GetDefaultOperation<string>("NameLike", "[Name] LIKE {0}")
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
                        "PageCountEquals",
                        _metadataGenerator.GetDefaultOperation<int>("PageCountEquals", "[PageCount] = {0}")
                    }
                };
        }
    }
}
