# POS Billing & Inventory Management System

A POS application built with .NET 8, ASP.NET Core MVC, Web API, SQL Server and Dapper.

The application covers product management, billing, inventory, payment processing, order history and sales reports.

## Features

- Product CRUD and search
- Inventory and low-stock tracking
- POS billing with cart management
- Percentage and fixed discounts
- GST calculation
- Order creation and order history
- Payment webhook for successful and failed payments
- Inventory deduction after successful payment
- Daily sales report
- CSV and PDF export
- Payment API integrated with the POS web billing flow
- Background service for low-stock CSV reporting

## Technology Stack

- C#
- .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- SQL Server
- Dapper
- Stored Procedures
- Bootstrap
- Razor Views
- Swagger
- QuestPDF
- .NET BackgroundService
- Git and GitHub

## Project Structure

```text
POSBillingSystem
│
├── Database
│   ├── POSBillingDB.sql
│   └── ClearPOSBillingDB.sql
│
├── POS.Web
├── POS.API
├── POS.BLL
├── POS.DAL
├── POS.Models
├── POS.BackgroundService
│
├── .gitignore
├── README.md
└── POSBillingSystem.sln
```

## Architecture

The project follows a layered architecture.

```text
POS.Web
   ↓
POS.BLL
   ↓
POS.DAL
   ↓
POS.Models
   ↓
SQL Server
```

`POS.API` and `POS.BackgroundService` also use the BLL, DAL and Models layers.

## Database

Database name:

```text
POSBillingDB
```

The complete database script is available at:

```text
Database/POSBillingDB.sql
```

The script creates the database, tables, stored procedures, GST configuration and sample products.

A separate cleanup script is available at `Database/ClearPOSBillingDB.sql`. It clears existing test data and adds the GST configuration again for fresh testing.

Main tables:

- Products
- Orders
- OrderItems
- Payments
- WebhookLogs
- TaxSettings

## Setup

### Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server
- SQL Server Management Studio

### 1. Clone the Repository

```text
https://github.com/Veeeru-Varma/POS_Billing_Inventory_Management_System
```

Open `POSBillingSystem.sln` in Visual Studio 2022.

### 2. Create the Database

Open SQL Server Management Studio.

Open:

```text
Database/POSBillingDB.sql
```

Run the complete script.

This creates `POSBillingDB` and all required tables and stored procedures.

### 3. Configure SQL Server

Update the SQL Server name in:

```text
POS.Web/appsettings.json
POS.API/appsettings.json
POS.BackgroundService/appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=POSBillingDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Replace `YOUR_SERVER_NAME` with the SQL Server instance available on your machine.

No other database configuration is required for the project setup.

### 4. Restore and Build

Open the solution in Visual Studio and restore the NuGet packages.

Then select:

```text
Build → Rebuild Solution
```

Make sure the solution builds successfully.

## Running the Application

### POS.Web

Set `POS.Web` as the startup project and run it.

The application contains:

- Billing
- Products
- Orders
- Low Stock
- Reports

### POS.API

Run `POS.API` separately when testing the payment webhook.

Swagger is available for API testing.

### POS.BackgroundService

Run `POS.BackgroundService` to execute the low-stock reporting service.

## Product Management

The Products section supports:

- Add product
- Edit product
- Search product
- Delete product
- Update stock
- Low-stock threshold

Products are marked inactive when deleted.

## Billing

The Billing screen supports:

1. Product search
2. Add product to cart
3. Quantity changes
4. Remove product
5. Percentage discount
6. Fixed discount
7. GST calculation
8. Grand total calculation
9. Order creation
10. Payment processing

After completing a sale, the order is created with payment status `Pending`.

The POS web application is integrated with the Payment API, so successful and failed payments can be simulated directly from the Billing screen.

## Payment Webhook

A simulated payment gateway webhook is implemented using ASP.NET Core Web API.

Endpoint:

```text
POST /api/PaymentWebhook/webhook
```

Header:

```text
X-Webhook-Secret
```

Example successful payment request:

```json
{
  "order_id": "YOUR_ORDER_NUMBER",
  "status": "success",
  "amount": 29.50,
  "transaction_id": "TXN-SUCCESS-001"
}
```

After successful payment:

```text
Payment Status = Paid
Order Status   = Completed
```

Inventory is reduced after successful payment.

Example failed payment request:

```json
{
  "order_id": "YOUR_ORDER_NUMBER",
  "status": "failed",
  "amount": 29.50,
  "transaction_id": "TXN-FAIL-001"
}
```

For a failed payment, the payment is marked as failed and inventory is not reduced.

Webhook requests are stored in the `WebhookLogs` table.

## Payment Flow from Web UI

The payment API is connected with the POS billing screen.

```text
Billing
   ↓
Complete Sale
   ↓
Order Created
   ↓
Payment Pending
   ↓
Successful / Failed Payment
   ↓
POS.API Payment Webhook
   ↓
Payment Processing
   ↓
SQL Server
```

Successful payment:

```text
Payment = Paid
Order = Completed
Inventory = Reduced
```

Failed payment:

```text
Payment = Failed
Inventory = Unchanged
```

## Orders

The Orders section provides:

- Order history
- Date filtering
- Payment status filtering
- Order details
- Order line items
- Payment status
- Order status

## Reports

The Reports section provides:

- Total orders
- Total revenue
- Total discount
- Daily sales report
- Low-stock report
- CSV export
- PDF export

Only successfully paid orders are included in the daily sales summary.

PDF reports are generated using QuestPDF.

## Background Service

`POS.BackgroundService` handles low-stock reporting.

It retrieves products where:

```text
StockQuantity <= LowStockThreshold
```

and generates a low-stock CSV report.

## Stored Procedures

Stored procedures are used for database operations and critical business operations.

### Product

```text
sp_Product_GetAll
sp_Product_GetById
sp_Product_Search
sp_Product_Insert
sp_Product_Update
sp_Product_Delete
sp_Product_GetLowStock
```

### Tax

```text
sp_Tax_GetActive
```

### Orders

```text
sp_Order_Create
sp_Order_GetById
sp_Order_GetHistory
sp_Order_GetDailySalesSummary
```

### Payment / Webhook

```text
sp_WebhookLog_Insert
sp_ProcessPaymentWebhook
```

## End-to-End Flow

```text
Product Management
      ↓
Billing
      ↓
Add Products to Cart
      ↓
Discount + GST
      ↓
Create Order
      ↓
Payment Pending
      ↓
Payment from Web UI
      ↓
POS.API Payment Webhook
      ↓
Success / Failed
      ↓
Success → Inventory Updated
Failed  → Inventory Unchanged
      ↓
Order History
      ↓
Sales Reports
      ↓
CSV / PDF Export
```

## Database Cleanup

For fresh testing, run:

```text
Database/ClearPOSBillingDB.sql
```

The script clears existing Products, Orders, OrderItems, Payments, WebhookLogs and TaxSettings data, then adds the GST configuration again.

## GitHub Repository

```text
https://github.com/Veeeru-Varma/POS_Billing_Inventory_Management_System
```

The repository contains the application source code, database script, solution file, `.gitignore` and README.

## Conclusion

This project demonstrates a complete POS workflow using ASP.NET Core MVC, ASP.NET Core Web API, C#, .NET 8, SQL Server, Dapper, stored procedures, Bootstrap, QuestPDF and .NET BackgroundService.
