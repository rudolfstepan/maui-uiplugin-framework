using ModuleContracts;
using Microsoft.Maui.Controls;
using System.Reflection;

namespace PluginModule
{
    public class SampleUIPluginModule : IAppModule
    {
        public string Id => "SampleUIPlugin";

        public void Initialize(IModuleHost host)
        {
            // Migrationen direkt aus der Plugin‐Assembly laden
            var pluginAssembly = typeof(SampleUIPluginModule).Assembly;
            var db = host.RegisterDatabase(Id, pluginAssembly);

            // Route und Flyout‐Eintrag anlegen
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
