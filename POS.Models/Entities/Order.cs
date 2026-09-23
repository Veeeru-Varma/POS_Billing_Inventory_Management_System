namespace POS.Models.Entities
{
    public class Order
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal Subtotal { get; set; }

        public string? DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxPercentage { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string OrderStatus { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
