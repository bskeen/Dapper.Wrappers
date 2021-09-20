// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Dapper.Wrappers.OperationFormatters;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run a get query.
    /// </summary>
    public abstract class GetQueryGenerator : FilterableQueryGenerator, IGetQueryGenerator
    {
        /// <summary>
        /// Returns the base get query string.
        /// </summary>
        protected abstract string GetQueryString { get; }

        /// <summary>
        /// Returns the default ordering.
        ///
        /// Used if no ordering is specified, but pagination values are.
        /// </summary>
        protected abstract string DefaultOrdering { get; }

        /// <summary>
        /// Specifies the possible ordering operations to be applied to the results.
        /// </summary>
        protected abstract IDictionary<string, QueryOperationMetadata> OrderOperationMetadata { get; }

        protected GetQueryGenerator(IQueryOperationFormatter queryFormatter)
            : base(queryFormatter)
        {
        }

        /// <summary>
        /// Adds a get query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="filterOperations">Declares how to filter the results.</param>
        /// <param name="orderOperations">Declares how to order the results.</param>
        /// <param name="pagination">Declares how to paginate the results</param>
        /// <returns>The query results processor that will provide the results.</returns>
        public virtual void AddGetQuery(IQueryContext context,
            IEnumerable<QueryOperation> filterOperations = null, IEnumerable<QueryOperation> orderOperations = null,
            Pagination pagination = null)
        {
            string skipVariable = null;
            string takeVariable = null;

            if (pagination != null)
            {
                skipVariable = context.AddVariable("skip", pagination.Skip, System.Data.DbType.Int32);
                takeVariable = context.AddVariable("take", pagination.Take, System.Data.DbType.Int32);
            }

            string criteria = FormatFilterOperations(context, filterOperations);

            var formattedOrderItems = FormatOperations(context, orderOperations, OrderOperationMetadata,
                QueryFormatter.FormatOrderOperation, NoopOperationAction, true);

            string ordering;

            if (formattedOrderItems.Count == 0)
            {
                ordering = pagination is null ? string.Empty : DefaultOrdering;
            }
            else
            {
                ordering = QueryFormatter.FormatOrderOperations(formattedOrderItems);
            }

            var query = QueryFormatter.FormatGetQuery(GetQueryString, criteria, ordering, pagination != null,
                skipVariable, takeVariable);

            context.AddQuery(query);
        }
    }
}
