using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.Maui.Storage;

namespace SampleMauiApp.Services
{
    public class ModuleManager
    {
        private readonly HttpClient _httpClient;
        private readonly string _modulesFolder;
        // URL zum Modul-Manifest (JSON-Array mit Zip-URLs)
        private readonly string _manifestUrl = "https://www.webstart.at/plugins/modules.json";

        public ModuleManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _modulesFolder = Path.Combine(FileSystem.AppDataDirectory, "Modules");
            Directory.CreateDirectory(_modulesFolder);
        }

        /// <summary>
        /// Standard-Sync: lädt alle Module vom Manifest-Endpoint.
        /// </summary>
        public async Task SyncAsync()
        {
            await SyncFromEndpointAsync();
        }

        /// <summary>
        /// Lädt das Modul-Manifest von der konfigurierten URL
        /// (Liste von ZIP-URLs), entpackt alle Plugins ins Module-Verzeichnis.
        /// </summary>
        public async Task SyncFromEndpointAsync()
        {
            try
            {
                // Manifest als Liste von URLs abrufen
                var moduleUrls = await _httpClient.GetFromJsonAsync<List<string>>(_manifestUrl);
                if (moduleUrls == null)
                    return;

                foreach (var moduleUrl in moduleUrls)
                {
                    // ZIP direkt aus dem Stream entpacken
                    using var stream = await _httpClient.GetStreamAsync(moduleUrl);
                    using var archive = new ZipArchive(stream);
                    archive.ExtractToDirectory(_modulesFolder, overwriteFiles: true);
                }
            }
            catch (Exception ex)
            {
                // TODO: Hier ggf. Logging hinzufügen
                System.Diagnostics.Debug.WriteLine($"ModuleSync-Fehler: {ex}");
            }
        }
    }
}
