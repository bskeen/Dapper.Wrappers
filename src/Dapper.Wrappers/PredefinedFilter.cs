using System;
namespace Dapper.Wrappers
{
    public class PredefinedFilter : IFilterItem
    {
        /// <summary>
        /// The name of the key associated with the filter.
        /// </summary>
        public string KeyName { get; set; }
    }
}
