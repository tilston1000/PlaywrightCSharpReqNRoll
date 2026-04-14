using Microsoft.Playwright;

public class HomePage
{
    private readonly IPage _page;

    public HomePage(PlaywrightDriver driver)
    {
        _page = driver.Page;
    }

    public async Task Navigate()
    {
        await _page.GotoAsync(Hooks.Settings.BaseUrl);
    }

    public async Task Login(string userName, string password)
    {
        await _page.FillAsync("#user-name", userName);
        await _page.FillAsync("#password", password);
        await _page.ClickAsync("#login-button");
    }
}