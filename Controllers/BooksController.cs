using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;

public class BooksController : Controller
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    // Index مع Search و Sort
    public async Task<IActionResult> Index(string? search = null, string? sortBy = null)
    {
        var books = await _bookService.GetBooksAsync(search, sortBy);
        ViewBag.CurrentSearch = search;
        ViewBag.CurrentSort = sortBy;
        return View(books);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Admin")
            return StatusCode(403); // Forbidden بدل Forbid()

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MyBookModel book, IFormFile? BookImage)
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Admin") return StatusCode(403);

        if (!ModelState.IsValid) return View(book);

        byte[]? imageData = null;
        string? imageFileName = null;

        if (BookImage != null && BookImage.Length > 0)
        {
            // حفظ الصورة في wwwroot/images/books
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/books");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // عمل اسم فريد للصورة لتجنب التكرار
            var fileName = Guid.NewGuid() + Path.GetExtension(BookImage.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await BookImage.CopyToAsync(stream);
            }

            using var ms = new MemoryStream();
            await BookImage.CopyToAsync(ms);
            imageData = ms.ToArray();
            imageFileName = fileName;
        }

        // إرسال البيانات للـAPI
        bool success = await _bookService.AddBookWithImageAsync(book, imageData, imageFileName);

        if (success) return RedirectToAction("Index");

        ModelState.AddModelError("", "Failed to add book. Check console for details.");
        return View(book);
    }
}
