using Microsoft.Maui.Controls;

namespace PluginModule
{
    public class PluginPage : ContentPage
    {
        public PluginPage()
        {
            Title = "Plugin Page";
            Content = new VerticalStackLayout
            {
                Children =
                {
                    new Label { Text = "Hello from the remote UI plugin!", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
                }
            };
        }
    }
}