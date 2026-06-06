using Microsoft.Playwright;
using playwrightreqnroll.Drivers;

namespace playwrightreqnroll.Pages
{
    public class ProductsPage
    {
        private readonly PlaywrightDriver _driver;
        private IPage Page => _driver.Page ?? throw new InvalidOperationException("Playwright page is not initialized. Ensure the driver is started before using ProductsPage.");

        private const string InventoryItemButtonSelector = ".inventory_item_button";
        private const string ShoppingCartLinkSelector = ".shopping_cart_link";
        private const string ProductContainerSelectorTemplate = ".inventory_item:has(.inventory_item_name:text-is('{0}'))";
        private const string AddToCartButtonName = "Add to cart";

        public ProductsPage(PlaywrightDriver driver)
        {
            _driver = driver;
        }

        public async Task AddFirstItemToCart()
        {
            await Page.ClickAsync(InventoryItemButtonSelector);
        }

        public async Task GoToCart()
        {
            await Page.ClickAsync(ShoppingCartLinkSelector);
        }
        
        public ILocator GetProductContainerByName(string productName)
        {
            var selector = string.Format(ProductContainerSelectorTemplate, productName);
            return Page.Locator(selector);
        }

        private static async Task ClickAddToCartAsync(ILocator productContainer)
        {
            var addButton = productContainer.GetByRole(AriaRole.Button, new() { Name = AddToCartButtonName });
            await addButton.ClickAsync();
        }

        public async Task AddProductToCartByName(string productName)
        {
            var productContainer = GetProductContainerByName(productName);
            await ClickAddToCartAsync(productContainer);
        }
    }
}