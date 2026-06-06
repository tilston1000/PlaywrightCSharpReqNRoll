using DotNetEnv;
using playwrightreqnroll.Config;
using System.Diagnostics;

namespace playwrightreqnroll.Helpers;

public static class TestStartupHelper
{
    public static void LoadEnvVariables()
    {
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
            var isCi = string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase);
            if (!isCi)
                Trace.TraceWarning("[WARN] .env file not found in any parent directory.");
        }
    }

    public static void CleanUpOldVideos()
    {
        var settings = ConfigReader.Load();
        var videosDir = settings.VideosDirectory;
        var retentionDays = Math.Max(0, settings.VideoRetentionDays);
        var videosPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", videosDir));
        if (!Directory.Exists(videosPath))
            return;

        var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
        foreach (var file in Directory.GetFiles(videosPath, "*", SearchOption.AllDirectories))
        {
            try
            {
                var info = new FileInfo(file);
                if (info.LastWriteTimeUtc < cutoff)
                {
                    info.Delete();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"[WARN] Failed to delete old video artifact '{file}': {ex.Message}");
            }
        }

        var directories = Directory.GetDirectories(videosPath, "*", SearchOption.AllDirectories)
            .OrderByDescending(d => d.Length);

        foreach (var directory in directories)
        {
            try
            {
                var dirInfo = new DirectoryInfo(directory);
                if (dirInfo.LastWriteTimeUtc < cutoff || !Directory.EnumerateFileSystemEntries(directory).Any())
                {
                    Directory.Delete(directory, recursive: true);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"[WARN] Failed to delete old video directory '{directory}': {ex.Message}");
            }
        }
    }
}