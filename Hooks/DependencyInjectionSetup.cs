using Microsoft.Extensions.DependencyInjection;
using playwrightreqnroll.Drivers;
using playwrightreqnroll.Pages;
using Reqnroll;
// using Reqnroll.Microsoft.Extensions.DependencyInjection; // Removed for Docker build

namespace playwrightreqnroll.Hooks
{
    [Binding] 
    public class DependencyInjectionSetup
    {

        // [ScenarioDependencies]
        // public static IServiceCollection  RegisterServices()
        // {
        //     var services = new ServiceCollection();
        //
        //     // Register Playwright + browser services
        //     services.AddScoped<PlaywrightDriver>();
        //
        //     // Register page objects
        //     services.AddScoped<CartPage>();
        //     services.AddScoped<HomePage>();
        //     services.AddScoped<ProductsPage>();
        //
        //     // Build provider
        //     return services;
        // }
    }
}