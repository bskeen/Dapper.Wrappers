// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper.Wrappers.Builders;
using Dapper.Wrappers.OperationFormatters;
using Dapper.Wrappers.QueryFormatters;
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
        private static readonly IDictionary<SupportedDatabases, Type> QueryOperationFormatterTypes = new Dictionary<SupportedDatabases, Type>
        {
            { SupportedDatabases.SqlServer, typeof(SqlServerQueryOperationFormatter) },
            { SupportedDatabases.PostgreSQL, typeof(PostgresQueryOperationFormatter) }
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
            if (QueryOperationFormatterTypes.ContainsKey(options.DatabaseEngine))
            {
                services.TryAddSingleton(typeof(IQueryOperationFormatter), QueryOperationFormatterTypes[options.DatabaseEngine]);
            }

            services.TryAddScoped(typeof(IQueryContext), options.QueryContextType);
            services.TryAddSingleton<IMetadataGenerator, MetadataGenerator>();

            if (options.DbConnectionType != null)
            {
                services.TryAddScoped(typeof(IDbConnection), options.DbConnectionType);
            }

            if (options.QueryBuilderTypeAssembly != null)
            {
                RegisterQueryBuilders(services, options.QueryBuilderTypeAssembly);
            }

            RegisterQueryFormatters(services);
        }

        /// <summary>
        /// Registers any QueryBuilders found in the given assembly.
        /// </summary>
        /// <param name="services">The service collection with which to register the query builder types.</param>
        /// <param name="assembly">The assembly containing the types to register.</param>
        private static void RegisterQueryBuilders(IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes();

            var genericQueryBuilders = new HashSet<Type>(new[]
            {
                typeof(IQueryBuilder<>),
                typeof(IQueryBuilder<,>),
                typeof(IQueryBuilder<,,>),
                typeof(IQueryBuilder<,,,>),
                typeof(IQueryBuilder<,,,,>),
                typeof(IQueryBuilder<,,,,,>),
                typeof(IQueryBuilder<,,,,,,>),
                typeof(IQueryBuilder<,,,,,,,>),
                typeof(IQueryBuilder<,,,,,,,,>),
                typeof(IQueryBuilder<,,,,,,,,,>)
            });

            foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract))
            {
                var interfaceToRegister = type.GetInterfaces().FirstOrDefault(i =>
                    i.IsGenericType && genericQueryBuilders.Contains(i.GetGenericTypeDefinition()));

                if (interfaceToRegister != null)
                {
                    services.TryAddSingleton(interfaceToRegister, type);
                }
            }
        }

        /// <summary>
        /// Registers the default query formatters.
        /// </summary>
        /// <param name="services">The services collection with which to register the query formatter types.</param>
        private static void RegisterQueryFormatters(IServiceCollection services)
        {
            services.TryAddSingleton<IFilterFormatter, FilterFormatter>();
            services.TryAddSingleton<IInsertFormatter, InsertFormatter>();
            services.TryAddSingleton<IOrderingFormatter, OrderingFormatter>();
            services.TryAddSingleton<IUpdateFormatter, UpdateFormatter>();
        }
    }
}
