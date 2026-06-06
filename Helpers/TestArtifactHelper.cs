using playwrightreqnroll.Config;
using playwrightreqnroll.Drivers;
using Reqnroll;
using System.Diagnostics;

namespace playwrightreqnroll.Helpers;

public class TestArtifactHelper(TestSettings settings)
{
    private readonly TestSettings _settings = settings;

    public async Task CaptureScreenshotOnFailureAsync(ScenarioContext scenarioContext, PlaywrightDriver driver)
    {
        if (scenarioContext.TestError == null || driver.Page == null)
            return;

        var (safeScenarioName, safeProductName) = GetSafeNames(scenarioContext);
        var screenshotPath = GetScreenshotPath(safeScenarioName, safeProductName);

        var dir = Path.GetDirectoryName(screenshotPath);
        if (dir != null)
            Directory.CreateDirectory(dir);

        try
        {
            await driver.Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
            scenarioContext["PlaywrightScreenshotPath"] = screenshotPath;
        }
        catch (Exception ex)
        {
            Trace.TraceWarning($"[WARN] Failed to capture screenshot for scenario '{scenarioContext.ScenarioInfo.Title}': {ex.Message}");
        }
    }

    public async Task HandleVideoAsync(ScenarioContext scenarioContext, PlaywrightDriver driver)
    {
        var page = driver.Page;
        if (page?.Video == null)
            return;

        try
        {
            if (!page.IsClosed)
                await page.CloseAsync();

            var videoPath = await page.Video.PathAsync();
            scenarioContext["PlaywrightVideoPath"] = videoPath;
        }
        catch (Exception ex)
        {
            Trace.TraceWarning($"[WARN] Failed to process Playwright video for scenario '{scenarioContext.ScenarioInfo.Title}': {ex.Message}");
        }
    }

    public string GetVideoDirectory(ScenarioContext scenarioContext)
    {
        var (safeScenarioName, safeProductName) = GetSafeNames(scenarioContext);
        var timestamp = DateTime.UtcNow.ToString("ddMMyyyy_HHmmss");
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", _settings.VideosDirectory, $"{safeScenarioName}_{safeProductName}_{timestamp}"));
    }

    private string GetScreenshotPath(string safeScenarioName, string safeProductName)
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", _settings.ScreenshotsDirectory, $"{safeScenarioName}_{safeProductName}_{DateTime.UtcNow:ddMMyyyy_HHmmss}.png"));
    }

    private static (string SafeScenarioName, string SafeProductName) GetSafeNames(ScenarioContext scenarioContext)
    {
        var safeScenarioName = GetSafeScenarioName(scenarioContext.ScenarioInfo.Title);
        scenarioContext.TryGetValue("ProductName", out var productNameObj);
        var safeProductName = productNameObj is string name && !string.IsNullOrWhiteSpace(name) ? GetSafeScenarioName(name) : "";
        return (safeScenarioName, safeProductName);
    }

    private static string GetSafeScenarioName(string scenarioName)
    {
        var safeName = string.Concat(scenarioName.Split(Path.GetInvalidFileNameChars()));
        safeName = safeName.Replace("<", "").Replace(">", "");
        return safeName;
    }
}