using Microsoft.Extensions.DependencyInjection;
using playwrightreqnroll.Config;
using playwrightreqnroll.Drivers;
using playwrightreqnroll.Helpers;
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

            // Test settings loaded from appsettings.json and env overrides.
            services.AddScoped(_ => ConfigReader.Load());

            // Browser driver - one instance per scenario
            services.AddScoped<PlaywrightDriver>();

            // Artifact helper - one instance per scenario
            services.AddScoped<TestArtifactHelper>();

            // Page objects - one instance per scenario
            services.AddScoped<CartPage>();
            services.AddScoped<HomePage>();
            services.AddScoped<ProductsPage>();

            return services;
        }
    }
}