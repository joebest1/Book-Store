using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication8.Data;
using WebApplication8.Models;

namespace WebApplication8.Controllers.book
{
    [ApiController]
    [Route("[controller]")]
    public class ApiBookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<Books> _logger;

        public ApiBookController(AppDbContext context,ILogger<Books>logger)
        {
            _context = context;
            _logger = logger;

        }
        [HttpPost("Books")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook([FromForm] Books book, IFormFile? BookImage)
        {
            if (book == null) return BadRequest(new { message = "Book data is required." });

           
            if (BookImage != null && BookImage.Length > 0)
            {
                
                var fileName = BookImage.FileName;
               
                book.ImageFileName = fileName;
            }

            try
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding a book");
                return StatusCode(500, new { message = "An error occurred while saving the book." });
            }
        }




        [HttpGet("Books")]
        public IActionResult GetBooks()
        {
        var books = _context.Books.ToList();
            return Ok(books);
        }   


        [HttpDelete("Books/{id}")]
        public IActionResult DeleteBook(int id) {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                _logger.LogWarning("Book with Id={Id} not found", id);
                return NotFound(new { message = $"Book with Id={id} not found" });
            }
            _context.Books.Remove(book);
            _context.SaveChanges();
            return NoContent();

        }

}
}

