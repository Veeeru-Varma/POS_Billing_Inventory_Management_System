using POS.Models.Common;
using POS.Models.DTOs;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DAL.Interfaces
{
    public interface IOrderRepository
    {
        Task<ApiResponse<dynamic>> CreateOrderAsync(CreateOrderDto model, decimal taxPercentage);
        Task<(Order? Order, IEnumerable<OrderItem> Items)> GetByIdAsync(int orderId);

        Task<IEnumerable<Order>> GetHistoryAsync(OrderHistoryFilterDto filter);

        Task<dynamic?> GetDailySalesSummaryAsync(DateTime saleDate);

        Task<ApiResponse> ProcessPaymentAsync(string orderNumber,string status,decimal amount,string transactionId);

        Task<ApiResponse<int>> InsertWebhookLogAsync(string orderNumber,string transactionId,string status,decimal amount, string rawPayload, bool isValid, string responseMessage);
    }
}
