using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Wrappers.Abstraction
{
    public interface IConnectionWrapper : IDisposable
    {
        Task<SqlMapper.GridReader> QueryMultipleAsync(string query, DynamicParameters parameters);
        Task ExecuteAsync(string command, DynamicParameters parameters);
        void CommitTransaction();

    }
}
