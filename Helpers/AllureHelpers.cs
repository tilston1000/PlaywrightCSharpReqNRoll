using Allure.Net.Commons;

namespace playwrightreqnroll.Helpers;

public static class AllureHelpers
{
    public static async Task RunAllureStep(string name, Func<Task> action)
    {
        var stepResult = new StepResult { name = name };
        AllureLifecycle.Instance.StartStep(stepResult);
        try
        {
            await action();
            AllureLifecycle.Instance.UpdateStep(x => x.status = Status.passed);
        }
        catch (Exception ex)
        {
            AllureLifecycle.Instance.UpdateStep(x =>
            {
                x.status = Status.failed;
                x.statusDetails = new StatusDetails { message = ex.Message };
            });
            throw;
        }
        finally
        {
            AllureLifecycle.Instance.StopStep();
        }
    }
}