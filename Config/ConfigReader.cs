using Microsoft.Extensions.Configuration;

public static class ConfigReader
{
    public static TestSettings Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        return config.Get<TestSettings>();
    }
}