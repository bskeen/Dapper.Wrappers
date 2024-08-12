using System;

namespace Dapper.Wrappers.Generation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QueryBuilderInsertAttribute : QueryBuilderAttribute
    {
        public bool IsMultiple { get; set; } = true;

        public QueryBuilderInsertAttribute(string builderName, int order)
            : base(builderName, order)
        {
        }
    }
}
