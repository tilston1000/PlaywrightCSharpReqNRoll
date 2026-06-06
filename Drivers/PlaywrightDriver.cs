using Microsoft.Playwright;

namespace playwrightreqnroll.Drivers
{
    public class PlaywrightDriver : IAsyncDisposable, IDisposable
    {
        private static readonly SemaphoreSlim BrowserInitLock = new(1, 1);
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;
        private IBrowserContext? _context;
        public IPage? Page {get; private set;}

        public async Task StartAsync(bool headless, string? videoDir = null, int timeout = 5000, int slowMo = 300)
        {
            await EnsureBrowserAsync(headless, slowMo);

            _context = await _browser!.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = videoDir
            });

            _context.SetDefaultTimeout(timeout);

            Page = await _context.NewPageAsync();
        }

        private static async Task EnsureBrowserAsync(bool headless, int slowMo)
        {
            if (_browser != null)
                return;

            await BrowserInitLock.WaitAsync();
            try
            {
                if (_browser == null)
                {
                    _playwright = await Playwright.CreateAsync();
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = headless,
                        SlowMo = slowMo
                    });
                }
            }
            finally
            {
                BrowserInitLock.Release();
            }
        }

        public static async Task DisposeSharedAsync()
        {
            await BrowserInitLock.WaitAsync();
            try
            {
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                    _browser = null;
                }

                _playwright?.Dispose();
                _playwright = null;
            }
            finally
            {
                BrowserInitLock.Release();
            }
        }


        public async ValueTask DisposeAsync()
        {
            if (Page != null && !Page.IsClosed) await Page.CloseAsync();
            if (_context != null) await _context.CloseAsync();
            Page = null;
            _context = null;
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            // No-op. Required for compatibility with DI containers that expect IDisposable.
        }
    }
}
