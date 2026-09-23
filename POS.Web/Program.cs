using POS.BLL.Interfaces;
using POS.BLL.Services;
using POS.DAL.Context;
using POS.DAL.Interfaces;
using POS.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DAL
builder.Services.AddScoped<DapperContext>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// BLL
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
