using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers
{
    public interface IQueryFormatter
    {
        string FormatFilterItem(string filterItemString, string variableName);
        string FormatFilterItems(IEnumerable<string> filterItems);
        string FormatOrderItems(IEnumerable<string> orderItems);
        string FormatGetQuery(string baseQueryString, string filterItems, string orderItems, bool isPaginated, string skipVariableName, string takeVariableName);
        string FormatDeleteQuery(string baseQueryString, string deleteCriteria);
    }
}
