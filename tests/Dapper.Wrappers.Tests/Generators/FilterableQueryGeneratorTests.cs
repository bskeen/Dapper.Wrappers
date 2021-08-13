// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Formatters;
using Dapper.Wrappers.Generators;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dapper.Wrappers.Tests.Generators
{
    public class FilterableQueryGeneratorTests
    {
        private static TestFilterableQueryGenerator GetDefaultTestInstance(SupportedDatabases dbType,
            IDictionary<string, QueryOperationMetadata> metadata)
        {
            IQueryFormatter formatter = dbType == SupportedDatabases.SqlServer
                ? (IQueryFormatter)(new SqlServerQueryFormatter())
                : new PostgresQueryFormatter();
            
            return new TestFilterableQueryGenerator(formatter, metadata);
        }

        private static Mock<IQueryContext> GetMockQueryContext()
        {
            Random generator = new Random();
            var result = new Mock<IQueryContext>();
            result.Setup(context => context.AddVariable(It.IsAny<string>(), It.IsAny<object>(),
                It.Is<DbType?>((obj, type) => type == typeof(DbType?)), It.IsAny<bool>())).Returns(
                (string name, object value, DbType? type, bool isUnique) => $"{name}{generator.Next(1000)}");

            return result;
        }

        private readonly IMetadataGenerator _metadataGenerator;

        public FilterableQueryGeneratorTests(IMetadataGenerator metadataGenerator)
        {
            _metadataGenerator = metadataGenerator;
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer, true)]
        [InlineData(SupportedDatabases.SqlServer, false)]
        [InlineData(SupportedDatabases.PostgreSQL, true)]
        [InlineData(SupportedDatabases.PostgreSQL, false)]
        public void FormatFilterOperations_WithEmptyInputs_ShouldReturnAnEmptyString(SupportedDatabases dbType,
            bool isEmptyInput)
        {
            // Arrange
            var metadata = new Dictionary<string, QueryOperationMetadata>
            {
                {"Column1", _metadataGenerator.GetDefaultOperation<int>("Column1", "Column1 = {0}", "Column1")},
                {"Column2", _metadataGenerator.GetDefaultOperation<Guid>("Column2", "Column2 = {0}", "Column2")}
            };

            var generator = GetDefaultTestInstance(dbType, metadata);
            var context = GetMockQueryContext();

            var operations = isEmptyInput ? new QueryOperation[]{} : null;

            // Act
            var result = generator.TestFormatFilterOperations(context.Object, operations);

            // Assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void FormatFilterOperations_WithInputs_ShouldReturnExpectedString(SupportedDatabases dbType)
        {
            // Arrange
            var metadata = new Dictionary<string, QueryOperationMetadata>
            {
                {"Column1", _metadataGenerator.GetDefaultOperation<int>("Column1", "Column1 = {0}", "Column1")},
                {
                    "Subquery1", _metadataGenerator.GetOperation("Subquery1",
                        "EXISTS (SELECT Column2 FROM Values WHERE TestValue = {0})", new[]
                        {
                            _metadataGenerator.GetParameter<int>("Value1")
                        })
                }
            };

            var generator = GetDefaultTestInstance(dbType, metadata);
            var context = GetMockQueryContext();

            var operations = new[]
            {
                new QueryOperation
                {
                    Name = "Column1",
                    Parameters = new Dictionary<string, object>
                    {
                        {"Column1", 1}
                    }
                },
                new QueryOperation
                {
                    Name = "Subquery1",
                    Parameters = new Dictionary<string, object>
                    {
                        {"Value1", 2}
                    }
                }
            };

            // Act
            var result = generator.TestFormatFilterOperations(context.Object, operations);

            // Assert
            result.Should().MatchRegex("WHERE Column1 = @Column1[0-9]* AND EXISTS \\(SELECT Column2 FROM Values WHERE TestValue = @Value1[0-9]*\\)");
        }
    }

    public class TestFilterableQueryGenerator : FilterableQueryGenerator
    {
        public TestFilterableQueryGenerator(IQueryFormatter queryFormatter,
            IDictionary<string, QueryOperationMetadata> filterOperationMetadata) : base(queryFormatter)
        {
            FilterOperationMetadata = filterOperationMetadata;
        }

        protected override IDictionary<string, QueryOperationMetadata> FilterOperationMetadata { get; }

        public string TestFormatFilterOperations(IQueryContext queryContext,
            IEnumerable<QueryOperation> filterOperations = null) =>
            FormatFilterOperations(queryContext, filterOperations);
    }
}
