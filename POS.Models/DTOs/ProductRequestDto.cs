namespace POS.Models.DTOs
{
    public class ProductRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }
    }
}
