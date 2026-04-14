using Microsoft.Playwright;

public class CartPage
{
    private readonly IPage _page;

    public CartPage(PlaywrightDriver driver)
    {
        _page = driver.Page;
    }

    public async Task<bool> IsItemDisplayed()
    {
        return await _page.IsVisibleAsync(".cart_item");
    }
}