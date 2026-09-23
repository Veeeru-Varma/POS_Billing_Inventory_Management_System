namespace POS.Models.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
