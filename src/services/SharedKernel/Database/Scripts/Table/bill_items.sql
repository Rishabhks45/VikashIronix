IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'bill_items' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE bill_items (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        BillId UNIQUEIDENTIFIER NOT NULL,
        MaterialId UNIQUEIDENTIFIER NOT NULL,
        Quantity DECIMAL(18, 3) NOT NULL DEFAULT 0.000,
        UnitPrice DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
        TaxPercentage DECIMAL(18, 2) NOT NULL DEFAULT 18.00,
        TotalPrice DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_BillItems_Bills FOREIGN KEY (BillId) REFERENCES bills(Id) ON DELETE CASCADE,
        CONSTRAINT FK_BillItems_Materials FOREIGN KEY (MaterialId) REFERENCES materials(Id)
    );
    PRINT 'Created bill_items table'
END
GO
