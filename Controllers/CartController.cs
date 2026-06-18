using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

public class CartController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly CartService _cartService;

    public CartController(HttpClient httpClient,CartService cartService)
    {
        _httpClient = httpClient;
        _cartService=cartService;
    }

    // ---------------------------------
    // Get Cart page
    // ---------------------------------
    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetFromJsonAsync<CartResponse>("api/Cart/mycart");

        // حفظ عدد العناصر في الـViewBag ليظهر في الـLayout
        ViewBag.CartCount = response?.Items.Sum(i => i.Quantity) ?? 0;

        return View(response ?? new CartResponse());
    }

  
    [HttpPost]
    public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
    {
        var cartItem = new { BookId = bookId, Quantity = quantity };
        var response = await _httpClient.PostAsJsonAsync("api/Cart/add", cartItem);

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Failed to add book to cart.";

        // بعد الإضافة، تحديث عدد العناصر
        ViewBag.CartCount = await GetCartCountAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFromCart(int bookId)
    {
        var response = await _httpClient.DeleteAsync($"api/Cart/delete/{bookId}");

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Failed to remove item.";

       
        ViewBag.CartCount = await GetCartCountAsync();

        return RedirectToAction("Index");
    }

   
    private async Task<int> GetCartCountAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<CartResponse>("api/Cart/mycart");
        return response?.Items.Sum(i => i.Quantity) ?? 0;
    }
   
}
