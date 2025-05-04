using ModuleContracts;
using Microsoft.Maui.Controls;
using System.IO;
using Microsoft.Maui.Storage;

namespace PluginModule
{
    public class SampleUIPluginModule : IAppModule
    {
        public string Id => "SampleUIPlugin";

        public void Initialize(IModuleHost host)
        {
            var migrationsPath = Path.Combine(FileSystem.AppDataDirectory, "Modules", Id, "Migrations");
            var db = host.RegisterDatabase(Id, migrationsPath);

            Routing.RegisterRoute($"plugin/{Id}", typeof(PluginPage));
            Shell.Current.Items.Add(new FlyoutItem
            {
                Title = "Plugin UI",
                Route = $"plugin/{Id}",
                Items = { new ShellContent { ContentTemplate = new DataTemplate(typeof(PluginPage)) } }
            });
        }
    }
}