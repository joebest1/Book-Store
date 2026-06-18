using BookStore.Models;
using System.Net.Http;
using System.Net.Http.Json;

public class CartService
{
    private readonly HttpClient _httpClient;
   

    public CartService(HttpClient httpClient )
    {
        _httpClient = httpClient;
        
    }

    // جلب الكارت بالكامل
    public async Task<CartResponse> GetMyCartAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<CartResponse>("api/Cart/mycart");
        return response ?? new CartResponse();
    }

    // إضافة كتاب للكارت
    public async Task<bool> AddToCartAsync(int bookId, int quantity = 1)
    {
        var cartItem = new { BookId = bookId, Quantity = quantity };
        var response = await _httpClient.PostAsJsonAsync("api/Cart/add", cartItem);
        return response.IsSuccessStatusCode;
    }

    // حذف كتاب من الكارت
    public async Task<bool> DeleteFromCartAsync(int bookId)
    {
        var response = await _httpClient.DeleteAsync($"api/Cart/delete/{bookId}");
        return response.IsSuccessStatusCode;
    }

    // جلب عدد العناصر فقط
    public async Task<int> GetCartCountAsync()
    {
        var cart = await GetMyCartAsync();
        return cart.Items.Sum(i => i.Quantity);
    }

}
    