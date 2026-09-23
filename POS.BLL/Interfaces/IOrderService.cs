using POS.Models.Common;
using POS.Models.DTOs;
using POS.Models.Entities;

namespace POS.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<dynamic>> CreateOrderAsync(CreateOrderDto model);

        Task<(Order? Order, IEnumerable<OrderItem> Items)> GetByIdAsync(int orderId);

        Task<IEnumerable<Order>> GetHistoryAsync(OrderHistoryFilterDto filter);

        Task<dynamic?> GetDailySalesSummaryAsync(DateTime saleDate);
    }
}
