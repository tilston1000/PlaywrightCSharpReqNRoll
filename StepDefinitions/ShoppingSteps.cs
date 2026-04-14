using FluentAssertions;
using Reqnroll;

[Binding]
public class ShoppingSteps
{
    private readonly HomePage _home;
    private readonly ProductsPage _products;
    private readonly CartPage _cart;

    public ShoppingSteps(HomePage home, ProductsPage products, CartPage cart)
    {
        _home = home;
        _products = products;
        _cart = cart;
    }

    [Given("I open the application")]
    public async Task OpenApp()
    {
        await _home.Navigate();
    }

    [When("I login with valid credentials")]
    public async Task Login()
    {
        await _home.Login("standard_user", "secret_sauce");
    }

    [When("I add a product to the cart")]
    public async Task AddProduct()
    {
        await _products.AddFirstItemToCart();
        await _products.GoToCart();
    }

    [Then("the cart should contain the product")]
    public async Task VerifyCart()
    {
        (await _cart.IsItemDisplayed()).Should().BeTrue();
    }
}