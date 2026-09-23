using POS.BLL.Interfaces;
using POS.DAL.Interfaces;
using POS.Models.Common;

namespace POS.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;

        public PaymentService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<ApiResponse> ProcessPaymentAsync(
            string orderNumber,
            string status,
            decimal amount,
            string transactionId)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Order number is required."
                };
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Payment status is required."
                };
            }

            if (amount <= 0)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Payment amount must be greater than zero."
                };
            }

            if (string.IsNullOrWhiteSpace(transactionId))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Transaction ID is required."
                };
            }

            status = status.Trim().ToLower();

            if (status != "success" && status != "failed")
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Invalid payment status."
                };
            }

            return await _orderRepository.ProcessPaymentAsync(
                orderNumber,
                status,
                amount,
                transactionId);
        }

        public async Task<ApiResponse<int>> LogWebhookAsync(
            string orderNumber,
            string transactionId,
            string status,
            decimal amount,
            string rawPayload,
            bool isValid,
            string responseMessage)
        {
            return await _orderRepository.InsertWebhookLogAsync(
                orderNumber,
                transactionId,
                status,
                amount,
                rawPayload,
                isValid,
                responseMessage);
        }
    }
}
