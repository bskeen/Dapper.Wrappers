using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.Wrappers
{
    public class DefaultQueryResultsProcessorProvider : IQueryResultsProcessorProvider
    {
        private readonly IServiceProvider _services;

        public DefaultQueryResultsProcessorProvider(IServiceProvider services)
        {
            _services = services;
        }

        public IQueryResultsProcessor<M> GetQueryResultsProcessor<M>()
        {
            return _services.GetRequiredService<IQueryResultsProcessor<M>>();
        }
    }
}
