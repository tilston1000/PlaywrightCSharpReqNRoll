using Allure.Net.Commons;
using Reqnroll;

namespace playwrightreqnroll.Hooks
{
    [Binding]

    public class AllureHook(ScenarioContext _scenarioContext)
    {

        [BeforeScenario]
        public void BeforeScenario()
        {
            var (displayName, exampleValues) = GetDisplayNameAndExampleValues(_scenarioContext);
            var uniqueId = $"{displayName.Replace(" ", "_")}_{exampleValues.Replace(" ", "_")}";
            var tags = _scenarioContext.ScenarioInfo.Tags;

            AllureLifecycle.Instance.StartTestCase(new Allure.Net.Commons.TestResult
            {
                name = displayName,
                fullName = displayName,
                uuid = uniqueId,
                historyId = uniqueId,
                testCaseId = uniqueId,
                labels = tags.Select(t => new Label { name = "tag", value = t }).ToList()
            });
        }

        [AfterScenario(Order = 1)]
        public void AfterScenario()
        {
            if (_scenarioContext.TestError == null)
            {
                AllureLifecycle.Instance.UpdateTestCase(testCase => testCase.status = Status.passed);
            }
            else
            {
                AllureLifecycle.Instance.UpdateTestCase(testCase => 
                {
                    testCase.status = Status.failed;
                    testCase.statusDetails = new StatusDetails 
                    { 
                        message = _scenarioContext.TestError?.Message 
                    };
                });
            }

            // Attach Playwright screenshot if available
            if (_scenarioContext.ContainsKey("PlaywrightScreenshotPath"))
            {
                var screenshotPath = _scenarioContext["PlaywrightScreenshotPath"] as string;
                if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                {
                    var screenshotBytes = File.ReadAllBytes(screenshotPath);
                    AllureApi.AddAttachment("Failure Screenshot", "image/png", screenshotBytes, ".png");
                }
            }

            // Attach Playwright video if available
            if (_scenarioContext.ContainsKey("PlaywrightVideoPath"))
            {
                var videoPath = _scenarioContext["PlaywrightVideoPath"] as string;
                if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
                {
                    var videoBytes = File.ReadAllBytes(videoPath);
                    AllureApi.AddAttachment("Test Video", "video/webm", videoBytes, ".webm");
                }
            }

            AllureLifecycle.Instance.StopTestCase();
            AllureLifecycle.Instance.WriteTestCase();
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