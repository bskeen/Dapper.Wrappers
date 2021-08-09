// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper.Wrappers.DependencyInjection;
using Dapper.Wrappers.Formatters;
using Dapper.Wrappers.Generators;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dapper.Wrappers.Tests.Generators
{
    public class QueryGeneratorTests
    {
        private static TestQueryGenerator GetDefaultTestInstance(SupportedDatabases dbType)
        {
            IQueryFormatter formatter = dbType == SupportedDatabases.SqlServer
                ? (IQueryFormatter)(new SqlServerQueryFormatter())
                : new PostgresQueryFormatter();
            return new TestQueryGenerator(formatter);
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

        public QueryGeneratorTests(IMetadataGenerator metadataGenerator)
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
            var generator = GetDefaultTestInstance(dbType);
            var mockContext = GetMockQueryContext();
            IEnumerable<QueryOperation> operations = isNull ? null : new List<QueryOperation>();

            // Act
            var results =
                generator.TestFormatOperations<QueryOperationMetadata>(mockContext.Object, operations, null, null,
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
        public void FormatOperations_WithBogusOperators_ShouldReturnAnEmptyList(SupportedDatabases dbType)
        {
            // Arrange
            var generator = GetDefaultTestInstance(dbType);
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                new QueryOperation
                {
                    Name = "Bogus1",
                    Parameters = new Dictionary<string, object>
                    {
                        { "BogusValue1", null },
                        { "BogusValue2", true }
                    }
                },
                new QueryOperation
                {
                    Name = "Bogus2",
                    Parameters = new Dictionary<string, object>
                    {
                        { "EvenMoreBogusValue1", 1 },
                        { "EvenMoreBogusValue2", Guid.NewGuid() }
                    }
                }
            };

            var operationMetadata = new Dictionary<string, QueryOperationMetadata>
            {
                {"Real1", _metadataGenerator.GetDefaultOperation<string>("Real1", "this is not a query")},
                {"Real2", _metadataGenerator.GetDefaultOperation<string>("Real2", "this is not a query either")}
            };

            // Act
            var results = generator.TestFormatOperations(mockContext.Object, operations, operationMetadata, null, null);

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
            var generator = GetDefaultTestInstance(dbType);
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                new QueryOperation
                {
                    Name = "Bogus1",
                    Parameters = new Dictionary<string, object>
                    {
                        { "BogusValue1", null },
                        { "BogusValue2", true }
                    }
                },
                new QueryOperation
                {
                    Name = "Valid1",
                    Parameters = new Dictionary<string, object>
                    {
                        { "Valid1Value1", 1 },
                        { "Valid1Value2", Guid.NewGuid() }
                    }
                },
                new QueryOperation
                {
                    Name = "Valid2",
                    Parameters = new Dictionary<string, object>
                    {
                        { "Valid2Value1", "Test String"},
                        { "Valid2Value2", 233.4M }
                    }
                }
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

            // Act
            var results = generator.TestFormatOperations(mockContext.Object, operations, operationMetadata,
                generator.TestGetNonOrderingFormatOperation(generator.TestFormatter.FormatFilterOperation), TestAction);

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

            void TestAction(QueryOperationMetadata metadata)
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
            var generator = GetDefaultTestInstance(dbType);
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                new QueryOperation
                {
                    Name = "Bogus1",
                    Parameters = new Dictionary<string, object>
                    {
                        { "BogusValue1", null },
                        { "BogusValue2", true }
                    }
                },
                new QueryOperation
                {
                    Name = "Valid1",
                    Parameters = new Dictionary<string, object>()
                },
                new QueryOperation
                {
                    Name = "Valid2",
                    Parameters = new Dictionary<string, object>()
                }
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

            // Act
            var results = generator.TestFormatOperations(mockContext.Object, operations, operationMetadata,
                generator.TestGetNonOrderingFormatOperation(generator.TestFormatter.FormatFilterOperation),
                generator.TestNoopOperationAction);

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
            var generator = GetDefaultTestInstance(dbType);
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                new QueryOperation
                {
                    Name = "Bogus1",
                    Parameters = new Dictionary<string, object>
                    {
                        { "BogusValue1", null },
                        { "BogusValue2", true }
                    }
                },
                new QueryOperation
                {
                    Name = "Valid1",
                    Parameters = new Dictionary<string, object>()
                },
                new QueryOperation
                {
                    Name = "Valid2",
                    Parameters = new Dictionary<string, object>()
                }
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

            // Act
            Func<List<string>> act = () => generator.TestFormatOperations(mockContext.Object, operations, operationMetadata,
                generator.TestGetNonOrderingFormatOperation(generator.TestFormatter.FormatFilterOperation),
                generator.TestNoopOperationAction);

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
            var generator = GetDefaultTestInstance(dbType);
            var mockContext = GetMockQueryContext();

            var operations = new[]
            {
                new QueryOperation
                {
                    Name = "Bogus1",
                    Parameters = new Dictionary<string, object>
                    {
                        { "BogusValue1", null },
                        { "BogusValue2", true }
                    }
                },
                new QueryOperation
                {
                    Name = "Column1",
                    Parameters = new Dictionary<string, object>
                    {
                        {DapperWrappersConstants.OrderByDirectionParameter, OrderDirections.Asc.ToString()}
                    }
                },
                new QueryOperation
                {
                    Name = "Subquery1",
                    Parameters = new Dictionary<string, object>
                    {
                        {DapperWrappersConstants.OrderByDirectionParameter, OrderDirections.Desc.ToString()},
                        {"Value1", 1}
                    }
                }
            };

            var operationMetadata = new Dictionary<string, QueryOperationMetadata>
            {
                {
                    "Column1",
                    _metadataGenerator.GetOperation("Column1", "Column1 {0}",
                        new QueryParameterMetadata[]
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

            // Act
            var results = generator.TestFormatOperations(mockContext.Object, operations,
                operationMetadata, generator.TestFormatter.FormatOrderOperation, generator.TestNoopOperationAction, true);

            // Assert
            results.Should().Contain(new[] {"Column1 ASC"});
            results[1].Should().MatchRegex("\\(SELECT 1 FROM TestTable WHERE Value1 = @Value1[0-9]*\\) DESC");
        }
    }

    public class TestQueryGenerator : QueryGenerator
    {
        public TestQueryGenerator(IQueryFormatter queryFormatter) : base(queryFormatter)
        {
        }

        public IQueryFormatter TestFormatter => QueryFormatter;

        public List<string> TestFormatOperations<TOpMetadata>(IQueryContext context,
            IEnumerable<QueryOperation> operations, IDictionary<string, TOpMetadata> operationMetadata,
            Func<string, IEnumerable<string>, OrderDirections?, string> formatOperation,
            Action<TOpMetadata> operationAction, bool checkOrdering = false, bool useUniqueVariables = true)
            where TOpMetadata : QueryOperationMetadata => FormatOperations(context, operations, operationMetadata,
            formatOperation, operationAction, checkOrdering, useUniqueVariables);

        public void TestNoopOperationAction(QueryOperationMetadata metadata){}

        public Func<string, IEnumerable<string>, OrderDirections?, string> TestGetNonOrderingFormatOperation(
            Func<string, IEnumerable<string>, string> operation)
        {
            return (operationString, variableNames, orderDirection) => operation(operationString, variableNames);
        }
    }
}
