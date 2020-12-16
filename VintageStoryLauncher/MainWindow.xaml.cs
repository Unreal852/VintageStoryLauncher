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

        public string Message
        {
            get => this.FindControl<TextBlock>("MessageBlock").Text;
            set { this.FindControl<TextBlock>("MessageBlock").Text = value; }
        }

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
                Message = "Failed to load website";
                return;
            }

            AuthResult result = await WebManager.Login(Email, Password);
            if (!result.Success)
                Message = result.Message;
            else
            {
                AuthInfo authInfo = await WebManager.GetAuthInfo();
                if (authInfo == default)
                    Message = "Login successful but failed to get user profile.";
                else
                    Message = $"Login successful. Welcome {authInfo.PlayerName} ({authInfo.LastName.ToUpper()} {authInfo.FirstName}, {authInfo.Email})";
            }
        }
    }
}