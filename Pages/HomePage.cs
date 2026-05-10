using Microsoft.Playwright;
using playwrightreqnroll.Drivers;
using playwrightreqnroll.Hooks;

namespace playwrightreqnroll.Pages
{
    public class HomePage
    {
        private readonly PlaywrightDriver _driver;
        private IPage Page => _driver.Page!;

        private const string UserNameSelector = "#user-name";
        private const string PasswordSelector = "#password";
        private const string LoginButtonSelector = "#login-button";
        private const string MenuButtonSelector = "#react-burger-menu-btn";
        private const string LogoutLinkSelector = "#logout_sidebar_link";


        public HomePage(PlaywrightDriver driver)
        {
            _driver = driver;
            if (_driver.Page == null)
                throw new InvalidOperationException("Playwright page is not initialized. Ensure the driver is started before using HomePage.");
        }

        public async Task Navigate()
        {
            if (PlaywrightHooks.Settings == null || string.IsNullOrWhiteSpace(PlaywrightHooks.Settings.BaseUrl))
                throw new InvalidOperationException("BaseUrl is not configured.");

            await Page.GotoAsync(PlaywrightHooks.Settings.BaseUrl);
        }

        public async Task Login(string userName, string password)
        {
            await Page.FillAsync(UserNameSelector, userName);
            await Page.FillAsync(PasswordSelector, password);
            await Page.ClickAsync(LoginButtonSelector);
        }

        public async Task Logout()
        {
            await Page.ClickAsync(MenuButtonSelector);
            await Page.ClickAsync(LogoutLinkSelector);
        }
    }
}