namespace POS.Models.Entities
{
    public class WebhookLog
    {
        public int WebhookLogId { get; set; }

        public string? OrderNumber { get; set; }

        public string? TransactionId { get; set; }

        public string? Status { get; set; }

        public decimal? Amount { get; set; }

        public string RawPayload { get; set; } = string.Empty;

        public bool IsValid { get; set; }

        public string? ResponseMessage { get; set; }

        public DateTime ReceivedAt { get; set; }
    }
}
