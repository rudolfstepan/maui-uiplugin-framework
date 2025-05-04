using Microsoft.Maui.Controls;

namespace DemoMauiApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}