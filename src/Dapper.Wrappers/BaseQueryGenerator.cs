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
        protected abstract IDictionary<string, IDictionary<FilterOperations, string>> FilterItemStrings { get; }
        protected abstract IDictionary<string, IDictionary<OrderDirections, string>> OrderItemStrings { get; }

        public BaseQueryGenerator(IQueryResultsProcessorProvider resultsProcessorProvider, IQueryFormatter queryFormatter)
        {
            _resultsProcessorProvider = resultsProcessorProvider;
            _queryFormatter = queryFormatter;
        }

        public virtual IQueryResultsProcessor<M> AddGetQuery(IQueryContext context, IEnumerable<FilterItem> filterItems = null, IEnumerable<OrderItem> orderItems = null,
            Pagination pagination = null)
        {
            throw new NotImplementedException();
        }

        public virtual IQueryResultsProcessor<M> AddInsertQuery(IQueryContext context, IEnumerable<IEnumerable<QueryKeyValue>> entityValues)
        {
            throw new NotImplementedException();
        }

        public IQueryResultsProcessor<M> AddUpdateQuery(IQueryContext context, IEnumerable<QueryKeyValue> entityUpdates, IEnumerable<FilterItem> filterItems)
        {
            throw new NotImplementedException();
        }

        public void AddDeleteQuery(IQueryContext context, IEnumerable<FilterItem> deleteCriteria)
        {
            throw new NotImplementedException();
        }

        protected virtual string FormatFilterItems(IQueryContext context, IEnumerable<FilterItem> filterItems, bool isUnique = true)
        {
            if (filterItems == null)
            {
                return string.Empty;
            }

            List<string> formattedFilterItems = new List<string>();

            foreach (var filterItem in filterItems)
            {
                var variableName = context.AddVariable(filterItem.KeyName, filterItem.Value, filterItem.ValueType, isUnique);
                formattedFilterItems.Add(_queryFormatter.FormatFilterItem(FilterItemStrings[filterItem.KeyName][filterItem.Operation], variableName));
            }

            if (formattedFilterItems.Count == 0)
            {
                return string.Empty;
            }

            return _queryFormatter.FormatFilterItems(formattedFilterItems);
        }

        protected virtual string FormatOrderItems(IEnumerable<OrderItem> orderItems)
        {
            if (orderItems == null)
            {
                return string.Empty;
            }

            List<string> formattedOrderItems = new List<string>();

            foreach (var orderItem in orderItems)
            {
                formattedOrderItems.Add(OrderItemStrings[orderItem.KeyName][orderItem.Direction]);
            }

            return _queryFormatter.FormatOrderItems(formattedOrderItems);
        }
    }
}
