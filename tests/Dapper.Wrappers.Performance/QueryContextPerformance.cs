using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Moq;
using Npgsql;

namespace Dapper.Wrappers.Performance
{
    [MemoryDiagnoser]
    public class QueryContextPerformance
    {
        private IDbConnection _connection;
        private QueryContext _context;
        private MemoryQueryContext _memoryContext;
        private readonly string[] _queries = new string[100];

        [GlobalSetup]
        public void GlobalSetup()
        {
            _connection = new NpgsqlConnection("User ID=docker;Password=gztVfTKVYD3ATt4ic98sBBuvNWeYrGz27TuwK2vFhhR3yPcah5;Server=localhost;Port=5432;Database=dapperwrapperstest");
            _context = new QueryContext(_connection);
            _memoryContext = new MemoryQueryContext(_connection);

            for (var i = 0; i < _queries.Length; i++)
            {
                _queries[i] = $"SELECT id FROM \"TestTable\" WHERE ID = {i};";
            }
        }

        [Benchmark(Baseline = true)]
        public async Task Baseline()
        {
            for (var i = 0; i < 1000; i++)
            {
                foreach (var query in _queries)
                {
                    _context.AddQuery(query);
                }

                await _context.ExecuteCommands();
            }
        }

        [Benchmark]
        public async Task UsingMemoryInsteadOfJoin()
        {
            for (var i = 0; i < 1000; i++)
            {
                foreach (var query in _queries)
                {
                    _context.AddQuery(query);
                }

                await _context.ExecuteCommands();
            }
        }
    }
}
