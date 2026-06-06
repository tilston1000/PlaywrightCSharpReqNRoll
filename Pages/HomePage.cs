using Microsoft.Playwright;
using playwrightreqnroll.Config;
using playwrightreqnroll.Drivers;

namespace playwrightreqnroll.Pages
{
    public class HomePage
    {
        private readonly PlaywrightDriver _driver;
        private readonly TestSettings _settings;
        private IPage Page => _driver.Page ?? throw new InvalidOperationException("Playwright page is not initialized. Ensure the driver is started before using HomePage.");

        private const string UserNameSelector = "#user-name";
        private const string PasswordSelector = "#password";
        private const string LoginButtonSelector = "#login-button";
        private const string MenuButtonSelector = "#react-burger-menu-btn";
        private const string LogoutLinkSelector = "#logout_sidebar_link";


        public HomePage(PlaywrightDriver driver, TestSettings settings)
        {
            _driver = driver;
            _settings = settings;
        }

        public async Task Navigate()
        {
            if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
                throw new InvalidOperationException("BaseUrl is not configured.");

            await Page.GotoAsync(_settings.BaseUrl);
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