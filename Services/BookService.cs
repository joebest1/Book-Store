using BookStore.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BookService
{
    private readonly HttpClient _httpClient;

    public BookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // جلب الكتب مع فلترة Search على Title و Author + Sort
    public async Task<List<MyBookModel>> GetBooksAsync(string? search = null, string? sortBy = null)
    {
        var books = await _httpClient.GetFromJsonAsync<List<MyBookModel>>("/ApiBook/Books")
                     ?? new List<MyBookModel>();

        // فلترة Search
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            books = books.Where(b =>
                        (b.Title?.ToLower().Contains(search) ?? false) ||
                        (b.Author?.ToLower().Contains(search) ?? false))
                         .ToList();
        }

        // تطبيق Sort
        if (!string.IsNullOrEmpty(sortBy))
        {
            books = sortBy switch
            {
                "price_asc" => books.OrderBy(b => b.Price).ToList(),
                "price_desc" => books.OrderByDescending(b => b.Price).ToList(),
                "date_asc" => books.OrderBy(b => b.PublishedDate).ToList(),
                "date_desc" => books.OrderByDescending(b => b.PublishedDate).ToList(),
                _ => books
            };
        }

        return books;
    }

    // إضافة كتاب مع صورة
    public async Task<bool> AddBookWithImageAsync(MyBookModel book, byte[]? imageData = null, string? imageFileName = null)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(book.Title ?? ""), "Title");
        content.Add(new StringContent(book.Author ?? ""), "Author");
        content.Add(new StringContent(book.Category ?? ""), "Category");
        content.Add(new StringContent(book.Price.ToString()), "Price");
        content.Add(new StringContent(book.PublishedDate.ToString("yyyy-MM-dd")), "PublishedDate");

        if (imageData != null && imageFileName != null)
        {
            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(imageContent, "BookImage", imageFileName);
        }

        var response = await _httpClient.PostAsync("/ApiBook/Books", content);

        Console.WriteLine($"AddBookWithImageAsync Response Status: {response.StatusCode}");
        var respText = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response Content: {respText}");

        return response.IsSuccessStatusCode;
    }
}
