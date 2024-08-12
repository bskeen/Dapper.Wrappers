hhggjkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkusing System;

namespace Dapper.Wrappers.Generation
{
    public abstract class QueryBuilderAttribute : Attribute
    {
        public string BuilderName { get; }
        public int Order { get; }
        public string QueryFormat { get; set; }

        public QueryBuilderAttribute(string builderName, int order)
        {
            BuilderName = builderName;
            Order = order;
        }
    }
}
