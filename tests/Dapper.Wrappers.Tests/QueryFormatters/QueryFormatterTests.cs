// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.OperationFormatters;
using Dapper.Wrappers.QueryFormatters;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dapper.Wrappers.Tests.QueryFormatters
{
    public class QueryFormatterTests
    {
        private static TestQueryFormatter GetDefaultTestInstance()
        {
            return new TestQueryFormatter();
        }

        private static IQueryOperationFormatter GetQueryOperationFormatter(SupportedDatabases dbType) =>
            dbType == SupportedDatabases.SqlServer
                ? (IQueryOperationFormatter) (new SqlServerQueryOperationFormatter())
                : new PostgresQueryOperationFormatter();

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

        public QueryFormatterTests(IMetadataGenerator metadataGenerator)
        {
            _metadataGenerator = metadataGenerator;
        }

        [Theory]
        [InlineData(true, SupportedDatabases.SqlServer)]
        [InlineData(false, SupportedDatabases.SqlServer)]
        [InlineData(true, SupportedDatabases.PostgreSQL)]
        [InlineData(false, SupportedDatabases.PostgreSQL)]
        public void FormatOperations_WithNullOrEmptyInputs_ShouldReturnAnEmptyList(bool isNull, SupportedDatabases dbType)
        {
            // Arrange
            var queryFormatter = GetDefaultTestInstance();
            var mockContext = GetMockQueryContext();
            IEnumerable<QueryOperation> operations = isNull ? null : new List<QueryOperation>();

            // Act
            var results =
                queryFormatter.TestFormatOperations<QueryOperationMetadata>(mockContext.Object, operations, null, null,
                    null, null);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeEmpty();
            mockContext.Verify(
                context => context.AddVariable(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DbType>(),
                    It.IsAny<bool>()), Times.Never);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void FormatOperations_WithBogusOperators_ShouldReturnAnEmptyList(SupportedDatabases dbType)
        {
            // Arrange
            var queryFormatter = GetDefaultTestInstance();
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Bogus1", ("BogusValue1", null), ("BogusValue2", true)),
                _metadataGenerator.GetQueryOperation("Bogus2", ("EvenMoreBogusValue1", 1),
                    ("EvenMoreBogusValue2", Guid.NewGuid()))
            };

            var operationMetadata = new Dictionary<string, QueryOperationMetadata>
            {
                {"Real1", _metadataGenerator.GetDefaultOperation<string>("Real1", "this is not a query", "Real1")},
                {
                    "Real2",
                    _metadataGenerator.GetDefaultOperation<string>("Real2", "this is not a query either", "Real2")
                }
            };

            // Act
            var results =
                queryFormatter.TestFormatOperations(mockContext.Object, operations, operationMetadata, null, null,
                    null);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeEmpty();

            mockContext.Verify(
                context => context.AddVariable(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DbType>(),
                    It.IsAny<bool>()), Times.Never);
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void FormatOperations_WithOperationInputs_ShouldCallTheOperationActionForEachValidOperation(
            SupportedDatabases dbType)
        {
            // Arrange
            var queryFormatter = GetDefaultTestInstance();
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Bogus1", ("BogusValue1", null), ("BogusValue2", true)),
                _metadataGenerator.GetQueryOperation("Valid1", ("Valid1Value1", 1), ("Valid1Value2", Guid.NewGuid())),
                _metadataGenerator.GetQueryOperation("Valid2", ("Valid2Value1", "Test String"),
                    ("Valid2Value2", 233.4M))
            };

            var operationMetadata = new Dictionary<string, QueryOperationMetadata>
            {
                {
                    "Valid1", _metadataGenerator.GetOperation("Valid1", "Value1: {0}, Value2: {1}", new[]
                    {
                        _metadataGenerator.GetParameter<int>("Valid1Value1"),
                        _metadataGenerator.GetParameter<Guid>("Valid1Value2")
                    })
                },
                {
                    "Valid2", _metadataGenerator.GetOperation("Valid2", "Value1: {0}, Value2: {1}", new []
                    {
                        _metadataGenerator.GetParameter<string>("Valid2Value1"),
                        _metadataGenerator.GetParameter<decimal>("Valid2Value2")
                    })
                }
            };

            List<QueryOperationMetadata> collectedMetadata = new List<QueryOperationMetadata>();

            var operationFormatter = GetQueryOperationFormatter(dbType);

            // Act
            var results = queryFormatter.TestFormatOperations(mockContext.Object, operations, operationMetadata,
                queryFormatter.TestGetNonOrderingFormatOperation(operationFormatter.FormatFilterOperation), TestAction, null);

            // Assert
            foreach (var result in results)
            {
                result.Should().MatchRegex("Value1: @Valid[12]Value1[0-9]*, Value2: @Valid[12]Value2[0-9]*");
            }

            collectedMetadata.Should().HaveCount(2);
            var sortedMetadata = collectedMetadata.OrderBy(m => m.Name);
            var sortedInputMetadata = operationMetadata.Values.OrderBy(m => m.Name);

            sortedMetadata.Should().Equal(sortedInputMetadata,
                (m1, m2) => m1.Name == m2.Name && m1.BaseQueryString == m2.BaseQueryString);

            mockContext.Verify(
                context => context.AddVariable(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DbType>(),
                    It.IsAny<bool>()), Times.Exactly(4));

            void TestAction(QueryOperationMetadata metadata, int index, object state)
            {
                collectedMetadata.Add(metadata);
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void FormatOperations_WithMissingParametersWithDefault_ShouldStillReturnResults(
            SupportedDatabases dbType)
        {

            // Arrange
            var queryFormatter = GetDefaultTestInstance();
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Bogus1", ("BogusValue1", null), ("BogusValue2", true)),
                _metadataGenerator.GetQueryOperation("Valid1"),
                _metadataGenerator.GetQueryOperation("Valid2")
            };

            var operationMetadata = new Dictionary<string, QueryOperationMetadata>
            {
                {
                    "Valid1", _metadataGenerator.GetOperation("Valid1", "Value1: {0}, Value2: {1}", new []
                    {
                        _metadataGenerator.GetParameter<int>("Valid1Value1", 1),
                        _metadataGenerator.GetParameter<Guid>("Valid1Value2", Guid.NewGuid())
                    })
                },
                {
                    "Valid2", _metadataGenerator.GetOperation("Valid2", "Value1: {0}, Value2: {1}", new []
                    {
                        _metadataGenerator.GetParameter<string>("Valid2Value1", "Test String"),
                        _metadataGenerator.GetParameter<decimal>("Valid2Value2", 233.4M)
                    })
                }
            };

            var operationFormatter = GetQueryOperationFormatter(dbType);

            // Act
            var results = queryFormatter.TestFormatOperations(mockContext.Object, operations, operationMetadata,
                queryFormatter.TestGetNonOrderingFormatOperation(operationFormatter.FormatFilterOperation),
                queryFormatter.TestNoopOperationAction, null);

            // Assert
            foreach (var result in results)
            {
                result.Should().MatchRegex("Value1: @Valid[12]Value1[0-9]*, Value2: @Valid[12]Value2[0-9]*");
            }
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void FormatOperations_WithMissingParameters_ShouldThrowError(SupportedDatabases dbType)
        {

            // Arrange
            var queryFormatter = GetDefaultTestInstance();
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Bogus1", ("BogusValue1", null), ("BogusValue2", true)),
                _metadataGenerator.GetQueryOperation("Valid1"),
                _metadataGenerator.GetQueryOperation("Valid2")
            };

            var operationMetadata = new Dictionary<string, QueryOperationMetadata>
            {
                {
                    "Valid1", _metadataGenerator.GetOperation("Valid1", "Value1: {0}, Value2: {1}", new []
                    {
                        _metadataGenerator.GetParameter<int>("Valid1Value1", 1),
                        _metadataGenerator.GetParameter<Guid>("Valid1Value2", Guid.NewGuid())
                    })
                },
                {
                    "Valid2", _metadataGenerator.GetOperation("Valid2", "Value1: {0}, Value2: {1}", new []
                    {
                        _metadataGenerator.GetParameter<string>("Valid2Value1", "Test String"),
                        _metadataGenerator.GetParameter<decimal>("Valid2Value2")
                    })
                }
            };

            var operationFormatter = GetQueryOperationFormatter(dbType);

            // Act
            Func<List<string>> act = () => queryFormatter.TestFormatOperations(mockContext.Object, operations, operationMetadata,
                queryFormatter.TestGetNonOrderingFormatOperation(operationFormatter.FormatFilterOperation),
                queryFormatter.TestNoopOperationAction, null);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Parameter 'Valid2Value2' is required for the 'Valid2' operation.");
        }

        [Theory]
        [InlineData(SupportedDatabases.SqlServer)]
        [InlineData(SupportedDatabases.PostgreSQL)]
        public void FormatOperations_WithOrderByParameter_ShouldParseAndPassItToFormattingFunction(
            SupportedDatabases dbType)
        {

            // Arrange
            var queryFormatter = GetDefaultTestInstance();
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                _metadataGenerator.GetQueryOperation("Bogus1", ("BogusValue1", null), ("BogusValue2", true)),
                _metadataGenerator.GetQueryOperation("Column1",
                    (DapperWrappersConstants.OrderByDirectionParameter, OrderDirections.Asc.ToString())),
                _metadataGenerator.GetQueryOperation("Subquery1",
                    (DapperWrappersConstants.OrderByDirectionParameter, OrderDirections.Desc.ToString()), ("Value1", 1))
            };

            var operationMetadata = new Dictionary<string, QueryOperationMetadata>
            {
                {
                    "Column1",
                    _metadataGenerator.GetOperation("Column1", "Column1 {0}",
                        new []
                        {
                            _metadataGenerator.GetParameter<OrderDirections>(DapperWrappersConstants.OrderByDirectionParameter, OrderDirections.Asc)
                        })
                },
                {
                    "Subquery1", _metadataGenerator.GetOperation("Subquery1",
                        "(SELECT 1 FROM TestTable WHERE Value1 = {1}) {0}", new[]
                        {
                            _metadataGenerator.GetParameter<OrderDirections>(DapperWrappersConstants.OrderByDirectionParameter, OrderDirections.Asc),
                            _metadataGenerator.GetParameter<int>("Value1")
                        })
                }
            };

            var operationFormatter = GetQueryOperationFormatter(dbType);

            // Act
            var results = queryFormatter.TestFormatOperations(mockContext.Object, operations,
                operationMetadata, operationFormatter.FormatOrderOperation, queryFormatter.TestNoopOperationAction, null, true);

            // Assert
            results.Should().Contain(new[] {"Column1 ASC"});
            results[1].Should().MatchRegex("\\(SELECT 1 FROM TestTable WHERE Value1 = @Value1[0-9]*\\) DESC");
        }
    }

    public class TestQueryFormatter : QueryFormatter<object>
    {
        public List<string> TestFormatOperations<TOpMetadata>(IQueryContext context,
            IEnumerable<QueryOperation> operations, IDictionary<string, TOpMetadata> operationMetadata,
            Func<string, IEnumerable<string>, OrderDirections?, string> formatOperation,
            Action<TOpMetadata, int, object> operationAction, object operationActionState, bool checkOrdering = false,
            bool useUniqueVariables = true) where TOpMetadata : QueryOperationMetadata => FormatOperations(
            context, operations, operationMetadata, formatOperation, operationAction, operationActionState,
            checkOrdering, useUniqueVariables);

        public void TestNoopOperationAction(QueryOperationMetadata metadata, int index, object state) {}

        public Func<string, IEnumerable<string>, OrderDirections?, string> TestGetNonOrderingFormatOperation(
            Func<string, IEnumerable<string>, string> operation)
        {
            return (operationString, variableNames, orderDirection) => operation(operationString, variableNames);
        }
    }
}
