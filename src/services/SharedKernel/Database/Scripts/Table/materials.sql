IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Materials' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Materials (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CategoryId UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(255) NOT NULL,
        Shape NVARCHAR(100) NOT NULL,
        Thickness DECIMAL(18, 2) NULL,
        Width DECIMAL(18, 2) NULL,
        StandardLength DECIMAL(18, 2) NULL,
        SoldBy NVARCHAR(50) NOT NULL,
        GlobalRate DECIMAL(18, 2) NOT NULL DEFAULT 0.0,
        LastPurchaseRate DECIMAL(18, 2) NOT NULL DEFAULT 0.0,
        StockMT DECIMAL(18, 2) NOT NULL DEFAULT 0.0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Materials_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
    );
    PRINT 'Created Materials table'
END
GO
