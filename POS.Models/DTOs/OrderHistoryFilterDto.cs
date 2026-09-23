namespace POS.Models.DTOs;

public class OrderHistoryFilterDto
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? PaymentStatus { get; set; }
}