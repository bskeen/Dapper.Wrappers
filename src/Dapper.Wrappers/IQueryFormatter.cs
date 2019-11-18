using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers
{
    public interface IQueryFormatter
    {
        string FormatFilterOperation(string filterOperationString, IEnumerable<string> variableNames, OrderDirections? orderDirection);
        string FormatFilterOperations(IEnumerable<string> filterOperations);
        string FormatOrderOperation(string orderOperationString, IEnumerable<string> variableNames, OrderDirections? orderDirection);
        string FormatOrderOperations(IEnumerable<string> orderOperations);
        string FormatUpdateOperation(string updateOperationString, IEnumerable<string> variableNames, OrderDirections? orderDirection);
        string FormatUpdateOperations(IEnumerable<string> updateOperations);
        string FormatInsertOperation(string insertOperation, IEnumerable<string> variableNames, OrderDirections? orderDirection);
        string FormatInsertOperations(IEnumerable<string> insertOperations);
        string FormatInsertColumn(string insertColumn);
        string FormatInsertColumns(IEnumerable<string> insertColumns);
        string FormatGetQuery(string baseQueryString, string filterOperations, string orderOperations, bool isPaginated, string skipVariableName, string takeVariableName);
        string FormatDeleteQuery(string baseQueryString, string deleteCriteria);
        string FormatUpdateQuery(string entityUpdates, string updateCriteria);
        string FormatInsertQuery(string baseQueryString, string columnList, string insertOperations);
    }
}
