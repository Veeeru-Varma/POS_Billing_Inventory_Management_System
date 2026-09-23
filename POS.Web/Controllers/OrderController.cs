using Microsoft.AspNetCore.Mvc;
using POS.BLL.Interfaces;
using POS.Models.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace POS.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // ==============================
        // ORDER HISTORY
        // ==============================

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? fromDate,
            DateTime? toDate,
            string? paymentStatus)
        {
            var filter = new OrderHistoryFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                PaymentStatus = paymentStatus
            };

            var orders = await _orderService.GetHistoryAsync(filter);

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.PaymentStatus = paymentStatus;

            return View(orders);
        }


        // ==============================
        // ORDER DETAILS
        // ==============================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var result = await _orderService.GetByIdAsync(id);

            if (result.Order == null)
            {
                return NotFound();
            }

            ViewBag.Items = result.Items;

            return View(result.Order);
        }


        // ==============================
        // DAILY SALES REPORT
        // ==============================

        [HttpGet]
        public async Task<IActionResult> Reports(DateTime? date)
        {
            var selectedDate = date?.Date ?? DateTime.Today;

            var summary =
                await _orderService.GetDailySalesSummaryAsync(selectedDate);

            ViewBag.SelectedDate = selectedDate;

            return View(summary);
        }


        // ==============================
        // EXPORT CSV
        // ==============================

        [HttpGet]
        public async Task<IActionResult> ExportCsv(DateTime? date)
        {
            var selectedDate = date?.Date ?? DateTime.Today;

            var orders = await _orderService.GetHistoryAsync(
                new OrderHistoryFilterDto
                {
                    FromDate = selectedDate,
                    ToDate = selectedDate.AddDays(1).AddTicks(-1),
                    PaymentStatus = "Paid"
                });

            var csv = new System.Text.StringBuilder();

            csv.AppendLine(
                "Order Number,Order Date,Subtotal,Discount,Tax,Grand Total,Payment Status");

            foreach (var order in orders)
            {
                csv.AppendLine(
                    $"\"{order.OrderNumber}\"," +
                    $"\"{order.OrderDate:yyyy-MM-dd HH:mm:ss}\"," +
                    $"{order.Subtotal:F2}," +
                    $"{order.DiscountAmount:F2}," +
                    $"{order.TaxAmount:F2}," +
                    $"{order.GrandTotal:F2}," +
                    $"\"{order.PaymentStatus}\"");
            }

            var fileName =
                $"DailySales_{selectedDate:yyyyMMdd}.csv";

            return File(
                System.Text.Encoding.UTF8.GetBytes(csv.ToString()),
                "text/csv",
                fileName);
        }


        // ==============================
        // EXPORT PDF
        // ==============================

        [HttpGet]
        public async Task<IActionResult> ExportPdf(DateTime? date)
        {
            var selectedDate = date?.Date ?? DateTime.Today;

            var orders = await _orderService.GetHistoryAsync(
                new OrderHistoryFilterDto
                {
                    FromDate = selectedDate,
                    ToDate = selectedDate.AddDays(1).AddTicks(-1),
                    PaymentStatus = "Paid"
                });

            var orderList = orders.ToList();

            var totalOrders = orderList.Count;

            var totalRevenue =
                orderList.Sum(x => x.GrandTotal);

            var totalDiscount =
                orderList.Sum(x => x.DiscountAmount);


            // QuestPDF License
            QuestPDF.Settings.License =
                LicenseType.Community;


            // Create PDF
            var document =
                QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // Page Settings
                        page.Size(PageSizes.A4);

                        page.Margin(40);

                        page.DefaultTextStyle(
                            x => x.FontSize(10));


                        // ==========================
                        // HEADER
                        // ==========================

                        page.Header()
                            .Column(column =>
                            {
                                column.Item()
                                    .Text(
                                        "POS Billing & Inventory Management System")
                                    .Bold()
                                    .FontSize(18);

                                column.Item()
                                    .PaddingTop(5)
                                    .Text("Daily Sales Report")
                                    .Bold()
                                    .FontSize(14);

                                column.Item()
                                    .PaddingTop(5)
                                    .Text(
                                        $"Date: {selectedDate:dd-MM-yyyy}")
                                    .FontSize(10);
                            });


                        // ==========================
                        // CONTENT
                        // ==========================

                        page.Content()
                            .PaddingTop(20)
                            .Column(column =>
                            {
                                if (orderList.Count == 0)
                                {
                                    column.Item()
                                        .Text(
                                            "No paid orders found for the selected date.")
                                        .FontSize(12);
                                }
                                else
                                {
                                    column.Item()
                                        .Table(table =>
                                        {
                                            // ======================
                                            // TABLE COLUMNS
                                            // ======================

                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1.5f);
                                            });


                                            // ======================
                                            // TABLE HEADER
                                            // ======================

                                            table.Header(header =>
                                            {
                                                header.Cell()
                                                    .Background(
                                                        Colors.Grey.Lighten2)
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text("Order Number")
                                                    .Bold();

                                                header.Cell()
                                                    .Background(
                                                        Colors.Grey.Lighten2)
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text("Order Date")
                                                    .Bold();

                                                header.Cell()
                                                    .Background(
                                                        Colors.Grey.Lighten2)
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text("Subtotal")
                                                    .Bold();

                                                header.Cell()
                                                    .Background(
                                                        Colors.Grey.Lighten2)
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text("Discount")
                                                    .Bold();

                                                header.Cell()
                                                    .Background(
                                                        Colors.Grey.Lighten2)
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text("Tax")
                                                    .Bold();

                                                header.Cell()
                                                    .Background(
                                                        Colors.Grey.Lighten2)
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text("Grand Total")
                                                    .Bold();
                                            });


                                            // ======================
                                            // TABLE DATA
                                            // ======================

                                            foreach (var order in orderList)
                                            {
                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text(
                                                        order.OrderNumber);

                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text(
                                                        order.OrderDate
                                                            .ToString(
                                                                "dd-MM-yyyy HH:mm"));

                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text(
                                                        $"Rs. {order.Subtotal:N2}");

                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text(
                                                        $"Rs. {order.DiscountAmount:N2}");

                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text(
                                                        $"Rs. {order.TaxAmount:N2}");

                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .Text(
                                                        $"Rs. {order.GrandTotal:N2}");
                                            }
                                        });
                                }


                                // ==========================
                                // SUMMARY
                                // ==========================

                                column.Item()
                                    .PaddingTop(20)
                                    .Text(
                                        $"Total Orders: {totalOrders}")
                                    .Bold()
                                    .FontSize(11);

                                column.Item()
                                    .PaddingTop(5)
                                    .Text(
                                        $"Total Revenue: Rs. {totalRevenue:N2}")
                                    .Bold()
                                    .FontSize(11);

                                column.Item()
                                    .PaddingTop(5)
                                    .Text(
                                        $"Total Discount: Rs. {totalDiscount:N2}")
                                    .Bold()
                                    .FontSize(11);
                            });


                        // ==========================
                        // FOOTER
                        // ==========================

                        page.Footer()
                            .AlignCenter()
                            .Text(
                                $"Generated on {DateTime.Now:dd-MM-yyyy HH:mm:ss}");
                    });
                });


            // Generate PDF
            var pdfBytes = document.GeneratePdf();


            // File Name
            var fileName =
                $"DailySales_{selectedDate:yyyyMMdd}.pdf";


            // Return PDF
            return File(
                pdfBytes,
                "application/pdf",
                fileName);
        }
    }
}