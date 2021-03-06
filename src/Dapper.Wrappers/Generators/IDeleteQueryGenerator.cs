﻿using System;
// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Dapper.Wrappers.Generators
{
    /// <summary>
    /// Updates the context to add all the pieces to run a get query.
    /// </summary>
    /// <typeparam name="M">The type of model being returned.</typeparam>
    public interface IDeleteQueryGenerator<M>
    {
        /// <summary>
        /// Adds a delete query to the context.
        /// </summary>
        /// <param name="context">The context to be updated.</param>
        /// <param name="deleteCriteria">The filter describing which items should be deleted.</param>
        /// <returns>The query results processor that will provide the results.</returns>
        IQueryResultsProcessor<M> AddDeleteQuery(IQueryContext context, IEnumerable<QueryOperation> deleteCriteria);
    }
}
