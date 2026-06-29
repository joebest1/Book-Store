using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WebApplication8.Models;


namespace WebApplication8.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string? UserId { get; set; }  
        public IdentityUser? User { get; set; } 

        public int BookId { get; set; }
        public Books? Book { get; set; } 

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
