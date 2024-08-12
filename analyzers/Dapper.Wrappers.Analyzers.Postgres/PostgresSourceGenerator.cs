using Dapper.Wrappers.Analyzers.Common;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Analyzers.Postgres
{
    public class PostgresSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.AddTrackUpdatesClasses();
        }
    }
}
