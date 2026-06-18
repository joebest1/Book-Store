namespace BookStore.Models
{
    public class CartResponse
    {
        public IEnumerable<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal TotalPrice { get; set; }
    }

    public class CartItemViewModel
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Book? Book { get; set; } // لو جاي من API مع Book object
    }

    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal TotalPrice { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? ImageFileName { get; set; }
    }
}
