using POS.BLL.Interfaces;
using POS.DAL.Interfaces;
using POS.Models.Common;
using POS.Models.DTOs;
using POS.Models.Entities;

namespace POS.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITaxRepository _taxRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ITaxRepository taxRepository)
        {
            _orderRepository = orderRepository;
            _taxRepository = taxRepository;
        }

        public async Task<ApiResponse<dynamic>> CreateOrderAsync(
            CreateOrderDto model)
        {
            if (model.Items == null || model.Items.Count == 0)
            {
                return new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "Cart cannot be empty."
                };
            }

            foreach (var item in model.Items)
            {
                if (item.ProductId <= 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = "Invalid product ID."
                    };
                }

                if (item.Quantity <= 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        Success = false,
                        Message = "Product quantity must be greater than zero."
                    };
                }
            }

            if (model.DiscountValue < 0)
            {
                return new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "Discount cannot be negative."
                };
            }

            var tax = await _taxRepository.GetActiveTaxAsync();

            if (tax == null)
            {
                return new ApiResponse<dynamic>
                {
                    Success = false,
                    Message = "Active GST configuration not found."
                };
            }

            return await _orderRepository.CreateOrderAsync(
                model,
                tax.TaxPercentage);
        }

        public async Task<(Order? Order, IEnumerable<OrderItem> Items)>
            GetByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetHistoryAsync(
            OrderHistoryFilterDto filter)
        {
            return await _orderRepository.GetHistoryAsync(filter);
        }

        public async Task<dynamic?> GetDailySalesSummaryAsync(
            DateTime saleDate)
        {
            return await _orderRepository.GetDailySalesSummaryAsync(saleDate);
        }
    }
}
