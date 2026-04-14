using Microsoft.Playwright;

public class ProductsPage
{
    private readonly IPage _page;

    public ProductsPage(PlaywrightDriver driver)
    {
        _page = driver.Page;
    }

    public async Task AddFirstItemToCart()
    {
        await _page.ClickAsync(".inventory_item button");
    }

    public async Task GoToCart()
    {
        await _page.ClickAsync(".shopping_cart_link");
    }
}