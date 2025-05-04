using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Dapper;
using ModuleContracts;
using System.Linq;
using System.Reflection;

namespace SampleMauiApp.Services
{
    public class ModuleHost : IModuleHost
    {
        private readonly string _dbPath;

        public ModuleHost(string globalDbPath)
        {
            _dbPath = globalDbPath;
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);
        }

        public IModuleDatabase RegisterDatabase(string moduleId, Assembly moduleAssembly)
        {
            var conn = new SqliteConnection($"Data Source={_dbPath}");
            conn.Open();

            // History-Tabelle anlegen
            var historyTable = $"{moduleId}_MigrationsHistory";
            conn.Execute($@"
            CREATE TABLE IF NOT EXISTS {historyTable} (
                ScriptName TEXT PRIMARY KEY,
                AppliedOn  TEXT NOT NULL
            );");

            // 1. Versuch: Migrations im Dateisystem
            var folder = Path.Combine(FileSystem.AppDataDirectory, "Modules", moduleId, "Migrations");
            IEnumerable<(string Name, string Sql)> scripts;

            if (Directory.Exists(folder))
            {
                scripts = Directory
                    .GetFiles(folder, "*.sql")
                    .OrderBy(Path.GetFileName)
                    .Select(f => (Path.GetFileName(f), File.ReadAllText(f)));
            }
            else
            {
                // 2. Fallback: Embedded Resources in der Assembly
                var prefix = moduleAssembly.GetName().Name + ".Migrations.";
                scripts = moduleAssembly
                    .GetManifestResourceNames()
                    .Where(r => r.StartsWith(prefix) && r.EndsWith(".sql"))
                    .OrderBy(r => r)
                    .Select(r =>
                    {
                        using var stream = moduleAssembly.GetManifestResourceStream(r)!;
                        using var rdr = new StreamReader(stream);
                        return (Name: Path.GetFileName(r), Sql: rdr.ReadToEnd());
                    });
            }

            // Skripte anwenden
            foreach (var (name, rawSql) in scripts)
            {
                var already = conn.QuerySingle<int>(
                    $@"SELECT COUNT(1) FROM {historyTable} WHERE ScriptName = @Name",
                    new { Name = name });

                if (already > 0)
                    continue;

                var sql = rawSql.Replace("{prefix}_", $"{moduleId}_");
                conn.Execute(sql);
                conn.Execute($@"
                INSERT INTO {historyTable}(ScriptName, AppliedOn)
                VALUES(@Name, @Now)",
                    new { Name = name, Now = DateTime.UtcNow.ToString("o") });
            }

            return new OrmModuleDatabase(conn, moduleId);
        }
    }
}