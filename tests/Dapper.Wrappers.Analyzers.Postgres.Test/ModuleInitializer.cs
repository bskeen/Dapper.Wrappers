using System.Runtime.CompilerServices;
using VerifyTests;

namespace Dapper.Wrappers.Analyzers.Postgres.Test
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifySourceGenerators.Enable();
        }
    }
}
