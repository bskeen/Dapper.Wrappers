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
    }
}
