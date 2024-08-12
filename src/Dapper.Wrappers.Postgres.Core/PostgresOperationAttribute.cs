using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Postgres.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class PostgresOperationAttribute : Attribute
    {
        public string ColumnName { get; }
        public string QueryBuilderName { get; }
        public string CollectionKey { get; }

        public PostgresOperationAttribute(string columnName, string queryBuilderName, string collectionKey)
        {
            ColumnName = columnName;
            QueryBuilderName = queryBuilderName;
            CollectionKey = collectionKey;
        }
    }
}
