using Reqnroll;
using Allure.Net.Commons;

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
                        videoBytes
                    );
                    Console.WriteLine($"[AllureHook] Video attached: {videoPath}");
                }
                else
                {
                    Console.WriteLine($"[AllureHook] Video not found: {videoPath}");
                }
            }

            // Allure diagnostics: print loaded plugin info and allure-results contents
            try
            {
                Console.WriteLine($"[AllureHook][DIAG] AllureLifecycle.Instance: {AllureLifecycle.Instance}");
                var allureResultsDir = "/app/allure-results";
                if (Directory.Exists(allureResultsDir))
                {
                    var files = Directory.GetFiles(allureResultsDir);
                    Console.WriteLine($"[AllureHook][DIAG] allure-results contains {files.Length} files after scenario.");
                    foreach (var file in files)
                    {
                        Console.WriteLine($"[AllureHook][DIAG] allure-results file: {file}");
                    }
                }
                else
                {
                    Console.WriteLine($"[AllureHook][DIAG] allure-results directory does not exist after scenario.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AllureHook][DIAG] Error printing allure-results diagnostics: {ex.Message}");
            }
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