using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Dapper;
using ModuleContracts;
using System.Linq;

namespace DemoMauiApp.Services
{
    public class ModuleHost : IModuleHost
    {
        private readonly string _dbPath;

        public ModuleHost(string globalDbPath)
        {
            _dbPath = globalDbPath;
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);
        }

        public IModuleDatabase RegisterDatabase(string moduleId, string migrationsFolder)
        {
            var conn = new SqliteConnection($"Data Source={_dbPath}");
            conn.Open();

            var historyTable = $"{moduleId}_MigrationsHistory";
            conn.Execute($@"
                CREATE TABLE IF NOT EXISTS {historyTable} (
                    ScriptName TEXT PRIMARY KEY,
                    AppliedOn TEXT NOT NULL
                );");

            if (Directory.Exists(migrationsFolder))
            {
                var scripts = Directory.GetFiles(migrationsFolder, "*.sql").OrderBy(f => f);
                foreach (var file in scripts)
                {
                    var name = Path.GetFileName(file);
                    var count = conn.QuerySingle<int>($@"SELECT COUNT(1) FROM {historyTable} WHERE ScriptName = @Name", new { Name = name });
                    if (count > 0) continue;
                    var sql = File.ReadAllText(file).Replace("{prefix}_", $"{moduleId}_");
                    conn.Execute(sql);
                    conn.Execute($@"INSERT INTO {historyTable}(ScriptName, AppliedOn) VALUES(@Name, @Now)", new { Name = name, Now = DateTime.UtcNow.ToString("o") });
                }
            }

            return new OrmModuleDatabase(conn, moduleId);
        }
    }
}