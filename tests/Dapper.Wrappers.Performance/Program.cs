using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;

namespace Dapper.Wrappers.Performance
{
    class Program
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async Task Main(string[] args)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            //var testClass = new QueryContextPerformance();

            //testClass.GlobalSetup();

            //await testClass.Baseline();

            //await testClass.UsingMemoryInsteadOfJoin();

            var summary = BenchmarkRunner.Run<QueryContextPerformance>();

            Console.WriteLine(summary);
            Console.ReadLine();
        }
    }
}
