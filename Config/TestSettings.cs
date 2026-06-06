namespace playwrightreqnroll.Config
{
   public class TestSettings
    {
        public required string BaseUrl { get; set; }
        public bool Headless { get; set; }
        public int Timeout {get; set; }
        public required string ScreenshotsDirectory {get; set;} 
        public required string VideosDirectory {get; set;}
        public int SlowMo { get; set; } = 300;
    } 
}

