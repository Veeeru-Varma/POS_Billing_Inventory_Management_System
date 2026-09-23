using System.Text.Json.Serialization;

namespace POS.Models.DTOs;

public class PaymentWebhookDto
{
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; } = string.Empty;
}