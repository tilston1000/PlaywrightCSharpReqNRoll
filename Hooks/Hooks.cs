using Reqnroll;

[Binding]
public class Hooks
{
    private readonly PlaywrightDriver _driver;
    public static TestSettings Settings;

    public Hooks(PlaywrightDriver driver)
    {
        _driver = driver;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Settings = ConfigReader.Load();
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        await _driver.StartAsync(Settings.Headless);
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        await _driver.DisposeAsync();
    }
}