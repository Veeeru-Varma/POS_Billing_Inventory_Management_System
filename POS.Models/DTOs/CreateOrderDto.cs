namespace POS.Models.DTOs;

public class CreateOrderDto
{
    public string? DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public List<CartItemDto> Items { get; set; } = new();
}