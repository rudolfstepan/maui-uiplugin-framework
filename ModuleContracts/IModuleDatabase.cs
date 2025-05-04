using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuleContracts
{
    public interface IModuleDatabase
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
        Task ExecuteAsync(string sql, object? parameters = null);
    }
}