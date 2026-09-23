namespace POS.Models.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string? TransactionId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime? PaymentDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
