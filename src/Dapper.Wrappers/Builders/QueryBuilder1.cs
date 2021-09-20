// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Builders
{
    public abstract class QueryBuilder<TOperations> : QueryBuilder, IQueryBuilder<TOperations>
    {
        public void AddQueryToContext(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> queryOperations)
        {
            var formattedOperations = GetFormattedOperations(queryOperations);

            var formattedQuery = string.Format(QueryFormat, formattedOperations);
            
            context.AddQuery(formattedQuery);
        }

        public void AddQueryToContext(IQueryContext context, TOperations operationObject)
        {
            var operations = GetOperationsFromObject(operationObject);
            AddQueryToContext(context, operations);
        }

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations operationObject);

        public abstract string GetFormattedOperations(IEnumerable<IEnumerable<QueryOperation>> operations);
    }
}
