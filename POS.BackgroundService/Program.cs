using POS.BackgroundService;
using POS.BLL.Interfaces;
using POS.BLL.Services;
using POS.DAL.Context;
using POS.DAL.Interfaces;
using POS.DAL.Repositories;

var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddHostedService<POS.BackgroundService.Worker>();

var host = builder.Build();
host.Run();
