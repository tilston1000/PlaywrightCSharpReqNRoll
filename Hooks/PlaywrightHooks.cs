
using Reqnroll;
using playwrightreqnroll.Drivers;
using playwrightreqnroll.Config;
using playwrightreqnroll.Helpers;

namespace playwrightreqnroll.Hooks
{
    [Binding]
    public class PlaywrightHooks(PlaywrightDriver driver, ScenarioContext scenarioContext, TestSettings settings, TestArtifactHelper testArtifactHelper)
    {
        private readonly PlaywrightDriver _driver = driver;
        private readonly ScenarioContext _scenarioContext = scenarioContext;
        private readonly TestSettings _settings = settings;
        private readonly TestArtifactHelper _testArtifactHelper = testArtifactHelper;

        [BeforeTestRun]
        public static void GlobalTestRunSetup()
        {
            TestStartupHelper.LoadEnvVariables();
            TestStartupHelper.CleanUpOldVideos();
        }

        [AfterTestRun]
        public static async Task GlobalTestRunTeardown()
        {
            await PlaywrightDriver.DisposeSharedAsync();
        }

        [BeforeScenario(Order = -1)]
        public void SetProductNameFromScenarioOutline()
        {
            if (_scenarioContext.ScenarioInfo.Arguments.Contains("product"))
            {
                var productObj = _scenarioContext.ScenarioInfo.Arguments["product"];
                if (productObj is string product && !string.IsNullOrWhiteSpace(product))
                {
                    _scenarioContext["ProductName"] = product;
                }
            }
            else if(!_scenarioContext.ContainsKey("ProductName"))
            {
                _scenarioContext["ProductName"] = "";
            }
        }

        [BeforeScenario()]
        public async Task InitializeScenarioWithVideo()
        {
            var videoDir = _testArtifactHelper.GetVideoDirectory(_scenarioContext);
            Directory.CreateDirectory(videoDir);
            await _driver.StartAsync(_settings.Headless, videoDir, _settings.Timeout, _settings.SlowMo);
        }

        [AfterScenario(Order = 0)]
        public async Task CleanupScenarioArtifacts()
        {
            await _testArtifactHelper.CaptureScreenshotOnFailureAsync(_scenarioContext, _driver);
            await _testArtifactHelper.HandleVideoAsync(_scenarioContext, _driver);
            await _driver.DisposeAsync();
        }

    }
}