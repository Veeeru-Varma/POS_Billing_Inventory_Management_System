using Microsoft.AspNetCore.Mvc;
using POS.BLL.Interfaces;
using POS.Models.DTOs;
using POS.Models.Common;
using System.Net.Http.Json;

namespace POS.Web.Controllers
{
    public class BillingController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BillingController(
            IProductService productService,
            IOrderService orderService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _productService = productService;
            _orderService = orderService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var products = string.IsNullOrWhiteSpace(search)
                ? await _productService.GetAllAsync()
                : await _productService.SearchAsync(search);

            ViewBag.Search = search;

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto model)
        {
            var result = await _orderService.CreateOrderAsync(model);

            if (!result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Json(new
            {
                success = true,
                message = result.Message,
                data = new
                {
                    orderNumber = result.Data?.OrderNumber,
                    grandTotal = result.Data?.GrandTotal
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SimulatePayment(
            string orderNumber,
            decimal amount,
            string status)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return Json(new
                {
                    success = false,
                    message = "Order number is required."
                });
            }

            if (amount <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid payment amount."
                });
            }

            if (status != "success" && status != "failed")
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid payment status."
                });
            }

            var transactionId =
                $"TXN-{status.ToUpper()}-{DateTime.Now:yyyyMMddHHmmssfff}";

            var request = new PaymentWebhookDto
            {
                OrderId = orderNumber,
                Status = status,
                Amount = amount,
                TransactionId = transactionId
            };

            var client =
                _httpClientFactory.CreateClient("PaymentApi");

            var secret =
                _configuration["PaymentApi:Secret"];

            client.DefaultRequestHeaders.Remove(
                "X-Webhook-Secret");

            client.DefaultRequestHeaders.Add(
                "X-Webhook-Secret",
                secret);

            var response = await client.PostAsJsonAsync(
                "api/PaymentWebhook/webhook",
                request);

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse>();

            return Json(new
            {
                success =
                    response.IsSuccessStatusCode &&
                    result?.Success == true,

                message =
                    result?.Message ??
                    "Payment processing failed."
            });
        }
    }
}