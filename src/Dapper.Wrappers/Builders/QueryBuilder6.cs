// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Builders
{
    public abstract class QueryBuilder<TOperations1, TOperations2, TOperations3, TOperations4, TOperations5,
        TOperations6> : QueryBuilder,
        IQueryBuilder<TOperations1, TOperations2, TOperations3, TOperations4, TOperations5, TOperations6>
    {
        public void AddQueryToContext(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> queryOperations1,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations2,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations3,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations4,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations5,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations6)
        {
            var formattedOperations1 = GetFormattedOperations1(queryOperations1);
            var formattedOperations2 = GetFormattedOperations2(queryOperations2);
            var formattedOperations3 = GetFormattedOperations3(queryOperations3);
            var formattedOperations4 = GetFormattedOperations4(queryOperations4);
            var formattedOperations5 = GetFormattedOperations5(queryOperations5);
            var formattedOperations6 = GetFormattedOperations6(queryOperations6);

            var formattedQuery = string.Format(QueryFormat, formattedOperations1, formattedOperations2,
                formattedOperations3, formattedOperations4, formattedOperations5, formattedOperations6);

            context.AddQuery(formattedQuery);
        }

        public void AddQueryToContext(IQueryContext context, TOperations1 operationObject1,
            TOperations2 operationObject2, TOperations3 operationObject3, TOperations4 operationObject4,
            TOperations5 operationObject5, TOperations6 operationObject6)
        {
            var operations1 = GetOperationsFromObject(operationObject1);
            var operations2 = GetOperationsFromObject(operationObject2);
            var operations3 = GetOperationsFromObject(operationObject3);
            var operations4 = GetOperationsFromObject(operationObject4);
            var operations5 = GetOperationsFromObject(operationObject5);
            var operations6 = GetOperationsFromObject(operationObject6);

            AddQueryToContext(context, operations1, operations2, operations3, operations4, operations5, operations6);
        }

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations1 operationObject);
        public abstract string GetFormattedOperations1(IEnumerable<IEnumerable<QueryOperation>> operations);

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations2 operationObject);
        public abstract string GetFormattedOperations2(IEnumerable<IEnumerable<QueryOperation>> operations);

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations3 operationObject);
        public abstract string GetFormattedOperations3(IEnumerable<IEnumerable<QueryOperation>> operations);

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations4 operationObject);
        public abstract string GetFormattedOperations4(IEnumerable<IEnumerable<QueryOperation>> operations);

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations5 operationObject);
        public abstract string GetFormattedOperations5(IEnumerable<IEnumerable<QueryOperation>> operations);

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations6 operationObject);
        public abstract string GetFormattedOperations6(IEnumerable<IEnumerable<QueryOperation>> operations);
    }
}
