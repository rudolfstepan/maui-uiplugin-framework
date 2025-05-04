using ModuleContracts;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Loader;
using System.IO;
using System.Linq;
using System;
using Microsoft.Maui.Storage;

namespace SampleMauiApp.Services
{
    public class PluginLoader
    {
        private readonly IServiceProvider _provider;
        private readonly IModuleHost _host;
        private readonly string _modulesFolder;

        public PluginLoader(IServiceProvider provider, IModuleHost host)
        {
            _provider = provider;
            _host = host;
            _modulesFolder = Path.Combine(FileSystem.AppDataDirectory, "Modules");
        }

        public void LoadAll()
        {
            foreach (var dll in Directory.GetFiles(_modulesFolder, "*.dll"))
            {
                var alc = new AssemblyLoadContext(Path.GetFileNameWithoutExtension(dll), true);
                using var fs = File.OpenRead(dll);
                var asm = alc.LoadFromStream(fs);

                var types = asm.GetTypes().Where(t => typeof(IAppModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                foreach (var type in types)
                {
                    var module = ActivatorUtilities.CreateInstance(_provider, type) as IAppModule;
                    module?.Initialize(_host);
                }
            }
        }
    }
}