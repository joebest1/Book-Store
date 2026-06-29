using Microsoft.EntityFrameworkCore;
using WebApplication8.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApplication8.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


       
        public DbSet<Books> Books { get; set; }
        public DbSet<Cart> carts { get; set; }
    }
}
