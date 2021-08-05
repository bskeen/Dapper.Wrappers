// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using Dapper.Wrappers.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Provides basic fo
    /// </summary>
    public abstract class QueryGenerator
    {
        /// <summary>
        /// Returns the IQueryFormatter that is passed to the constructor.
        /// </summary>
        protected IQueryFormatter QueryFormatter { get; }

        protected QueryGenerator(IQueryFormatter queryFormatter)
        {
            QueryFormatter = queryFormatter;
        }

        /// <summary>
        /// Formats the given operations using the given format function.
        /// </summary>
        /// <typeparam name="TOpMetadata">The type of QueryOperationMetadata to use while formatting the operation.</typeparam>
        /// <param name="context">The context to be updated.</param>
        /// <param name="operations">The operations to format.</param>
        /// <param name="operationMetadata">The metadata about allowed operations.</param>
        /// <param name="formatOperation">The format operation to use.</param>
        /// <param name="operationAction">An additional action to apply to each operation.</param>
        /// <param name="checkOrdering">Whether or not to check for an ordering parameter.</param>
        /// <param name="useUniqueVariables">Whether or not to add unique variables to the context.</param>
        /// <returns></returns>
        protected virtual List<string> FormatOperations<TOpMetadata>(IQueryContext context, IEnumerable<QueryOperation> operations,
            IDictionary<string, TOpMetadata> operationMetadata,
            Func<string, IEnumerable<string>, OrderDirections?, string> formatOperation, Action<TOpMetadata> operationAction,
            bool checkOrdering = false, bool useUniqueVariables = true)
            where TOpMetadata : QueryOperationMetadata
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
                        var variableName = context.AddVariable(parameter.Name, parameterValue, parameter.DbType, useUniqueVariables);
                        parameterNames.Add(variableName);
                    }
                }

                formattedOperations.Add(formatOperation(currentOperationMetadata.BaseQueryString, parameterNames, orderDirection));

            }

            return formattedOperations;
        }

        /// <summary>
        /// An action that can be used if no action is required to process the operations.
        /// </summary>
        /// <param name="metadata">The operation being processed.</param>
        protected void NoopOperationAction(QueryOperationMetadata metadata) { }

        protected Func<string, IEnumerable<string>, OrderDirections?, string> GetNonOrderingFormatOperation(
            Func<string, IEnumerable<string>, string> operation)
        {
            return (string operationString, IEnumerable<string> variableNames, OrderDirections? orderDirection) =>
                operation(operationString, variableNames);
        }
    }
}
