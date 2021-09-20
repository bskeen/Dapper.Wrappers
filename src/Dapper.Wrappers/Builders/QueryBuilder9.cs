// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Builders
{
    public abstract class QueryBuilder<TOperations1, TOperations2, TOperations3, TOperations4, TOperations5,
        TOperations6, TOperations7, TOperations8, TOperations9> : QueryBuilder,
        IQueryBuilder<TOperations1, TOperations2, TOperations3, TOperations4, TOperations5, TOperations6, TOperations7,
            TOperations8, TOperations9>
    {
        public void AddQueryToContext(IQueryContext context, IEnumerable<IEnumerable<QueryOperation>> queryOperations1,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations2,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations3,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations4,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations5,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations6,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations7,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations8,
            IEnumerable<IEnumerable<QueryOperation>> queryOperations9)
        {
            var formattedOperations1 = GetFormattedOperations1(queryOperations1);
            var formattedOperations2 = GetFormattedOperations2(queryOperations2);
            var formattedOperations3 = GetFormattedOperations3(queryOperations3);
            var formattedOperations4 = GetFormattedOperations4(queryOperations4);
            var formattedOperations5 = GetFormattedOperations5(queryOperations5);
            var formattedOperations6 = GetFormattedOperations6(queryOperations6);
            var formattedOperations7 = GetFormattedOperations7(queryOperations7);
            var formattedOperations8 = GetFormattedOperations8(queryOperations8);
            var formattedOperations9 = GetFormattedOperations9(queryOperations9);

            var formattedQuery = string.Format(QueryFormat, formattedOperations1, formattedOperations2,
                formattedOperations3, formattedOperations4, formattedOperations5, formattedOperations6,
                formattedOperations7, formattedOperations8, formattedOperations9);

            context.AddQuery(formattedQuery);
        }

        public void AddQueryToContext(IQueryContext context, TOperations1 operationObject1,
            TOperations2 operationObject2, TOperations3 operationObject3, TOperations4 operationObject4,
            TOperations5 operationObject5, TOperations6 operationObject6, TOperations7 operationObject7,
            TOperations8 operationObject8, TOperations9 operationObject9)
        {
            var operations1 = GetOperationsFromObject(operationObject1);
            var operations2 = GetOperationsFromObject(operationObject2);
            var operations3 = GetOperationsFromObject(operationObject3);
            var operations4 = GetOperationsFromObject(operationObject4);
            var operations5 = GetOperationsFromObject(operationObject5);
            var operations6 = GetOperationsFromObject(operationObject6);
            var operations7 = GetOperationsFromObject(operationObject7);
            var operations8 = GetOperationsFromObject(operationObject8);
            var operations9 = GetOperationsFromObject(operationObject9);

            AddQueryToContext(context, operations1, operations2, operations3, operations4, operations5, operations6,
                operations7, operations8, operations9);
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

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations7 operationObject);
        public abstract string GetFormattedOperations7(IEnumerable<IEnumerable<QueryOperation>> operations);

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations8 operationObject);
        public abstract string GetFormattedOperations8(IEnumerable<IEnumerable<QueryOperation>> operations);

        public abstract IEnumerable<IEnumerable<QueryOperation>> GetOperationsFromObject(TOperations9 operationObject);
        public abstract string GetFormattedOperations9(IEnumerable<IEnumerable<QueryOperation>> operations);
    }
}
