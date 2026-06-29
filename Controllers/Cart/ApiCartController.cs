    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using WebApplication8.Data;
    using WebApplication8.Models;
    using Microsoft.EntityFrameworkCore;

namespace WebApplication8.Controllers.ApiCart
{
    [Authorize]
    [ApiController]
    [Route("api/Cart")]
    public class ApiCartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApiCartController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] Cart model)
    {
        var userId = _userManager.GetUserId(User); 
        model.UserId = userId;

       
        var existingItem = await _context.carts
            .FirstOrDefaultAsync(c => c.BookId == model.BookId && c.UserId == userId);

        if (existingItem != null)
        {
         
            existingItem.Quantity += model.Quantity > 0 ? model.Quantity : 1;
            _context.carts.Update(existingItem);
        }
        else
        {
           
            if (model.Quantity <= 0)
                model.Quantity = 1;

            _context.carts.Add(model);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Book added to cart" });
    }
    [HttpGet("mycart")]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = _userManager.GetUserId(User);

            var cart = await _context.carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();

           
            var totalPrice = cart.Sum(c => (c.Book?.Price ?? 0) * c.Quantity);

            return Ok(new
            {
                items = cart,
                totalPrice = totalPrice
            });
        }
        [HttpDelete("delete/{bookId}")]
        public async Task<IActionResult> DeleteFromCart(int bookId)
        {
            var userId = _userManager.GetUserId(User);


            var cartItem = await _context.carts
                .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);
            
            if (cartItem == null)
            {
                return NotFound(new { message = "Item not found in cart." });
            }

            
            _context.carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item removed from cart." });
        }


    }
}
