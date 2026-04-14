using Microsoft.Playwright;

public class PlaywrightDriver
{
    public IPlaywright Playwright { get; private set; }
    public IBrowser Browser { get; private set; }
    public IBrowserContext Context { get; private set; }
    public IPage Page {get; private set;}

    public async Task StartAsync(bool headless)
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = headless
        });

        Context = await Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Browser?.CloseAsync();
    }

}