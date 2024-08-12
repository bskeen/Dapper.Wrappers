using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Postgres.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PostgresFilterOperationAttribute : PostgresOperationAttribute
    {
        public PostgresFilterOperators OperatorType { get; }
        public string? ColumnAlias { get; set; }

        public PostgresFilterOperationAttribute(PostgresFilterOperators operatorType, string columnName, string queryBuilderName, string collectionKey)
            : base(columnName, queryBuilderName, collectionKey)
        {
            OperatorType = operatorType;
        }
    }
}
