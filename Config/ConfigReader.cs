using Microsoft.Extensions.Configuration;

namespace playwrightreqnroll.Config
{
    public static class ConfigReader
    {
        private static readonly HashSet<string> SupportedBrowsers = new(StringComparer.OrdinalIgnoreCase)
        {
            "chromium",
            "firefox",
            "webkit"
        };

        public static TestSettings Load()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var settings = config.Get<TestSettings>();
            if (settings == null)
                throw new InvalidOperationException("Failed to load TestSettings from appsettings.json.");

            var headlessEnv = Environment.GetEnvironmentVariable("HEADLESS");
            if(!string.IsNullOrEmpty(headlessEnv))
            {
                settings.Headless = string.Equals(headlessEnv, "true", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                var isCi = string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase);
                if (isCi)
                {
                    settings.Headless = true;
                }
            }

            var slowMoEnv = Environment.GetEnvironmentVariable("SlowMo");
            if(!string.IsNullOrEmpty(slowMoEnv) && int.TryParse(slowMoEnv, out var slowMoValue))
            {
                settings.SlowMo = slowMoValue;
            }

            var videoRetentionDaysEnv = Environment.GetEnvironmentVariable("VideoRetentionDays");
            if(!string.IsNullOrEmpty(videoRetentionDaysEnv) && int.TryParse(videoRetentionDaysEnv, out var videoRetentionDaysValue))
            {
                settings.VideoRetentionDays = videoRetentionDaysValue;
            }

            var browserEnv = Environment.GetEnvironmentVariable("BROWSER");
            if (!string.IsNullOrWhiteSpace(browserEnv))
            {
                settings.Browser = browserEnv.Trim();
            }

            if (!SupportedBrowsers.Contains(settings.Browser))
            {
                throw new InvalidOperationException(
                    $"Unsupported browser '{settings.Browser}'. Supported values: chromium, firefox, webkit.");
            }

            settings.Browser = settings.Browser.ToLowerInvariant();

            return settings;
        }
    }
}