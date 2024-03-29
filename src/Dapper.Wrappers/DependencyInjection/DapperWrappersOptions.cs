﻿// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace Dapper.Wrappers.DependencyInjection
{
    /// <summary>
    /// Options used to customize the services available through dependency injection
    /// in .Net Core.
    /// </summary>
    public class DapperWrappersOptions
    {
        public DapperWrappersOptions()
        {
            DatabaseEngine = SupportedDatabases.SqlServer;
            QueryContextType = typeof(QueryContext);
        }

        /// <summary>
        /// Configures which database engine should be assumed by the query building classes.
        /// </summary>
        public SupportedDatabases DatabaseEngine { get; set; }

        /// <summary>
        /// Configures which type to inject as an IQueryContext.
        /// </summary>
        public Type QueryContextType { get; set; }

        /// <summary>
        /// Configures which type to inject as an IDbConnection to the QueryContext.
        /// </summary>
        public Type DbConnectionType { get; set; }

        /// <summary>
        /// Configures which assembly should be scanned for generators to automatically add.
        /// </summary>
        public Assembly QueryBuilderTypeAssembly { get; set; }
    }
}
