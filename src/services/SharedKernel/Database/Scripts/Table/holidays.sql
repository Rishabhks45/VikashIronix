-- 1. Create Holidays Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Holidays' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Holidays (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(255) NOT NULL,
        Date DATE NOT NULL,
        Type INT NOT NULL, -- 1: Public, 2: Restricted
        Description NVARCHAR(MAX) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    );
    PRINT 'Created Holidays table'
END
GO
