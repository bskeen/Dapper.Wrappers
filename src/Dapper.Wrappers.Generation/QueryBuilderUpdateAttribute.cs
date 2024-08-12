using System;

namespace Dapper.Wrappers.Generation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QueryBuilderUpdateAttribute : QueryBuilderAttribute
    {
        public QueryBuilderUpdateAttribute(string builderName, int order)
            : base(builderName, order)
        {
        }
    }
}
