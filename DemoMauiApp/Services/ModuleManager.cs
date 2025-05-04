using Microsoft.Maui.Storage;

using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace DemoMauiApp.Services
{
    public class ModuleManager
    {
        private readonly HttpClient _httpClient;
        private readonly string _modulesFolder;
        private readonly string _remoteUrl = "https://example.com/PluginModule.zip";

        public ModuleManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _modulesFolder = Path.Combine(FileSystem.AppDataDirectory, "Modules");
            Directory.CreateDirectory(_modulesFolder);
        }

        public async Task SyncAsync()
        {
            try
            {
                using var stream = await _httpClient.GetStreamAsync(_remoteUrl);
                using var archive = new ZipArchive(stream);
                archive.ExtractToDirectory(_modulesFolder, true);
            }
            catch
            {
                // ignore errors
            }
        }
    }
}