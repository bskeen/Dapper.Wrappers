using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Postgres.Core
{
    public class PostgresOrderingOperationAttribute : PostgresOperationAttribute
    {
        public string? ColumnAlias { get; set; }

        public PostgresOrderingOperationAttribute(string columnName, string queryBuilderName, string collectionKey)
            : base(columnName, queryBuilderName, collectionKey)
        {
        }
    }
}
