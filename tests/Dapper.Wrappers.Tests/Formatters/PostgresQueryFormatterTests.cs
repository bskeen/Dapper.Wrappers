// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using Dapper.Wrappers.Formatters;
using FluentAssertions;
using Xunit;

namespace Dapper.Wrappers.Tests.Formatters
{
    public class PostgresQueryFormatterTests
    {
        private readonly PostgresQueryFormatter _formatter = new PostgresQueryFormatter();

        [Theory]
        [InlineData("TestIdentifier1", "\"TestIdentifier1\"")]
        [InlineData("TestIdentifier2", "\"TestIdentifier2\"")]
        [InlineData("RidiculouslyLongIdentifierThatShouldNotBeThisLongForAnyReasonWhatsoever", "\"RidiculouslyLongIdentifierThatShouldNotBeThisLongForAnyReasonWhatsoever\"")]
        [InlineData("數據庫", "\"數據庫\"")]
        [InlineData("", "\"\"")]
        [InlineData(null, "\"\"")]
        public void FormatIdentifier_WithInput_ShouldEncloseIdentifierInSquareBrackets(string input, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatIdentifier(input);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("TestVariable1", "@TestVariable1")]
        [InlineData("TestVariable2", "@TestVariable2")]
        [InlineData("RidiculouslyLongVariableNameThatShouldNotBeThisLongForAnyReasonWhatsoever", "@RidiculouslyLongVariableNameThatShouldNotBeThisLongForAnyReasonWhatsoever")]
        [InlineData("數據庫", "@數據庫")]
        [InlineData("", "@")]
        [InlineData(null, "@")]
        public void FormatVariable_WithInput_ShouldAddAnAtSymbolBefore(string input, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatVariable(input);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("@skip", "@take", "LIMIT @take OFFSET @skip")]
        [InlineData("", "@take", "LIMIT @take OFFSET ")]
        [InlineData(null, "@take", "LIMIT @take OFFSET ")]
        [InlineData("@skip", "", "LIMIT  OFFSET @skip")]
        [InlineData("@skip", null, "LIMIT  OFFSET @skip")]
        [InlineData("@ReallyLongSkip", "@ReallyLongTake", "LIMIT @ReallyLongTake OFFSET @ReallyLongSkip")]
        [InlineData("@跳過", "@拿", "LIMIT @拿 OFFSET @跳過")]
        public void FormatPagination_WithInput_ShouldPutVariableNamesWithLimitAndOffset(string skipVariable,
            string takeVariable, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatPagination(skipVariable, takeVariable);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("DELETE FROM \"TestTable\" {0};", "WHERE \"ID\" = 1", "DELETE FROM \"TestTable\" WHERE \"ID\" = 1;")]
        [InlineData("DELETE FROM \"TestTable\" {0};", "", "DELETE FROM \"TestTable\" ;")]
        [InlineData("DELETE FROM \"TestTable\" {0};", null, "DELETE FROM \"TestTable\" ;")]
        [InlineData("This is not a real {0}.", "query", "This is not a real query.")]
        public void FormatDeleteQuery_WithInput_ShouldAddTheCriteriaToTheQuery(string baseQuery, string criteria,
            string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatDeleteQuery(baseQuery, criteria);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("This {0} not {1} real {2}", new[] { "is", "a", "query" }, "This @is not @a real @query")]
        [InlineData("\"TestColumn\" = {0}", new[] { "TestValue" }, "\"TestColumn\" = @TestValue")]
        [InlineData("\"TestDateColumn\" = DATEFROMPARTS({0}, {1}, {2})", new[] { "Year", "Month", "Day" }, "\"TestDateColumn\" = DATEFROMPARTS(@Year, @Month, @Day)")]
        [InlineData("This {0} not {1} real {2}", new[] { "", "", "" }, "This @ not @ real @")]
        [InlineData("This {0} not {1} real {2}", new string[] { null, null, null }, "This @ not @ real @")]
        public void FormatFilterOperation_WithInputs_ShouldInsertVariablesIntoFormatString(string filterOperation,
            string[] variables, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatFilterOperation(filterOperation, variables);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData(new[] { "this", "that" }, "WHERE this AND that")]
        [InlineData(new[] { "\"TestColumn1\" = @TestValue1" }, "WHERE \"TestColumn1\" = @TestValue1")]
        [InlineData(new[] { "\"TestColumn1\" = @TestValue1", "\"TestColumn2\" <> @TestValue2", "\"TestColumn3\" IN @TestValues3" }, "WHERE \"TestColumn1\" = @TestValue1 AND \"TestColumn2\" <> @TestValue2 AND \"TestColumn3\" IN @TestValues3")]
        [InlineData(new string[] { null, null }, "WHERE  AND ")]
        [InlineData(new[] { "", "" }, "WHERE  AND ")]
        public void FormatFilterOperations_WithInputs_ShouldJoinCriteriaWithAndsAndAddWhere(string[] operations,
            string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatFilterOperations(operations);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("{0} {1} {2}", "filter", "order", false, null, null, "filter order ")]
        [InlineData(FormatterTestConstants.Postgres.BaseGetQuery, null, null, false, null, null, FormatterTestConstants.Postgres.GetWithoutAnyAddonsQuery)]
        [InlineData(FormatterTestConstants.Postgres.BaseGetQuery, FormatterTestConstants.Postgres.TestGetWhere, null, false, null, null, FormatterTestConstants.Postgres.GetWithFilterQuery)]
        [InlineData(FormatterTestConstants.Postgres.BaseGetQuery, null, FormatterTestConstants.Postgres.TestGetOrder, false, null, null, FormatterTestConstants.Postgres.GetWithOrderingQuery)]
        [InlineData(FormatterTestConstants.Postgres.BaseGetQuery, null, FormatterTestConstants.Postgres.TestGetOrder, true, "skip", "take", FormatterTestConstants.Postgres.GetWithOrderingPaginationQuery)]
        [InlineData(FormatterTestConstants.Postgres.BaseGetQuery, FormatterTestConstants.Postgres.TestGetWhere, FormatterTestConstants.Postgres.TestGetOrder, false, null, null, FormatterTestConstants.Postgres.GetWithFilterAndOrderingQuery)]
        [InlineData(FormatterTestConstants.Postgres.BaseGetQuery, FormatterTestConstants.Postgres.TestGetWhere, FormatterTestConstants.Postgres.TestGetOrder, true, "skip", "take", FormatterTestConstants.Postgres.GetWithAllPieces)]
        public void FormatGetQuery_WithInputs_AddsAllThePartsIntoQuery(string baseQuery, string filterOperations,
            string orderOperations, bool pagination, string skipVariable, string takeVariable, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatGetQuery(baseQuery, filterOperations, orderOperations, pagination,
                skipVariable, takeVariable);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("TestIdentifier1", "\"TestIdentifier1\"")]
        [InlineData("TestIdentifier2", "\"TestIdentifier2\"")]
        [InlineData("RidiculouslyLongIdentifierThatShouldNotBeThisLongForAnyReasonWhatsoever", "\"RidiculouslyLongIdentifierThatShouldNotBeThisLongForAnyReasonWhatsoever\"")]
        [InlineData("數據庫", "\"數據庫\"")]
        [InlineData("", "\"\"")]
        [InlineData(null, "\"\"")]
        public void FormatInsertColumn_WithInput_ShouldEncloseIdentifierInSquareBrackets(string input, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatInsertColumn(input);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData(new[] { "This", "is", "a", "test" }, "This, is, a, test")]
        [InlineData(new[] { "\"Column1\"", "\"Column2\"", "\"Column3\"" }, "\"Column1\", \"Column2\", \"Column3\"")]
        [InlineData(new[] { "\"Column1\"" }, "\"Column1\"")]
        [InlineData(new string[] { null }, "")]
        [InlineData(new[] { "", "" }, ", ")]
        public void FormatInsertColumns_WithInput_ShouldAddCommasBetweenInputs(string[] inputs, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatInsertColumns(inputs);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("{0}", new[] { "TestValue" }, "@TestValue")]
        [InlineData("(SELECT GenreID FROM Genres WHERE Name = {0})", new[] { "GenreName" }, "(SELECT GenreID FROM Genres WHERE Name = @GenreName)")]
        [InlineData("This {0} not {1} real {2}", new[] { "is", "a", "query" }, "This @is not @a real @query")]
        [InlineData("\"TestColumn\" = {0}", new[] { "TestValue" }, "\"TestColumn\" = @TestValue")]
        [InlineData("\"TestDateColumn\" = DATEFROMPARTS({0}, {1}, {2})", new[] { "Year", "Month", "Day" }, "\"TestDateColumn\" = DATEFROMPARTS(@Year, @Month, @Day)")]
        [InlineData("This {0} not {1} real {2}", new[] { "", "", "" }, "This @ not @ real @")]
        [InlineData("This {0} not {1} real {2}", new string[] { null, null, null }, "This @ not @ real @")]
        public void FormatInsertOperation_WithInputs_ShouldAddVariablesToBase(string operation, string[] variableNames,
            string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatInsertOperation(operation, variableNames);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData(new[] { "This", "is", "a", "test" }, "This, is, a, test")]
        [InlineData(new[] { "@Value1", "@Value2", "@Value3" }, "@Value1, @Value2, @Value3")]
        [InlineData(new[] { "@Value1" }, "@Value1")]
        [InlineData(new string[] { null }, "")]
        [InlineData(new[] { "", "" }, ", ")]
        public void FormatInsertOperations_WithInputs_ShouldInsertCommasAndSpacesBetweenValues(string[] inputs,
            string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatInsertOperations(inputs);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("{0} {1}", "not a", "query", "not a query")]
        [InlineData(FormatterTestConstants.Postgres.BaseInsertQuery, FormatterTestConstants.Postgres.TestColumns, FormatterTestConstants.Postgres.TestValues, FormatterTestConstants.Postgres.InsertWithColumnsAndValues)]
        [InlineData(FormatterTestConstants.Postgres.BaseInsertQuery, FormatterTestConstants.Postgres.TestColumns, FormatterTestConstants.Postgres.TestSubqueries, FormatterTestConstants.Postgres.InsertWithColumnsAndSubqueries)]
        public void FormatInsertQuery_WithInputs_ShouldAddTheColumnListAndOperationsInTheCorrectPlaces(string baseQuery,
            string columnList, string insertOperations, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatInsertQuery(baseQuery, columnList, insertOperations);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("{0} {1} {2} {3} {4}", new[] { "is", "not", "a", "query" }, OrderDirections.Asc, "ASC @is @not @a @query")]
        [InlineData("{0} {1} {2} {3} {4}", new[] { "is", "not", "a", "query" }, OrderDirections.Desc, "DESC @is @not @a @query")]
        [InlineData("\"Column1\" {0}", new string[] { }, OrderDirections.Asc, "\"Column1\" ASC")]
        [InlineData("\"Column1\" {0}", new string[] { }, OrderDirections.Desc, "\"Column1\" DESC")]
        [InlineData("(SELECT 1 FROM Genres WHERE Name = {1}) {0}", new[] { "Name" }, OrderDirections.Asc, "(SELECT 1 FROM Genres WHERE Name = @Name) ASC")]
        [InlineData("(SELECT 1 FROM Genres WHERE Name = {1}) {0}", new[] { "Name" }, OrderDirections.Desc, "(SELECT 1 FROM Genres WHERE Name = @Name) DESC")]
        public void FormatOrderOperation_WithInputs_ShouldAddCorrectDirectionAndVariables(string baseOperation,
            string[] variables, OrderDirections direction, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatOrderOperation(baseOperation, variables, direction);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData(new[] { "\"Column1\" ASC" }, "ORDER BY \"Column1\" ASC")]
        [InlineData(new[] { "\"Column1\" ASC", "\"Column2\" DESC" }, "ORDER BY \"Column1\" ASC, \"Column2\" DESC")]
        [InlineData(new[] { "is", "not", "a", "query" }, "ORDER BY is, not, a, query")]
        public void FormatOrderOperations_WithInputs_ShouldCommaDelimitWithOrderBy(string[] operations, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatOrderOperations(operations);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData("\"Column1\" = {0}", new[] { "Value1" }, "\"Column1\" = @Value1")]
        [InlineData("\"Column1\" = (SELECT 1 FROM Genres WHERE Name = {0} OR GenreID = {1})", new[] { "Name", "GenreID" }, "\"Column1\" = (SELECT 1 FROM Genres WHERE Name = @Name OR GenreID = @GenreID)")]
        [InlineData("This {0} not {1} real {2}", new[] { "is", "a", "query" }, "This @is not @a real @query")]
        [InlineData("\"TestColumn\" = {0}", new[] { "TestValue" }, "\"TestColumn\" = @TestValue")]
        [InlineData("\"TestDateColumn\" = DATEFROMPARTS({0}, {1}, {2})", new[] { "Year", "Month", "Day" }, "\"TestDateColumn\" = DATEFROMPARTS(@Year, @Month, @Day)")]
        [InlineData("This {0} not {1} real {2}", new[] { "", "", "" }, "This @ not @ real @")]
        [InlineData("This {0} not {1} real {2}", new string[] { null, null, null }, "This @ not @ real @")]
        public void FormatUpdateOperation_WithInputs_ShouldAddVariableNamesToBaseQueryPiece(string operation,
            string[] variables, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatUpdateOperation(operation, variables);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData(new[] { "\"Column1\" = @Value1" }, "SET \"Column1\" = @Value1")]
        [InlineData(new[] { "\"Column1\" = @Value1", "\"Column2\" = @Value2" }, "SET \"Column1\" = @Value1, \"Column2\" = @Value2")]
        [InlineData(new[] { "is", "not", "a", "query" }, "SET is, not, a, query")]
        public void FormatUpdateOperations_WithInputs_ShouldAddSetAndCommas(string[] operations, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatUpdateOperations(operations);

            // Assert
            result.Should().Be(output);
        }

        [Theory]
        [InlineData(FormatterTestConstants.Postgres.BaseUpdateQuery, FormatterTestConstants.Postgres.TestUpdateOperations, null, FormatterTestConstants.Postgres.UpdateWithoutWhere)]
        [InlineData(FormatterTestConstants.Postgres.BaseUpdateQuery, FormatterTestConstants.Postgres.TestUpdateOperations, "", FormatterTestConstants.Postgres.UpdateWithoutWhere)]
        [InlineData(FormatterTestConstants.Postgres.BaseUpdateQuery, FormatterTestConstants.Postgres.TestUpdateOperations, FormatterTestConstants.Postgres.TestUpdateWhere, FormatterTestConstants.Postgres.UpdateWithEverything)]
        public void FormatUpdateQuery_WithInputs_ShouldReturnCorrectlyFormattedQuery(string baseQuery,
            string operations, string criteria, string output)
        {
            // Arrange
            // Nothing to do here...

            // Act
            var result = _formatter.FormatUpdateQuery(baseQuery, operations, criteria);

            // Assert
            result.Should().Be(output);
        }
    }
}
