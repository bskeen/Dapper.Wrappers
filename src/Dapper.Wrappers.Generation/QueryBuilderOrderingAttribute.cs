using System;

namespace Dapper.Wrappers.Generation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QueryBuilderOrderingAttribute : QueryBuilderAttribute
    {
        public string DefaultOrdering { get; }

        public QueryBuilderOrderingAttribute(string builderName, int order, string defaultOrdering)
            : base(builderName, order)
        {
            DefaultOrdering = defaultOrdering;
        }
    }
}
