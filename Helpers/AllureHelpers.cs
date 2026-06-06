using Allure.Net.Commons;
using System.Diagnostics;

namespace playwrightreqnroll.Helpers;

public static class AllureHelpers
{
    public static async Task RunAllureStep(string name, Func<Task> action)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Step name cannot be null or empty.", nameof(name));

        var stepStarted = false;
        try
        {
            AllureLifecycle.Instance.StartStep(new StepResult { name = name });
            stepStarted = true;  // Only set true AFTER successful start
            
            await action();
            AllureLifecycle.Instance.UpdateStep(x => x.status = Status.passed);
        }
        catch (Exception ex)
        {
            if (stepStarted)
            {
                try
                {
                    AllureLifecycle.Instance.UpdateStep(x =>
                    {
                        x.status = Status.failed;
                        x.statusDetails = new StatusDetails { message = ex.Message };
                    });
                }
                catch (Exception updateEx)
                {
                    Trace.TraceWarning($"[AllureHelpers] Failed to update Allure step status: {updateEx.Message}");
                }
            }
            throw;
        }
        finally
        {
            if (stepStarted)
            {
                try
                {
                    AllureLifecycle.Instance.StopStep();
                }
                catch (Exception stopEx)
                {
                    Trace.TraceWarning($"[AllureHelpers] Failed to stop Allure step: {stopEx.Message}");
                }
            }
        }
    }
}