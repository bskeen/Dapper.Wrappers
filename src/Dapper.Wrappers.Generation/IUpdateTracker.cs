using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Generation
{
    public interface IUpdateTracker
    {
        IEnumerable<string> UpdatedProperties { get; }
    }
}
