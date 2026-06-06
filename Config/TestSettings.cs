namespace playwrightreqnroll.Config
{
   public class TestSettings
    {
        public required string BaseUrl { get; set; }
        public string Browser { get; set; } = "chromium";
        public bool Headless { get; set; }
        public int Timeout {get; set; }
        public required string ScreenshotsDirectory {get; set;} 
        public required string VideosDirectory {get; set;}
        public int SlowMo { get; set; } = 300;
        public int VideoRetentionDays { get; set; } = 1;
    } 
}

