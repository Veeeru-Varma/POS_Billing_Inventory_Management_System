using POS.BLL.Interfaces;
using POS.Models.Entities;

namespace POS.BackgroundService
{
    public class Worker : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public Worker(
            ILogger<Worker> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Low Stock Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await GenerateLowStockReportAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while generating low stock report.");
                }

                // Run once every 24 hours
                await Task.Delay(
                    TimeSpan.FromHours(24),
                    stoppingToken);
            }
        }

        private async Task GenerateLowStockReportAsync(
            CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var productService =
                scope.ServiceProvider
                    .GetRequiredService<IProductService>();

            var products =
                await productService.GetLowStockAsync();

            var productList = products.ToList();

            var reportFolder =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Reports");

            Directory.CreateDirectory(reportFolder);

            var fileName =
                $"LowStock_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            var filePath =
                Path.Combine(
                    reportFolder,
                    fileName);

            var csv = new System.Text.StringBuilder();

            csv.AppendLine(
                "Product Id,Product Name,SKU,Category,Current Stock,Low Stock Threshold");

            foreach (var product in productList)
            {
                csv.AppendLine(
                    $"{product.ProductId}," +
                    $"\"{product.Name}\"," +
                    $"\"{product.SKU}\"," +
                    $"\"{product.Category}\"," +
                    $"{product.StockQuantity}," +
                    $"{product.LowStockThreshold}");
            }

            await File.WriteAllTextAsync(
                filePath,
                csv.ToString(),
                stoppingToken);

            _logger.LogInformation(
                "Low stock report generated. Products: {count}, File: {file}",
                productList.Count,
                filePath);
        }
    }
}