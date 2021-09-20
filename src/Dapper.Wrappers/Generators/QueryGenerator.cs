﻿// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Wrappers.OperationFormatters;

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
        protected IQueryOperationFormatter QueryFormatter { get; }

        protected QueryGenerator(IQueryOperationFormatter queryFormatter)
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
            Func<string, IEnumerable<string>, OrderDirections?, string> formatOperation, Action<TOpMetadata, int, bool> operationAction,
            bool checkOrdering = false, bool useUniqueVariables = true, bool isFirstOperationList = true)
            where TOpMetadata : QueryOperationMetadata
        {
            List<string> formattedOperations = new List<string>();

            if (operations == null)
            {
                return formattedOperations;
            }

            var index = 0;

            foreach (var operation in operations.Where(o => operationMetadata.ContainsKey(o.Name)))
            {
                var currentOperationMetadata = operationMetadata[operation.Name];

                operationAction(currentOperationMetadata, index++, isFirstOperationList);

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

                    if (checkOrdering &&
                        parameter.Name.ToLowerInvariant() == DapperWrappersConstants.OrderByDirectionParameter &&
                        Enum.TryParse(parameterValue.ToString(), true, out OrderDirections parsedDirection))
                    {
                        orderDirection = parsedDirection;
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
        /// <param name="index">The index of the currently processed operation.</param>
        protected void NoopOperationAction(QueryOperationMetadata metadata, int index, bool firstList) { }

        protected Func<string, IEnumerable<string>, OrderDirections?, string> GetNonOrderingFormatOperation(
            Func<string, IEnumerable<string>, string> operation)
        {
            return (string operationString, IEnumerable<string> variableNames, OrderDirections? orderDirection) =>
                operation(operationString, variableNames);
        }
    }
}
