using Microsoft.Extensions.Configuration;

namespace playwrightreqnroll.Config
{
    public static class ConfigReader
    {
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

            return settings;
        }
    }
}