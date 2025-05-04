using ModuleContracts;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleMauiApp.Services
{
    public class OrmModuleDatabase : IModuleDatabase
    {
        private readonly SqliteConnection _conn;
        private readonly string _prefix;

        public OrmModuleDatabase(SqliteConnection conn, string moduleId)
        {
            _conn = conn;
            _prefix = moduleId + "_";
        }

        private string Apply(string sql) => sql.Replace("{prefix}_", _prefix);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
            => _conn.QueryAsync<T>(Apply(sql), parameters);

        public Task ExecuteAsync(string sql, object? parameters = null)
            => _conn.ExecuteAsync(Apply(sql), parameters);
    }
}