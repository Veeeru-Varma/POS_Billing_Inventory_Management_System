USE POSBillingDB;
GO

-- Remove all existing data and reset identity values
TRUNCATE TABLE WebhookLogs;
TRUNCATE TABLE Payments;
TRUNCATE TABLE OrderItems;
TRUNCATE TABLE Orders;
TRUNCATE TABLE Products;
TRUNCATE TABLE TaxSettings;
GO

-- Add GST configuration again
INSERT INTO TaxSettings
(
    TaxName,
    TaxPercentage,
    IsActive,
    UpdatedAt
)
VALUES
(
    'GST',
    18.00,
    1,
    GETDATE()
);
GO