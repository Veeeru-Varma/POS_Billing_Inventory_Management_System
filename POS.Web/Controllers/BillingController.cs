using Microsoft.AspNetCore.Mvc;
using POS.BLL.Interfaces;
using POS.Models.DTOs;

namespace POS.Web.Controllers
{
    public class BillingController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public BillingController(
            IProductService productService,
            IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
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
                data = result.Data
            });
        }
    }
}
