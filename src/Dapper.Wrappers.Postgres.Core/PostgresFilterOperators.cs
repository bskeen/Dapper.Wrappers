using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Postgres.Core
{
    public enum PostgresFilterOperators
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        StringContains,
        StringNotContains,
        StringCaseInsensitiveContains,
        StringCaseInsensitiveNotContains,
        FullTextSearch,
        FullTextPlaintextSearch,
        FullTextWebSearch,
        FullTextSearchWithoutVectorConversion,
        FullTextPlaintextSearchWithoutVectorConversion,
        FullTextWebSearchWithoutVectorConversion,
        In,
        NotIn,
        IsNull,
        IsNotNull,
        Between,
        IsDistinct,
        IsNotDistinct
    }
}
