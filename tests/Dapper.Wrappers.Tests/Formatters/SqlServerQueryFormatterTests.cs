// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Wrappers.Formatters;
using FluentAssertions;
using Xunit;

namespace Dapper.Wrappers.Tests.Formatters
{
    public class SqlServerQueryFormatterTests
    {
        private SqlServerQueryFormatter _formatter = new SqlServerQueryFormatter();

        [Theory]
        [InlineData("TestIdentifier1", "[TestIdentifier1]")]
        [InlineData("TestIdentifier2", "[TestIdentifier2]")]
        [InlineData("RidiculouslyLongIdentifierThatShouldNotBeThisLongForAnyReasonWhatsoever", "[RidiculouslyLongIdentifierThatShouldNotBeThisLongForAnyReasonWhatsoever]")]
        [InlineData("數據庫", "[數據庫]")]
        [InlineData("", "[]")]
        [InlineData(null, "[]")]
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
        [InlineData("@skip", "@take", "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY")]
        [InlineData("", "@take", "OFFSET  ROWS FETCH NEXT @take ROWS ONLY")]
        [InlineData(null, "@take", "OFFSET  ROWS FETCH NEXT @take ROWS ONLY")]
        [InlineData("@skip", "", "OFFSET @skip ROWS FETCH NEXT  ROWS ONLY")]
        [InlineData("@skip", null, "OFFSET @skip ROWS FETCH NEXT  ROWS ONLY")]
        [InlineData("@ReallyLongSkip", "@ReallyLongTake", "OFFSET @ReallyLongSkip ROWS FETCH NEXT @ReallyLongTake ROWS ONLY")]
        [InlineData("@跳過", "@拿", "OFFSET @跳過 ROWS FETCH NEXT @拿 ROWS ONLY")]
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
        [InlineData("DELETE FROM [TestTable] {0};", "WHERE [ID] = 1", "DELETE FROM [TestTable] WHERE [ID] = 1;")]
        [InlineData("DELETE FROM [TestTable] {0};", "", "DELETE FROM [TestTable] ;")]
        [InlineData("DELETE FROM [TestTable] {0};", null, "DELETE FROM [TestTable] ;")]
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
        [InlineData("This {0} not {1} real {2}", new [] {"is", "a", "query"}, "This @is not @a real @query")]
        [InlineData("[TestColumn] = {0}", new [] { "TestValue" }, "[TestColumn] = @TestValue")]
        [InlineData("[TestDateColumn] = DATEFROMPARTS({0}, {1}, {2})", new [] {"Year", "Month", "Day"}, "[TestDateColumn] = DATEFROMPARTS(@Year, @Month, @Day)")]
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
    }
}
