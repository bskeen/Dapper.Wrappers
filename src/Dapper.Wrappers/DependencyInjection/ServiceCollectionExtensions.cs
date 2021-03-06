﻿// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using Dapper.Wrappers.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dapper.Wrappers.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to help with setting up Dapper.Wrappers with .Net Core's built-in dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// A map of supported databases to the type that should be used with that database.
        /// </summary>
        private static readonly IDictionary<SupportedDatabases, Type> _queryFormatterTypes = new Dictionary<SupportedDatabases, Type>
        {
            { SupportedDatabases.SqlServer, typeof(SqlServerQueryFormatter) },
            { SupportedDatabases.PostgreSQL, typeof(PostgresQueryFormatter) }
        };

        /// <summary>
        /// Adds the default types to the provided IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection that will be updated with the default types.</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddDapperWrappers(this IServiceCollection services)
        {
            SetupDependencyInjection(services, new DapperWrappersOptions());

            return services;
        }

        /// <summary>
        /// Adds the types necessary for using Dapper.Wrappers, while allowing these types to be configured.
        /// </summary>
        /// <param name="services">The IServiceCollection that will be updated with the requested types.</param>
        /// <param name="setupAction">An action which configures the options for setting up Dapper.Wrappers.</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddDapperWrappers(this IServiceCollection services,
            Action<DapperWrappersOptions> setupAction)
        {
            var options = new DapperWrappersOptions();
            setupAction(options);

            SetupDependencyInjection(services, options);

            return services;
        }

        /// <summary>
        /// A helper method that performs the work to update the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection that will be updated.</param>
        /// <param name="options">The options configuring how the IServiceCollection will be updated.</param>
        private static void SetupDependencyInjection(IServiceCollection services, DapperWrappersOptions options)
        {
            if (_queryFormatterTypes.ContainsKey(options.DatabaseEngine))
            {
                services.TryAddSingleton(typeof(IQueryFormatter), _queryFormatterTypes[options.DatabaseEngine]);
            }

            services.TryAddScoped(typeof(IQueryContext), options.QueryContextType);
            services.TryAddSingleton(typeof(IQueryResultsProcessorProvider), options.QueryResultsProcessorProviderType);
            if (options.DbConnectionType != null)
            {
                services.TryAddScoped(typeof(IDbConnection), options.DbConnectionType);
            }
        }
    }
}
