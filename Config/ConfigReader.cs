using Microsoft.Extensions.Configuration;

public static class ConfigReader
{
    public static TestSettings Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var settings = config.Get<TestSettings>();

        // Override with environment variable if present
        var headlessEnv = Environment.GetEnvironmentVariable("HEADLESS");
        if(!string.IsNullOrEmpty(headlessEnv))
        {
            settings.Headless = headlessEnv.ToLower() == "true";
        }

        return settings;
    }
}