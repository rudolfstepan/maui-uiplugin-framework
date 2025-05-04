using ModuleContracts;
using Microsoft.Maui.Controls;
using System;
using System.IO.Compression;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using DemoMauiApp.Services;

namespace DemoMauiApp
{
    public partial class MainPage : ContentPage
    {
        private readonly ModuleManager _moduleManager;
        private readonly PluginLoader _pluginLoader;

        public MainPage(ModuleManager moduleManager, PluginLoader pluginLoader)
        {
            InitializeComponent();
            _moduleManager = moduleManager;
            _pluginLoader = pluginLoader;
        }

        private async void OnLoadPluginClicked(object sender, EventArgs e)
        {
            await _moduleManager.SyncAsync();
            _pluginLoader.LoadAll();
            await DisplayAlert("Plugin", "Plugin loaded!", "OK");
        }

        private async void OnGoToPluginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"plugin/SampleUIPlugin");
        }
    }
}