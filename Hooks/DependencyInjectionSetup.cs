using Microsoft.Extensions.DependencyInjection;
using playwrightreqnroll.Drivers;
using playwrightreqnroll.Pages;
using Reqnroll;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace playwrightreqnroll.Hooks
{
    [Binding]
    public class DependencyInjectionSetup
    {
        [ScenarioDependencies]
        public static IServiceCollection RegisterServices()
        {
            var services = new ServiceCollection();

            // Browser driver - one instance per scenario
            services.AddScoped<PlaywrightDriver>();

            // Page objects - one instance per scenario
            services.AddScoped<CartPage>();
            services.AddScoped<HomePage>();
            services.AddScoped<ProductsPage>();

            return services;
        }
    }
}