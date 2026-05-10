using Allure.Net.Commons;
using Reqnroll;

namespace playwrightreqnroll.Hooks
{
    // Minimal hook to confirm allure-results directory creation in CI
    [Binding]
    public class AllureDiagnostics
    {
        [BeforeTestRun]
        public static void EnsureAllureResultsDirectory()
        {
            try
            {
                var dir = Environment.GetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY") ?? "allure-results";
                Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "ci-debug.txt"), $"Created by EnsureAllureResultsDirectory at {DateTime.UtcNow:O}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AllureDiagnostics] Could not create allure-results directory: {ex}");
            }
        }
    }

    [Binding]
    public class AllureHook
    {
        private readonly ScenarioContext _scenarioContext;
        public AllureHook(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [AfterScenario(Order = 1)]
        public void AfterScenario()
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
                        screenshotBytes,
                        ".png"
                    );
                    Console.WriteLine($"[AllureHook] Screenshot attached: {screenshotPath}");
                }
                else
                {
                    Console.WriteLine($"[AllureHook] Screenshot not found: {screenshotPath}");
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
                        videoBytes,
                        ".webm"
                    );
                    Console.WriteLine($"[AllureHook] Video attached: {videoPath}");
                }
                else
                {
                    Console.WriteLine($"[AllureHook] Video not found: {videoPath}");
                }
            }
            // Allure.Reqnroll adapter manages the test lifecycle. Manual StopTestCase/WriteTestCase removed.
        }

        private static (string displayName, string exampleValues) GetDisplayNameAndExampleValues(ScenarioContext context)
        {
           var title = context.ScenarioInfo.Title;
            var arguments = context.ScenarioInfo.Arguments;
            var exampleValues = arguments != null && arguments.Count > 0
                ? string.Join("_", arguments.Values.Cast<object>().Select(v => v.ToString()))
                : "";
            var displayName = !string.IsNullOrEmpty(exampleValues)
                ? title.Replace("<product>", exampleValues)
                : title;
            return (displayName, exampleValues);
        }
    }
}