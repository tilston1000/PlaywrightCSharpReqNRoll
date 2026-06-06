
using DotNetEnv;
using Reqnroll;
using playwrightreqnroll.Drivers;
using playwrightreqnroll.Config;

namespace playwrightreqnroll.Hooks
{
    [Binding]
    public class PlaywrightHooks(PlaywrightDriver driver, ScenarioContext scenarioContext)
    {
        private static TestSettings? _settings;
        public static TestSettings? Settings => _settings;
        private readonly PlaywrightDriver _driver = driver;
        private readonly ScenarioContext _scenarioContext = scenarioContext;

        [BeforeTestRun]
        public static void GlobalTestRunSetup()
        {
            LoadEnvVariables();
            CleanUpOldVideos();
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
            var (safeScenarioName, safeProductName) = GetSafeNames();
            var videoDir = GetVideoDirectory(safeScenarioName, safeProductName);
            Directory.CreateDirectory(videoDir);
            await _driver.StartAsync(Settings?.Headless ?? true, videoDir, Settings?.Timeout ?? 5000, Settings?.SlowMo ?? 300);
        }

        [AfterScenario(Order = 0)]
        public async Task CleanupScenarioArtifacts()
        {
            await CaptureScreenshotOnFailureAsync();
            await HandleVideoAsync();
            await _driver.DisposeAsync();
        }

        private async Task CaptureScreenshotOnFailureAsync()
        {
            if(_scenarioContext.TestError == null || _driver.Page == null)
                return; 

            var screenshotsDir = Settings?.ScreenshotsDirectory ?? "screenshots";  
            var (safeScenarioName, safeProductName) = GetSafeNames();
            
            var screenshotPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", screenshotsDir, $"{safeScenarioName}_{safeProductName}_{DateTime.Now:ddMMyyyy_HHmmss}.png"));

            var dir = Path.GetDirectoryName(screenshotPath);
            if(dir != null)
                Directory.CreateDirectory(dir);

            await _driver.Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
            _scenarioContext["PlaywrightScreenshotPath"] = screenshotPath;   
        }

        private async Task HandleVideoAsync()
        {
            if (_driver.Page?.Video == null)
                return;

            await _driver.Page.CloseAsync();
            var videoPath = await _driver.Page.Video.PathAsync();
            _scenarioContext["PlaywrightVideoPath"] = videoPath;
        }

        private static string GetSafeScenarioName(string scenarioName)
        {
            var safeName = string.Concat(scenarioName.Split(Path.GetInvalidFileNameChars()));
            safeName = safeName.Replace("<", "").Replace(">", "");
            return safeName;
        }

        private static string GetVideoDirectory(string safeScenarioName, string safeProductName)
        {
            var videosDir = Settings?.VideosDirectory ?? "videos";
            var timestamp = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", videosDir, $"{safeScenarioName}_{safeProductName}_{timestamp}"));
        }

        private static void LoadEnvVariables()
        {
            // Robustly search for .env in parent directories
            string? dir = AppContext.BaseDirectory;
            string? envPath = null;
            while (dir != null)
            {
                var candidate = Path.Combine(dir, ".env");
                if (File.Exists(candidate))
                {
                    envPath = candidate;
                    break;
                }
                dir = Path.GetDirectoryName(dir);
            }
            if (envPath != null)
            {
                Env.Load(envPath);
            }
            else
            {
                Console.WriteLine("[WARN] .env file not found in any parent directory.");
            }
            _settings = ConfigReader.Load();
        }

        private static void CleanUpOldVideos()
        {
            // Deletes videos older than 1 days
            var videosDir = Settings?.VideosDirectory ?? "videos";
            var videosPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", videosDir));
            if (Directory.Exists(videosPath))
            {
                var cutoff = DateTime.Now.AddDays(-1);
                foreach (var file in Directory.GetFiles(videosPath, "*", SearchOption.AllDirectories))
                {
                    var info = new FileInfo(file);
                    if (info.LastWriteTime < cutoff)
                    {
                        info.Delete();
                    }
                }
            }
        }

        private (string SafeScenarioName, string SafeProductName) GetSafeNames()
        {
            var safeScenarioName = GetSafeScenarioName(_scenarioContext.ScenarioInfo.Title);
            _scenarioContext.TryGetValue("ProductName", out var productNameObj);
            var safeProductName = productNameObj is string name && !string.IsNullOrWhiteSpace(name) ? GetSafeScenarioName(name): "";
            return (safeScenarioName, safeProductName);
        }

    }
}