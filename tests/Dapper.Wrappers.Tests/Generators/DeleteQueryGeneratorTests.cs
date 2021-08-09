using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Formatters;
using Dapper.Wrappers.Generators;
using Xunit;

namespace Dapper.Wrappers.Tests.Generators
{
    public class DeleteQueryGeneratorTests : IClassFixture<DatabaseFixture>
    {
        private readonly IMetadataGenerator _metadataGenerator;
        private readonly DatabaseFixture _databaseFixture;

        public DeleteQueryGeneratorTests(DatabaseFixture databaseFixture, IMetadataGenerator metadataGenerator)
        {
            _metadataGenerator = metadataGenerator;
            _databaseFixture = databaseFixture;
        }

        private TestDeleteQueryGenerator GetTestInstance(SupportedDatabases dbType, string deleteQueryString,
            IDictionary<string, QueryOperationMetadata> filterOperationMetadata)
        {
            var formatter = _databaseFixture.GetFormatter(dbType);

            return new TestDeleteQueryGenerator(formatter, deleteQueryString, filterOperationMetadata);
        }

        private IQueryContext GetQueryContext(SupportedDatabases dbType)
        {
            var connection = _databaseFixture.GetConnection(dbType);

            return new QueryContext(connection);
        }

        //public Task AddDeleteQuery_WithInputs_ShouldDeleteRowsFromDatabase(SupportedDatabases dbType)
        //{
        //    // Arrange
        //    var connection = _databaseFixture.GetConnection(dbType);
        //    var startIdentifier = dbType == SupportedDatabases.SqlServer ? "[" : "\"";
        //    var endIdentifier = dbType == SupportedDatabases.SqlServer ? "]" : "\"";

        //    var metadata = new Dictionary<string, QueryOperationMetadata>
        //    {
        //        {
        //            "NameLike",
        //            _metadataGenerator.GetDefaultOperation<string>("NameLike",
        //                dbType == SupportedDatabases.SqlServer ? "[Name] LIKE {0}" : "\"Name\" ILIKE {0}")
        //        },
        //        {
        //            "NameEquals",
        //            _metadataGenerator.GetDefaultOperation<string>("NameEquals", dbType == SupportedDatabases.SqlServer ? "[Name] = {0}" : "\"Name\" = {0}")
        //        },
        //        {
        //            "PageCountEquals",
        //            _metadataGenerator.GetDefaultOperation<int>("PageCountEquals", dbType == SupportedDatabases.SqlServer ? "[PageCount] = {0}" : "[PageCount] = {0}")
        //        }
        //    };

        //    var generator = GetTestInstance(dbType);
        //    var context = GetQueryContext(dbType);

        //    // Act
        //    generator.AddDeleteQuery(context);

        //    // Assert
        //}
    }

    public class TestDeleteQueryGenerator : DeleteQueryGenerator
    {
        public TestDeleteQueryGenerator(IQueryFormatter queryFormatter, string deleteQueryString,
            IDictionary<string, QueryOperationMetadata> filterOperationMetadata) : base(queryFormatter)
        {
            FilterOperationMetadata = filterOperationMetadata;
            DeleteQueryString = deleteQueryString;
        }

        protected override IDictionary<string, QueryOperationMetadata> FilterOperationMetadata { get; }
        protected override string DeleteQueryString { get; }
    }
}
