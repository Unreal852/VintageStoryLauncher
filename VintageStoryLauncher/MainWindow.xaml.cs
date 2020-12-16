using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VintageStoryLauncher.Core;
using VintageStoryLauncher.Core.Auth;

namespace VintageStoryLauncher
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WebManager = new WebManager();
        }

        private WebManager WebManager { get; }
        public  string     Email      { get; set; }
        public  string     Password   { get; set; }

        private void InitializeComponent()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);
        }

        private async void Button_OnClick(object sender, RoutedEventArgs e)
        {
            await WebManager.LoadWebsite();
            if (!WebManager.IsWebsiteLoaded)
            {
                Title = "Failed to login";
                return;
            }

            AuthResult result = await WebManager.Login(Email, Password);
            if (!result.Success)
                Title = "Failed";
            else
                Title = "Success";
        }
    }
}