using Allure.Net.Commons;

namespace playwrightreqnroll.Helpers;

public static class AllureHelpers
{
    public static async Task RunAllureStep(string name, Func<Task> action)
    {
        var stepResult = new StepResult { name = name };
        bool stepStarted = false;
        try
        {
            try
            {
                AllureLifecycle.Instance.StartStep(stepResult);
                stepStarted = true;
            }
            catch (InvalidOperationException)
            {
                await action();
                return;
            }

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
                if (stepStarted)
                    AllureLifecycle.Instance.StopStep();
            }
        }
        catch
        {
            throw;
        }
    }
}