using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;

namespace Dapper.Wrappers.Performance
{
    class Program
    {
        static async Task Main(string[] args)
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
