using POS.Models.Common;

namespace POS.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse> ProcessPaymentAsync(string orderNumber, string status, decimal amount, string transactionId);

        Task<ApiResponse<int>> LogWebhookAsync(string orderNumber, string transactionId, string status, decimal amount, string rawPayload, bool isValid, string responseMessage);
    }
}
