IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE customers (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        ShopName NVARCHAR(255) NULL,
        ShopAddress NVARCHAR(MAX) NULL,
        Bhada DECIMAL(18,2) NOT NULL DEFAULT 0.00,
        DisplayIndex INT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Customers_Users FOREIGN KEY (UserId) REFERENCES users(Id)
    );
END
GO
