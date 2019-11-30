// © 2019 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.Wrappers
{
    public abstract class BaseQueryGenerator<M> : IQueryGenerator<M>
    {
        protected readonly IQueryResultsProcessorProvider _resultsProcessorProvider;
        protected readonly IQueryFormatter _queryFormatter;

        protected abstract string GetQueryString { get; }
        protected abstract string DefaultOrdering { get; }
        protected abstract string InsertQueryString { get; }
        protected abstract string UpdateQueryString { get; }
        protected abstract string DeleteQueryString { get; }
        protected abstract IDictionary<string, QueryOperationMetadata> FilterOperationMetadata { get; }
        protected abstract IDictionary<string, QueryOperationMetadata> OrderOperationMetadata { get; }
        protected abstract IDictionary<string, QueryOperationMetadata> UpdateOperationMetadata { get; }
        protected abstract IDictionary<string, QueryOperationMetadata> InsertOperationMetadata { get; }
        protected abstract IDictionary<string, MergeOperationMetadata> GetRequiredInsertOperationMetadata();
        protected abstract string DefaultOrderItem { get; }

        public BaseQueryGenerator(IQueryResultsProcessorProvider resultsProcessorProvider, IQueryFormatter queryFormatter)
        {
            _resultsProcessorProvider = resultsProcessorProvider;
            _queryFormatter = queryFormatter;
        }

        public virtual IQueryResultsProcessor<M> AddGetQuery(IQueryContext context, IEnumerable<QueryOperation> filterItems = null, IEnumerable<QueryOperation> orderItems = null,
            Pagination pagination = null)
        {
            string skipVariable = null;
            string takeVariable = null;

            if (pagination != null)
            {
                skipVariable = context.AddVariable("skip", pagination.Skip, System.Data.DbType.Int32);
                takeVariable = context.AddVariable("take", pagination.Take, System.Data.DbType.Int32);
            }

            var formattedFilterItems = FormatOperations(context, filterItems, FilterOperationMetadata, _queryFormatter.FormatFilterOperation, NoopOperationAction);

            string criteria;
            
            if (formattedFilterItems.Count == 0)
            {
                criteria =  string.Empty;
            }
            else
            {
                criteria = _queryFormatter.FormatFilterOperations(formattedFilterItems);
            }

            var formattedOrderItems = FormatOperations(context, orderItems, OrderOperationMetadata, _queryFormatter.FormatOrderOperation, NoopOperationAction, checkOrdering: true);

            string ordering;

            if (formattedOrderItems.Count == 0)
            {
                ordering = pagination == null ? string.Empty : DefaultOrdering;
            }
            else
            {
                ordering = _queryFormatter.FormatOrderOperations(formattedOrderItems);
            }

            var query = _queryFormatter.FormatGetQuery(GetQueryString, criteria, ordering, pagination != null, skipVariable, takeVariable);

            var resultsHandler = _resultsProcessorProvider.GetQueryResultsProcessor<M>();

            context.AddQuery(query, resultsHandler);

            return resultsHandler;
        }

        public virtual IQueryResultsProcessor<M> AddInsertQuery(IQueryContext context, IEnumerable<QueryOperation> insertOperations)
        {
            if (insertOperations == null)
            {
                throw new ArgumentException("Insert operations cannot be null.");
            }

            var requiredOperations = GetRequiredInsertOperationMetadata();

            var currentColumns = new HashSet<string>();

            List<string> formattedColumnNames = new List<string>();

            var formattedOperations = FormatOperations(context, insertOperations, InsertOperationMetadata,
                _queryFormatter.FormatInsertOperation, InsertOperationAction);

            AddRequiredInsertOperations(context, requiredOperations, formattedColumnNames, formattedOperations);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No insert operations specified.");
            }

            var columnList = _queryFormatter.FormatInsertColumns(formattedColumnNames);
            var operationList = _queryFormatter.FormatInsertOperations(formattedOperations);
            var query = _queryFormatter.FormatInsertQuery(InsertQueryString, columnList, operationList);

            var resultsHandler = _resultsProcessorProvider.GetQueryResultsProcessor<M>();
            context.AddQuery(query, resultsHandler);

            return resultsHandler;

            void InsertOperationAction(QueryOperationMetadata metadata)
            {
                if (!(metadata is MergeOperationMetadata insertMetadata))
                {
                    throw new ArgumentException("Metadata must be of type MergeOperationMetadata.");
                }

                if (requiredOperations.ContainsKey(insertMetadata.ReferencedColumn))
                {
                    requiredOperations.Remove(insertMetadata.ReferencedColumn);
                }

                if (currentColumns.Contains(insertMetadata.ReferencedColumn))
                {
                    throw new ArgumentException("Cannot have multiple inserts into the same column.");
                }

                currentColumns.Add(insertMetadata.ReferencedColumn);
                formattedColumnNames.Add(_queryFormatter.FormatInsertColumn(insertMetadata.ReferencedColumn));
            }
        }

        private void AddRequiredInsertOperations(IQueryContext context,
            IDictionary<string, MergeOperationMetadata> requiredOperations, List<string> formattedColumnNames,
            List<string> formattedOperations)
        {
            foreach (var requiredOperation in requiredOperations)
            {
                List<string> parameterNames = new List<string>();

                foreach (var parameter in requiredOperation.Value.Parameters)
                {
                    if (!parameter.HasDefault)
                    {
                        throw new ArgumentException($"Value must be supplied for '{requiredOperation.Key}'.");
                    }

                    var variableName = context.AddVariable(parameter.Name, parameter.DefaultValue, parameter.DbType);

                    parameterNames.Add(variableName);
                }

                formattedColumnNames.Add(_queryFormatter.FormatInsertColumn(requiredOperation.Key));
                formattedOperations.Add(_queryFormatter.FormatInsertOperation(requiredOperation.Value.BaseQueryString, parameterNames, null));
            }
        }

        public virtual IQueryResultsProcessor<M> AddUpdateQuery(IQueryContext context, IEnumerable<QueryOperation> updateOperations, IEnumerable<QueryOperation> updateCriteria)
        {
            var currentColumns = new HashSet<string>();

            var formattedOperations = FormatOperations(context, updateOperations, UpdateOperationMetadata, _queryFormatter.FormatUpdateOperation, UpdateOperationAction);

            if (formattedOperations.Count == 0)
            {
                throw new ArgumentException("No update operations specified.");
            }

            string operations = _queryFormatter.FormatUpdateOperations(formattedOperations);

            var formattedUpdateCriteria = FormatOperations(context, updateCriteria, FilterOperationMetadata, _queryFormatter.FormatFilterOperation, NoopOperationAction);

            string criteria;

            if (formattedUpdateCriteria.Count == 0)
            {
                criteria = string.Empty;
            }
            else
            {
                criteria = _queryFormatter.FormatFilterOperations(formattedUpdateCriteria);
            }

            var query = _queryFormatter.FormatUpdateQuery(UpdateQueryString, operations, criteria);

            var resultsHandler = _resultsProcessorProvider.GetQueryResultsProcessor<M>();

            context.AddQuery(query, resultsHandler);

            return resultsHandler;

            void UpdateOperationAction(QueryOperationMetadata metadata)
            {
                if (!(metadata is MergeOperationMetadata updateMetadata))
                {
                    throw new ArgumentException("Metadata must be of type MergeOperationMetadata.");
                }

                if (currentColumns.Contains(updateMetadata.ReferencedColumn))
                {
                    throw new ArgumentException("Cannot have multiple updates of the same column.");
                }

                currentColumns.Add(updateMetadata.ReferencedColumn);
            }
        }

        public virtual void AddDeleteQuery(IQueryContext context, IEnumerable<QueryOperation> deleteCriteria)
        {
            var formattedDeleteCriteria = FormatOperations(context, deleteCriteria, FilterOperationMetadata, _queryFormatter.FormatFilterOperation, NoopOperationAction);

            string criteria;

            if (formattedDeleteCriteria.Count == 0)
            {
                criteria = string.Empty;
            }
            else
            {
                criteria = _queryFormatter.FormatFilterOperations(formattedDeleteCriteria);
            }

            var query = _queryFormatter.FormatDeleteQuery(DeleteQueryString, criteria);

            var resultsHandler = _resultsProcessorProvider.GetQueryResultsProcessor<M>();

            context.AddQuery(query, resultsHandler);
        }

        protected virtual List<string> FormatOperations(IQueryContext context, IEnumerable<QueryOperation> operations,
            IDictionary<string, QueryOperationMetadata> operationMetadata,
            Func<string, IEnumerable<string>, OrderDirections?, string> formatOperation, Action<QueryOperationMetadata> operationAction,
            bool checkOrdering = false, bool useUniqueVariables = true)
        {
            List<string> formattedOperations = new List<string>();

            if (operations == null)
            {
                return formattedOperations;
            }

            foreach (var operation in operations.Where(o => operationMetadata.ContainsKey(o.Name)))
            {
                var currentOperationMetadata = operationMetadata[operation.Name];

                operationAction(currentOperationMetadata);

                List<string> parameterNames = new List<string>();
                OrderDirections? orderDirection = null;

                foreach (var parameter in currentOperationMetadata.Parameters)
                {
                    object parameterValue;
                    if (operation.Parameters.ContainsKey(parameter.Name))
                    {
                        parameterValue = operation.Parameters[parameter.Name];
                    }
                    else if (parameter.HasDefault)
                    {
                        parameterValue = parameter.DefaultValue;
                    }
                    else
                    {
                        throw new ArgumentException($"Parameter '{parameter.Name}' is required for the '{currentOperationMetadata.Name}' operation.");
                    }

                    if (checkOrdering && parameter.Name == DapperWrappersConstants.OrderByDirectionParameter)
                    {
                        orderDirection = (OrderDirections)parameterValue;
                    }
                    else
                    {
                        var variableName = context.AddVariable(parameter.Name, parameterValue, parameter.DbType);
                        parameterNames.Add(variableName);
                    }
                }

                formattedOperations.Add(formatOperation(currentOperationMetadata.BaseQueryString, parameterNames, orderDirection));
                
            }

            return formattedOperations;
        }

        protected void NoopOperationAction(QueryOperationMetadata metadata) { }
    }
}
