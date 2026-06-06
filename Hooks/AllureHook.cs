using Reqnroll;
using Allure.Net.Commons;
using System.Diagnostics;
using playwrightreqnroll.Config;

namespace playwrightreqnroll.Hooks
{
    [Binding]
    public class AllureHook
    {
        private readonly ScenarioContext _scenarioContext;
        public AllureHook(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario(Order = 1)]
        public void AddBrowserMetadata()
        {
            var browser = Environment.GetEnvironmentVariable("BROWSER");
            if (string.IsNullOrWhiteSpace(browser))
            {
                try
                {
                    browser = ConfigReader.Load().Browser;
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"[AllureHook] Failed to resolve browser from config: {ex.Message}");
                    return;
                }
            }

            browser = browser.Trim().ToLowerInvariant();

            try
            {
                AllureLifecycle.Instance.UpdateTestCase(testResult =>
                {
                    testResult.parameters ??= new List<Parameter>();
                    var existingBrowserParameter = testResult.parameters
                        .FirstOrDefault(p => string.Equals(p.name, "browser", StringComparison.OrdinalIgnoreCase));

                    if (existingBrowserParameter is null)
                    {
                        testResult.parameters.Add(new Parameter
                        {
                            name = "browser",
                            value = browser,
                            excluded = false
                        });
                    }
                    else
                    {
                        existingBrowserParameter.value = browser;
                    }

                    testResult.labels ??= new List<Label>();
                    var existingBrowserLabel = testResult.labels
                        .FirstOrDefault(l => string.Equals(l.name, "browser", StringComparison.OrdinalIgnoreCase));

                    if (existingBrowserLabel is null)
                    {
                        testResult.labels.Add(new Label
                        {
                            name = "browser",
                            value = browser
                        });
                    }
                    else
                    {
                        existingBrowserLabel.value = browser;
                    }

                    if (!string.IsNullOrWhiteSpace(testResult.name) &&
                        !testResult.name.EndsWith($" [{browser}]", StringComparison.OrdinalIgnoreCase))
                    {
                        testResult.name = $"{testResult.name} [{browser}]";
                    }

                    if (!string.IsNullOrWhiteSpace(testResult.fullName) &&
                        !testResult.fullName.EndsWith($" [{browser}]", StringComparison.OrdinalIgnoreCase))
                    {
                        testResult.fullName = $"{testResult.fullName} [{browser}]";
                    }

                    if (!string.IsNullOrWhiteSpace(testResult.historyId))
                    {
                        testResult.historyId = $"{testResult.historyId}-{browser}";
                    }

                    if (!string.IsNullOrWhiteSpace(testResult.testCaseId))
                    {
                        testResult.testCaseId = $"{testResult.testCaseId}-{browser}";
                    }
                });
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"[AllureHook] Failed to add browser metadata: {ex.Message}");
            }
        }

        [AfterScenario(Order = 1)]
        public void GenerateVideoAndAllureDiagnostics()
        {
            // Attach Playwright screenshot if available
            if (_scenarioContext.ContainsKey("PlaywrightScreenshotPath"))
            {
                var screenshotPath = _scenarioContext["PlaywrightScreenshotPath"] as string;
                if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                {
                    var screenshotBytes = File.ReadAllBytes(screenshotPath);
                    AllureApi.AddAttachment(
                        "Failure Screenshot",
                        "image/png",
                        screenshotBytes
                    );
                }
                else
                {
                    Trace.TraceWarning($"[AllureHook] Screenshot not found: {screenshotPath}");
                }
            }

            // Attach Playwright video if available
            if (_scenarioContext.ContainsKey("PlaywrightVideoPath"))
            {
                var videoPath = _scenarioContext["PlaywrightVideoPath"] as string;
                if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
                {
                    var videoBytes = File.ReadAllBytes(videoPath);
                    AllureApi.AddAttachment(
                        "Test Video",
                        "video/webm",
                        videoBytes
                    );
                }
                else
                {
                    Trace.TraceWarning($"[AllureHook] Video not found: {videoPath}");
                }
            }
        }
    }
}