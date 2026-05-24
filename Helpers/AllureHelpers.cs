using Allure.Net.Commons;

namespace playwrightreqnroll.Helpers;

public static class AllureHelpers
{
    public static async Task RunAllureStep(string name, Func<Task> action)
    {
        var stepStarted = false;
        try
        {
            try
            {
                stepStarted = true;
                AllureLifecycle.Instance.StartStep(new StepResult { name = name });
            }
            catch (Exception startEx)
            {
                Console.WriteLine($"[AllureHelpers] Failed to start Allure step: {startEx.Message}");
                throw;
            }
            await action();
            if (stepStarted)
                AllureLifecycle.Instance.UpdateStep(x => x.status = Status.passed);
        }
        catch (Exception ex)
        {
            if (stepStarted)
            {
                AllureLifecycle.Instance.UpdateStep(x =>
                {
                    x.status = Status.failed;
                    x.statusDetails = new StatusDetails { message = ex.Message };
                });
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
                    Console.WriteLine($"[AllureHelpers] Failed to stop Allure step: {stopEx.Message}");
                }
            }
        }
    }

    // Helper to get the current parent step UUID from Allure's context (if any)
    private static string? GetCurrentAllureStepUuid()
    {
        try
        {
            var storageField = typeof(AllureLifecycle).GetField("_storage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (storageField != null)
            {
                var storage = storageField.GetValue(AllureLifecycle.Instance);
                if (storage != null)
                {
                    var stepStackField = storage.GetType().GetField("_stepContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (stepStackField != null)
                    {
                        var stepStack = stepStackField.GetValue(storage) as System.Collections.Generic.Stack<string>;
                        if (stepStack != null && stepStack.Count > 0)
                        {
                            return stepStack.Peek();
                        }
                    }
                }
            }
        }
        catch { /* Reflection may fail, just return null */ }
        return null;
    }
}