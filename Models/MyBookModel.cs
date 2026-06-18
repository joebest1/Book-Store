namespace BookStore.Models
{
    public class MyBookModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public string? ImageFileName { get; set; }

    }
}
