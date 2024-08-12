using System;

namespace Dapper.Wrappers.Generation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QueryBuilderFilterAttribute : QueryBuilderAttribute
    {
        public QueryBuilderFilterAttribute(string builderName, int order)
            : base(builderName, order)
        {
        }
    }
}
