-- Create SalaryConfigurations Table (Simplified)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalaryConfigurations' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE SalaryConfigurations (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        MonthlySalary DECIMAL(18, 2) NOT NULL DEFAULT 0,
        EffectiveDate DATE NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_SalaryConfigs_Users FOREIGN KEY (UserId) REFERENCES users(id) ON DELETE CASCADE
    );
    PRINT 'Created SalaryConfigurations table'
END
ELSE
BEGIN
    -- If already exists, we might need to alter it if we want to force the change
    -- But since I just created it, I'll just drop and recreate for simplicity in this dev phase
    DROP TABLE SalaryConfigurations;
    CREATE TABLE SalaryConfigurations (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        MonthlySalary DECIMAL(18, 2) NOT NULL DEFAULT 0,
        EffectiveDate DATE NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_SalaryConfigs_Users FOREIGN KEY (UserId) REFERENCES users(id) ON DELETE CASCADE
    );
END
GO
