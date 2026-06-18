using Microsoft.AspNetCore.Mvc;

namespace BookStore.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartCountViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = await _cartService.GetCartCountAsync();
            return View(count); // ده بيدور على Views/Shared/Components/CartCount/Default.cshtml
        }
    }
}
