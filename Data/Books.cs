using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication8.Models
{
    public class Books
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public int Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime PublishedDate { get; set; }

        public string? ImageFileName { get; set; } // مهم يكون nullable
    }
}
