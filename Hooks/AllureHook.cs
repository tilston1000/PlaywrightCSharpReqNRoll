using Reqnroll;
using Allure.Net.Commons;
using System.Diagnostics;

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