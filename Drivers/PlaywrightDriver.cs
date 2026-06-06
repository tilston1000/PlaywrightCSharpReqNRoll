using Microsoft.Playwright;

namespace playwrightreqnroll.Drivers
{
    public class PlaywrightDriver : IAsyncDisposable, IDisposable
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        public IPage? Page {get; private set;}

        public async Task StartAsync(bool headless, string? videoDir = null, int timeout = 5000, int slowMo = 300)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = headless,
                SlowMo = slowMo
            });


            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = videoDir
            });

            _context.SetDefaultTimeout(timeout);

            Page = await _context.NewPageAsync();
        }


        public async ValueTask DisposeAsync()
        {
            if (Page != null) await Page.CloseAsync();
            if (_context != null) await _context.CloseAsync();
            if (_browser != null) await _browser.CloseAsync();
            _playwright?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            // No-op. Required for compatibility with DI containers that expect IDisposable.
        }
    }
}
