using Allure.Net.Commons;
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
            Console.WriteLine("[DIAG] Entering step: Open the application and Login");
            await AllureHelpers.RunAllureStep("Open the application and Login", async () =>
            {
                Console.WriteLine("[DIAG] Inside Allure step: Open the application and Login");
                await _home.Navigate();
                await AuthHelpers.LoginWithEnvCredentials(_home);      
            });
        }

        [When("I add a {string} to the cart")]
        public async Task WhenIAddAToTheCart(string productName)
        {
            _scenarioContext["ProductName"] = productName;

            Console.WriteLine($"[DIAG] Entering step: Add '{productName}' to cart");
            await AllureHelpers.RunAllureStep($"Add '{productName}' to cart", async () =>
            {
                Console.WriteLine($"[DIAG] Inside Allure step: Add '{productName}' to cart");
                await _products.AddProductToCartByName(productName);
                await _products.GoToCart();       
            });
        }

        [When("I login to the application")]
        public async Task LoginToApp()
        {
            Console.WriteLine("[DIAG] Entering step: Login to the application");
            await AllureHelpers.RunAllureStep("Login to the application", async () =>
            {
                Console.WriteLine("[DIAG] Inside Allure step: Login to the application");
                await AuthHelpers.LoginWithEnvCredentials(_home);
            });
        }

        [When("I log out of the application")]
        public async Task WhenILogOut()
        {
            Console.WriteLine("[DIAG] Entering step: Logout of the application");
            await AllureHelpers.RunAllureStep("Logout of the application", async () =>
            {
                Console.WriteLine("[DIAG] Inside Allure step: Logout of the application");
                await _home.Logout();
            });
        }

        [When("I click on the cart icon")]
        public async Task WhenIClickOnTheCartIcon()
        {
            Console.WriteLine("[DIAG] Entering step: Click on cart icon");
            await AllureHelpers.RunAllureStep("Click on cart icon", async () =>
            {
                Console.WriteLine("[DIAG] Inside Allure step: Click on cart icon");
                await _products.GoToCart();
            });
        }

        [Then("the cart should contain {string}")]
        public async Task ThenTheCartShouldContain(string productName)
        {

            Console.WriteLine($"[DIAG] Entering step: Verify the cart contains '{productName}'");
            await AllureHelpers.RunAllureStep($"Verify the cart contains '{productName}'", async () =>
            {
                Console.WriteLine($"[DIAG] Inside Allure step: Verify the cart contains '{productName}'");
                Assert.IsTrue(await _cart.IsItemDisplayed(productName), $"the cart should display the added item: {productName}");
            });
        }
    }
}