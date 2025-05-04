using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using ModuleContracts;
using SampleMauiApp.Services;
using System.Net.Http;
using Microsoft.Maui.Storage;

namespace SampleMauiApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<IModuleHost>(sp => new ModuleHost(Path.Combine(FileSystem.AppDataDirectory, "global.db")));
            builder.Services.AddSingleton<ModuleManager>();
            builder.Services.AddSingleton<PluginLoader>();

            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}