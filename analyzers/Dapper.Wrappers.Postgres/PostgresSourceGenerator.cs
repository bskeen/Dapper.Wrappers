using Dapper.Wrappers.Analyzers.Common;
using Microsoft.CodeAnalysis;

namespace Dapper.Wrappers.Postgres
{
    public class PostgresSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.AddTrackUpdatesClasses();
        }
    }
}
