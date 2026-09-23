using Microsoft.AspNetCore.Mvc;
using POS.BLL.Interfaces;
using POS.Models.DTOs;

namespace POS.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index(string? search)
        {
            var products = string.IsNullOrWhiteSpace(search)
                ? await _productService.GetAllAsync()
                : await _productService.SearchAsync(search);

            ViewBag.Search = search;

            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> LowStock()
        {
            var products = await _productService.GetLowStockAsync();

            return View(products);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _productService.AddAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductRequestDto
            {
                Name = product.Name,
                SKU = product.SKU,
                Category = product.Category,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                LowStockThreshold = product.LowStockThreshold
            };

            ViewBag.ProductId = product.ProductId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ProductRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProductId = id;
                return View(model);
            }

            var result = await _productService.UpdateAsync(id, model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.ProductId = id;
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"]
                = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
