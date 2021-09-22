// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Generators;
using Dapper.Wrappers.OperationFormatters;
using Dapper.Wrappers.QueryFormatters;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dapper.Wrappers.Tests.QueryFormatters
{
    public class FilterFormatterTests
    {
        private static FilterFormatter GetDefaultTestInstance(SupportedDatabases dbType)
        {
            IQueryOperationFormatter formatter = dbType == SupportedDatabases.SqlServer
                ? (IQueryOperationFormatter)(new SqlServerQueryOperationFormatter())
                : new PostgresQueryOperationFormatter();
            
            return new FilterFormatter(formatter);
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

        public FilterFormatterTests(IMetadataGenerator metadataGenerator)
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

            var formatter = GetDefaultTestInstance(dbType);
            var context = GetMockQueryContext();

            var operations = isEmptyInput ? new QueryOperation[]{} : null;

            // Act
            var result = formatter.FormatFilterOperations(context.Object, metadata, operations);

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

            var formatter = GetDefaultTestInstance(dbType);
            var context = GetMockQueryContext();

            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Column1", ("Column1", 1)),
                _metadataGenerator.GetQueryOperation("Subquery1", ("Value1", 2))
            };

            // Act
            var result = formatter.FormatFilterOperations(context.Object, metadata, operations);

            // Assert
            result.Should().MatchRegex("WHERE Column1 = @Column1[0-9]* AND EXISTS \\(SELECT Column2 FROM Values WHERE TestValue = @Value1[0-9]*\\)");
        }
    }
}
