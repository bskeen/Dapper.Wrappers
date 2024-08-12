using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Postgres.Core
{
    public class PostgresUpdateOperationAttribute : PostgresOperationAttribute
    {
        public string? ColumnAlias { get; set; }

        public PostgresUpdateOperationAttribute(string columnName, string queryBuilderName, string collectionKey)
            : base(columnName, queryBuilderName, collectionKey)
        {
        }
    }
}
