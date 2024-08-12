using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Postgres.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PostgresInsertOperationAttribute : PostgresOperationAttribute
    {
        public bool Required { get; set; }
        public object? Default { get; set; }

        public PostgresInsertOperationAttribute(string columnName, string queryBuilderName, string collectionKey)
            : base(columnName, queryBuilderName, collectionKey)
        {
        }
    }
}
