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
        public static readonly string Url                         = "https://vintagestory.at/";
        public static readonly string AccountUrl                  = "https://account.vintagestory.at/";
        public static readonly string HtmlAttributeLoginForm      = "action";
        public static readonly string HtmlAttributeLoginFormValue = "attemptlogin";
        public static readonly string HtmlAttributeEmailField     = "email";
        public static readonly string HtmlAttributePasswordField  = "password";
        public static readonly string HtmlAttributeUrl            = "href";
        public static readonly string HtmlClassStableVersions     = "tabpane stable";
        public static readonly string HtmlClassUnstableVersions   = "tabpane unstable";
        public static readonly string HtmlClassLoginError         = "errormessage";
        public static readonly string HtmlTagVersions             = "p";
        public static readonly string HtmlTagVersionNumber        = "b";
        public static readonly string HtmlTagVersionUrl           = "a";

        public WebManager()
        {
        }

        public IBrowsingContext Context         { get; private set; }
        public IDocument        Document        { get; private set; }
        public IHtmlFormElement LoginForm       { get; private set; }
        public bool             IsLogged        { get; private set; } = false;
        public bool             IsWebsiteLoaded => Document != null && LoginForm != null;

        public async Task LoadWebsite()
        {
            Context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
            Document = await Context.OpenAsync(AccountUrl);
            if (Document == null)
                return;
            LoginForm = Document.Forms?.FirstOrDefault(element =>
                element.HasAttribute(HtmlAttributeLoginForm) && element.GetAttribute(HtmlAttributeLoginForm) == HtmlAttributeLoginFormValue);
        }

        public async Task<AuthResult> Login(string email, string password)
        {
            if (!IsWebsiteLoaded)
                return new AuthResult(false, 1, "Website and/or Login form didn't load properly.");
            IDocument loginDoc = await LoginForm.SubmitAsync(new Dictionary<string, string>()
            {
                {HtmlAttributeEmailField, email},
                {HtmlAttributePasswordField, password}
            });
            IElement loginError = loginDoc.GetElementsByClassName(HtmlClassLoginError).FirstOrDefault();
            if (loginError == default)
            {
                IsLogged = true;
                return new AuthResult(true, 0);
            }

            return new AuthResult(false, 2, loginError.Text());
        }
    }
}