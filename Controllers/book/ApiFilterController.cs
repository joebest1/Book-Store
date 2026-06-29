using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication8.Data;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public BooksController(AppDbContext context, ILogger<Books> logger)
        {
            _context = context;
            _logger = logger;

        }




        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks(string? author, string? title, string? category)
        {
            if (string.IsNullOrEmpty(author) && string.IsNullOrEmpty(title) && string.IsNullOrEmpty(category))
            {
                return BadRequest(new { message = "Please provide at least one filter parameter." });
            }

            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(author))
                books = books.Where(b => b.Author.Contains(author));

            if (!string.IsNullOrEmpty(title))
                books = books.Where(b => b.Title.Contains(title));

            if (!string.IsNullOrEmpty(category))
                books = books.Where(b => b.Category.Contains(category));

            var result = await books.ToListAsync();

            if (!result.Any())
            {
                _logger.LogWarning(
                    "No books found with filters: Author={Author}, Title={Title}, Category={Category}",
                    author, title, category
                );

                return NotFound(new { message = $"No books found with given filters (Author={author}, Title={title}, Category={category})" });
            }

            return Ok(result);
        }



        [HttpGet("sort")]
        public IActionResult Sort(string sortBy = "date", string order = "asc")
        {
            IQueryable<Books> query = _context.Books;

            switch (sortBy.ToLower())
            {
                case "date":
                    query = order.ToLower() == "desc"
                        ? query.OrderByDescending(b => b.PublishedDate)
                        : query.OrderBy(b => b.PublishedDate);
                    break;

                case "price":
                    query = order.ToLower() == "desc"
                        ? query.OrderByDescending(b => b.Price)
                        : query.OrderBy(b => b.Price);
                    break;

                default:
                    return BadRequest(new { message = "Invalid sortBy parameter. Use 'date' or 'price'." });
            }

            return Ok(query.ToList());
        }
    }
}
