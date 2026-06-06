using Microsoft.Playwright;
using playwrightreqnroll.Drivers;

namespace playwrightreqnroll.Pages
{
    public class CartPage
    {
        private readonly PlaywrightDriver _driver;
        private IPage Page => _driver.Page ?? throw new InvalidOperationException("Playwright page is not initialized. Ensure the driver is started before using CartPage.");

        private const string CartItemSelectorTemplate = ".cart_item:has(.inventory_item_name:text-is('{0}'))";

        public CartPage(PlaywrightDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> IsItemDisplayed(string productName)
        {
            // Checks if a cart item with the given product name is visible
            var selector = string.Format(CartItemSelectorTemplate, productName);
            return await Page.IsVisibleAsync(selector);
        }
    }
}