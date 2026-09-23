using Microsoft.AspNetCore.Mvc;
using POS.BLL.Interfaces;
using POS.Models.DTOs;
using System.Text.Json;

namespace POS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentWebhookController(
            IPaymentService paymentService,
            IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook(
            [FromHeader(Name = "X-Webhook-Secret")] string webhookSecret,
            [FromBody] PaymentWebhookDto model)
        {
            var rawPayload = JsonSerializer.Serialize(model);

            var secret = _configuration["PaymentWebhook:Secret"];

            // Get webhook secret from request header
            var requestSecret = webhookSecret;

            // Validate secret
            if (string.IsNullOrWhiteSpace(requestSecret) ||
                requestSecret != secret)
            {
                await _paymentService.LogWebhookAsync(
                    model.OrderId,
                    model.TransactionId,
                    model.Status,
                    model.Amount,
                    rawPayload,
                    false,
                    "Invalid webhook secret.");

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid webhook secret."
                });
            }

            // Validate request data
            if (string.IsNullOrWhiteSpace(model.OrderId) ||
                string.IsNullOrWhiteSpace(model.Status) ||
                string.IsNullOrWhiteSpace(model.TransactionId) ||
                model.Amount <= 0)
            {
                await _paymentService.LogWebhookAsync(
                    model.OrderId,
                    model.TransactionId,
                    model.Status,
                    model.Amount,
                    rawPayload,
                    false,
                    "Invalid webhook payload.");

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid webhook payload."
                });
            }

            // Process payment
            var result = await _paymentService.ProcessPaymentAsync(
                model.OrderId,
                model.Status,
                model.Amount,
                model.TransactionId);

            // Log webhook
            await _paymentService.LogWebhookAsync(
                model.OrderId,
                model.TransactionId,
                model.Status,
                model.Amount,
                rawPayload,
                result.Success,
                result.Message);

            // Payment processing failed
            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }

            // Payment processing successful
            return Ok(new
            {
                success = true,
                message = result.Message
            });
        }
    }
}