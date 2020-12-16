using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using VintageStoryLauncher.Core.Auth;

namespace VintageStoryLauncher.Core
{
    public class WebManager
    {
        public const string Url                           = "https://vintagestory.at/";
        public const string AccountUrl                    = "https://account.vintagestory.at/";
        public const string ProfileUrl                    = "https://account.vintagestory.at/profile";
        public const string HtmlAttributeLoginForm        = "action";
        public const string HtmlAttributeProfileForm      = "action";
        public const string HtmlAttributeLoginFormValue   = "attemptlogin";
        public const string HtmlAttributeProfileFormValue = "/profile";
        public const string HtmlAttributeEmailField       = "email";
        public const string HtmlAttributeFirstNameField   = "firstname";
        public const string HtmlAttributeLastNameField    = "lastname";
        public const string HtmlAttributePlayerNameField  = "playername";
        public const string HtmlAttributePasswordField    = "password";
        public const string HtmlAttributeUrl              = "href";
        public const string HtmlClassStableVersions       = "tabpane stable";
        public const string HtmlClassUnstableVersions     = "tabpane unstable";
        public const string HtmlClassLoginError           = "errormessage";
        public const string HtmlTagVersions               = "p";
        public const string HtmlTagVersionNumber          = "b";
        public const string HtmlTagVersionUrl             = "a";

        public WebManager()
        {
        }

        public IBrowsingContext Context         { get; private set; }
        public IDocument        AccountPage     { get; private set; }
        public IDocument        ProfilePage     { get; private set; }
        public IHtmlFormElement LoginForm       { get; private set; }
        public IHtmlFormElement ProfileForm     { get; private set; }
        public bool             IsLogged        { get; private set; } = false;
        public bool             IsWebsiteLoaded => AccountPage != null && LoginForm != null;

        public async Task LoadWebsite()
        {
            Context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
            AccountPage = await Context.OpenAsync(AccountUrl);
            if (AccountPage == null)
                return;
            LoginForm = AccountPage.Forms?.FirstOrDefault(element =>
                element.HasAttribute(HtmlAttributeLoginForm) && element.GetAttribute(HtmlAttributeLoginForm) == HtmlAttributeLoginFormValue);
        }

        public async Task<AuthResult> Login(string email, string password)
        {
            if (!IsWebsiteLoaded)
                return new AuthResult(false, 1, "Website and/or Login form didn't load properly.");
            AccountPage = await LoginForm.SubmitAsync(new Dictionary<string, string>()
            {
                {HtmlAttributeEmailField, email},
                {HtmlAttributePasswordField, password}
            });
            IElement loginError = AccountPage.GetElementsByClassName(HtmlClassLoginError).FirstOrDefault();
            if (loginError == default)
            {
                IsLogged = true;
                return new AuthResult(true, 0);
            }

            return new AuthResult(false, 2, loginError.Text());
        }

        public async Task<AuthInfo> GetAuthInfo()
        {
            if (!IsWebsiteLoaded || !IsLogged)
                return default;
            if (ProfilePage == null)
            {
                ProfilePage = await Context.OpenAsync(ProfileUrl);
                ProfileForm = ProfilePage.Forms?.FirstOrDefault(element =>
                    element.HasAttribute(HtmlAttributeProfileForm) && element.GetAttribute(HtmlAttributeProfileForm) == HtmlAttributeProfileFormValue);
            }

            if (ProfileForm == default)
                return null;

            string email = null;
            string firstName = null;
            string lastName = null;
            string playerName = null;
            foreach (IHtmlElement element in ProfileForm.Elements)
            {
                if (!element.HasAttribute("name"))
                    continue;
                switch (element.GetAttribute("name"))
                {
                    case HtmlAttributeEmailField:
                        email = element.GetAttribute("value");
                        break;
                    case HtmlAttributeFirstNameField:
                        firstName = element.GetAttribute("value");
                        break;
                    case HtmlAttributeLastNameField:
                        lastName = element.GetAttribute("value");
                        break;
                    case HtmlAttributePlayerNameField:
                        playerName = element.GetAttribute("value");
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) &&
                !string.IsNullOrWhiteSpace(playerName))
                return new AuthInfo(email, firstName, lastName, playerName);
            return default;
        }
    }
}