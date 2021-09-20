// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dapper.Wrappers.Builders
{
    public abstract class QueryBuilder : IQueryBuilder
    {
        public abstract string QueryFormat { get; }
    }
}
