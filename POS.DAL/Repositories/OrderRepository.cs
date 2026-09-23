using Dapper;
using POS.DAL.Context;
using POS.DAL.Interfaces;
using POS.Models.Common;
using POS.Models.DTOs;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POS.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext _context;

        public OrderRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<dynamic>> CreateOrderAsync(CreateOrderDto model, decimal taxPercentage)
        {
            using var connection = _context.CreateConnection();

            var orderNumber =
                $"ORD-{DateTime.Now:yyyyMMddHHmmssfff}";

            var itemsJson = JsonSerializer.Serialize(
                model.Items.Select(x => new
                {
                    x.ProductId,
                    x.Quantity
                }));

            var parameters = new
            {
                OrderNumber = orderNumber,
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                TaxPercentage = taxPercentage,
                ItemsJson = itemsJson
            };

            var result = await connection.QueryFirstAsync<dynamic>(
                "sp_Order_Create",
                parameters,
                commandType: CommandType.StoredProcedure);

            return new ApiResponse<dynamic>
            {
                Success = Convert.ToInt32(result.Success) == 1,
                Message = result.Message?.ToString() ?? string.Empty,
                Data = result
            };
        }
        public async Task<(Order? Order, IEnumerable<OrderItem> Items)>
            GetByIdAsync(int orderId)
        {
            using var connection = _context.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "sp_Order_GetById",
                new
                {
                    OrderId = orderId
                },
                commandType: CommandType.StoredProcedure);

            var order = await multi.ReadFirstOrDefaultAsync<Order>();

            var items = await multi.ReadAsync<OrderItem>();

            return (order, items);
        }

        public async Task<IEnumerable<Order>> GetHistoryAsync(
            OrderHistoryFilterDto filter)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Order>(
                "sp_Order_GetHistory",
                new
                {
                    filter.FromDate,
                    filter.ToDate,
                    filter.PaymentStatus
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> GetDailySalesSummaryAsync(
            DateTime saleDate)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Order_GetDailySalesSummary",
                new
                {
                    SaleDate = saleDate.Date
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<ApiResponse> ProcessPaymentAsync(
            string orderNumber,
            string status,
            decimal amount,
            string transactionId)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryFirstAsync<dynamic>(
                "sp_ProcessPaymentWebhook",
                new
                {
                    OrderNumber = orderNumber,
                    Status = status,
                    Amount = amount,
                    TransactionId = transactionId
                },
                commandType: CommandType.StoredProcedure);

            return new ApiResponse
            {
                Success = Convert.ToInt32(result.Success) == 1,
                Message = result.Message?.ToString() ?? string.Empty
            };
        }

        public async Task<ApiResponse<int>> InsertWebhookLogAsync(
            string orderNumber,
            string transactionId,
            string status,
            decimal amount,
            string rawPayload,
            bool isValid,
            string responseMessage)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.QueryFirstAsync<dynamic>(
                "sp_WebhookLog_Insert",
                new
                {
                    OrderNumber = orderNumber,
                    TransactionId = transactionId,
                    Status = status,
                    Amount = amount,
                    RawPayload = rawPayload,
                    IsValid = isValid,
                    ResponseMessage = responseMessage
                },
                commandType: CommandType.StoredProcedure);

            return new ApiResponse<int>
            {
                Success = true,
                Message = "Webhook logged successfully.",
                Data = Convert.ToInt32(result.WebhookLogId)
            };
        }
    }
}
