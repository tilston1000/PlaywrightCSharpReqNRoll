using Microsoft.VisualStudio.TestTools.UnitTesting;
using playwrightreqnroll.Helpers;
using playwrightreqnroll.Pages;
using Reqnroll;

namespace playwrightreqnroll.StepDefinitions
{
    [Binding]
    public class ShoppingSteps(HomePage home, ProductsPage products, CartPage cart, ScenarioContext scenarioContext)
    {
        private readonly HomePage _home = home;
        private readonly ProductsPage _products = products;
        private readonly CartPage _cart = cart;
        private readonly ScenarioContext _scenarioContext = scenarioContext;

        [Given("I open the application")]
        public async Task OpenApp()
        {
            await RunStepAsync("Open the application and Login", async () =>
            {
                await _home.Navigate();
                await AuthHelpers.LoginWithEnvCredentials(_home);
            });
        }

        [When("I add a {string} to the cart")]
        public async Task WhenIAddAToTheCart(string productName)
        {
            _scenarioContext["ProductName"] = productName;

            await RunStepAsync($"Add '{productName}' to cart", async () =>
            {
                await _products.AddProductToCartByName(productName);
                await _products.GoToCart();
            });
        }

        [When("I login to the application")]
        public async Task LoginToApp()
        {
            await RunStepAsync("Login to the application", async () =>
            {
                await AuthHelpers.LoginWithEnvCredentials(_home);
            });
        }

        [When("I log out of the application")]
        public async Task WhenILogOut()
        {
            await RunStepAsync("Logout of the application", async () =>
            {
                await _home.Logout();
            });
        }

        [When("I click on the cart icon")]
        public async Task WhenIClickOnTheCartIcon()
        {
            await RunStepAsync("Click on cart icon", async () =>
            {
                await _products.GoToCart();
            });
        }

        [Then("the cart should contain {string}")]
        public async Task ThenTheCartShouldContain(string productName)
        {
            await RunStepAsync($"Verify the cart contains '{productName}'", async () =>
            {
                Assert.IsTrue(await _cart.IsItemDisplayed(productName), $"the cart should display the added item: {productName}");
            });
        }

        private static Task RunStepAsync(string stepName, Func<Task> action)
        {
            return AllureHelpers.RunAllureStep(stepName, action);
        }
    }
}