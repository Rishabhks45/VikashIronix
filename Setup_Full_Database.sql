-- =============================================
-- Database Setup Script for Document Parser
-- =============================================

-- 1. Create Schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'identity')
BEGIN
    EXEC('CREATE SCHEMA [identity]')
END
GO

-- 2. Create Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'users' AND schema_id = SCHEMA_ID('identity'))
BEGIN
    CREATE TABLE users (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        FirstName nvarchar(100) NOT NULL,
        LastName nvarchar(100) NOT NULL,
        PhoneNumber nvarchar(20) NULL,
        LastLoginAt datetime2 NULL,
        RefreshToken nvarchar(500) NULL,
        RefreshTokenExpiry datetime2 NULL,
        email NVARCHAR(255) NOT NULL UNIQUE,
        password_hash NVARCHAR(MAX) NOT NULL,
        is_active BIT NOT NULL DEFAULT 1,
        deleted_at DATETIME2 NULL,
        created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    );
    PRINT 'Created Users table'
END
GO

-- 3. Create Roles Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'roles' AND schema_id = SCHEMA_ID('identity'))
BEGIN
    CREATE TABLE roles (
        id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(50) NOT NULL UNIQUE,
        description NVARCHAR(255) NULL
    );
    PRINT 'Created Roles table'
END
GO

-- 4. Create User Roles Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'user_roles' AND schema_id = SCHEMA_ID('identity'))
BEGIN
    CREATE TABLE user_roles (
        user_id UNIQUEIDENTIFIER NOT NULL,
        role_id INT NOT NULL,
        PRIMARY KEY (user_id, role_id),
        FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
        FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE
    );
    PRINT 'Created User Roles table'
END
GO

-- 5. Create Documents Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Documents' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Documents (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        FileName NVARCHAR(255) NOT NULL,
        StoragePath NVARCHAR(MAX) NOT NULL,
        ContentType NVARCHAR(100) NULL,
        FileSize BIGINT NULL,
        Status INT NOT NULL DEFAULT 0, -- 0: Pending, 1: Processed, 2: Failed
        ExtractedData NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Documents_Users FOREIGN KEY (UserId) REFERENCES users(id) ON DELETE CASCADE
    );
    PRINT 'Created Documents table'
END
GO

-- 6. Create Categories Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Categories (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(255) NOT NULL,
        ParentId UNIQUEIDENTIFIER NULL,
        HsnCode NVARCHAR(50) NULL,
        IsRoot BIT NOT NULL DEFAULT 1,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentId) REFERENCES Categories(Id)
    );
    PRINT 'Created Categories table'
END
GO

-- 7. Create Materials Table
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

-- 8. Create PasswordResetTokens Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PasswordResetTokens' AND schema_id = SCHEMA_ID('identity'))
BEGIN
    CREATE TABLE PasswordResetTokens (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId UNIQUEIDENTIFIER NOT NULL,
        ResetToken NVARCHAR(256) NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ExpiresAtUtc DATETIME2 NOT NULL,
        IsUsed BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_PasswordResetTokens_Users FOREIGN KEY (UserId) REFERENCES users(id) ON DELETE CASCADE
    );
    PRINT 'Created PasswordResetTokens table'
END
GO

-- 7. Create/Update Login SP
CREATE OR ALTER PROCEDURE [dbo].[usp_Login_Users]
(
    @QueryType          INT,                    
    @Id                 UNIQUEIDENTIFIER = NULL,
    @Email              NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. GET SINGLE RECORD BY ID
    IF (@QueryType = 1)
    BEGIN
        SELECT 
            u.id AS UserId,
            u.FirstName,
            u.LastName,
            CONCAT(u.FirstName, ' ', u.LastName) AS UserName,
            u.email AS Email,
            u.password_hash AS Password,
            u.PhoneNumber,
            u.LastLoginAt,
            u.RefreshToken,
            u.RefreshTokenExpiry,
            u.is_active AS IsActive,
            0 AS IsDeleted,
            u.created_at AS CreatedAt,
            u.updated_at AS UpdatedAt,
            NULL AS MapId,
            NULL AS OrganizationId,
            3 AS UserType,
            r.id AS RoleId,
            r.name AS RoleName
        FROM users u
        LEFT JOIN user_roles ur ON ur.user_id = u.id
        LEFT JOIN roles r ON r.id = ur.role_id
        WHERE u.id = @Id AND u.is_active = 1;
        RETURN;
    END

    -- 2. GET SINGLE RECORD BY EMAIL
    IF (@QueryType = 2)
    BEGIN
        SELECT 
            u.id AS UserId,
            u.FirstName,
            u.LastName,
            CONCAT(u.FirstName, ' ', u.LastName) AS UserName,
            u.email AS Email,
            u.password_hash AS Password,
            u.PhoneNumber,
            u.LastLoginAt,
            u.RefreshToken,
            u.RefreshTokenExpiry,
            u.is_active AS IsActive,
            0 AS IsDeleted,
            u.created_at AS CreatedAt,
            u.updated_at AS UpdatedAt,
            NULL AS MapId,
            NULL AS OrganizationId,
            3 AS UserType,
            r.id AS RoleId,
            r.name AS RoleName
        FROM users u
        LEFT JOIN user_roles ur ON ur.user_id = u.id
        LEFT JOIN roles r ON r.id = ur.role_id
        WHERE u.email = @Email AND u.is_active = 1;
        RETURN;
    END
END
GO
PRINT 'Created usp_Login_Users'
GO

-- 8. Create/Update Password Reset SP
CREATE OR ALTER PROCEDURE [dbo].[usp_Auth_PasswordReset]
    @QueryType INT,
    @UserId UNIQUEIDENTIFIER = NULL,
    @ResetToken NVARCHAR(256) = NULL,
    @ExpiresAtUtc DATETIME2 = NULL,
    @Password NVARCHAR(MAX) = NULL
AS
BEGIN
    -- QueryType 1: Save Password Reset Token
    IF @QueryType = 1
    BEGIN
        INSERT INTO PasswordResetTokens (UserId, ResetToken, CreatedAtUtc, ExpiresAtUtc, IsUsed)
        VALUES (@UserId, @ResetToken, GETUTCDATE(), @ExpiresAtUtc, 0);
    END
    -- QueryType 2: Get User By Reset Token
    ELSE IF @QueryType = 2
    BEGIN
        SELECT 
            u.id as UserId,
            u.email as Email,
            u.FirstName,
            u.LastName,
            u.password_hash as Password,
            u.is_active as IsActive
        FROM PasswordResetTokens t
        JOIN users u ON t.UserId = u.id
        WHERE t.ResetToken = @ResetToken 
          AND t.IsUsed = 0 
          AND t.ExpiresAtUtc > GETUTCDATE();
    END
    -- QueryType 3: Mark Token As Used
    ELSE IF @QueryType = 3
    BEGIN
        UPDATE PasswordResetTokens 
        SET IsUsed = 1 
        WHERE ResetToken = @ResetToken;
    END
    -- QueryType 4: Update User Password
    ELSE IF @QueryType = 4
    BEGIN
        UPDATE users
        SET password_hash = @Password,
            updated_at = SYSDATETIME()
        WHERE id = @UserId;
    END
END
GO
PRINT 'Created usp_Auth_PasswordReset'
GO

-- 11. Create/Update Inventory Category SP
CREATE OR ALTER PROCEDURE [dbo].[usp_Inventory_Category]
    @QueryType INT,
    @Name NVARCHAR(255) = NULL,
    @ParentId UNIQUEIDENTIFIER = NULL,
    @HsnCode NVARCHAR(50) = NULL,
    @IsRoot BIT = 1,
    @Id UNIQUEIDENTIFIER = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- QueryType 1: Insert Category
    IF @QueryType = 1
    BEGIN
        SET @Id = NEWID();
        INSERT INTO Categories (Id, Name, ParentId, HsnCode, IsRoot, IsActive, CreatedAt, UpdatedAt)
        VALUES (@Id, @Name, @ParentId, @HsnCode, @IsRoot, 1, SYSDATETIME(), SYSDATETIME());
    END

    -- QueryType 2: Get Active Categories
    IF @QueryType = 2
    BEGIN
        SELECT * FROM Categories WHERE IsActive = 1 ORDER BY CreatedAt DESC;
    END

    -- QueryType 3: Update Category
    IF @QueryType = 3
    BEGIN
        UPDATE Categories 
        SET Name = @Name,
            ParentId = @ParentId,
            HsnCode = @HsnCode,
            IsRoot = @IsRoot,
            UpdatedAt = SYSDATETIME()
        WHERE Id = @Id;
    END

    -- QueryType 4: Soft Delete Category (Set IsActive = 0)
    IF @QueryType = 4
    BEGIN
        UPDATE Categories SET IsActive = 0, UpdatedAt = SYSDATETIME() WHERE Id = @Id;
        UPDATE Categories SET IsActive = 0, UpdatedAt = SYSDATETIME() WHERE ParentId = @Id;
    END
END
GO
PRINT 'Created usp_Inventory_Category'
GO

-- 12. Create/Update Inventory Material SP
CREATE OR ALTER PROCEDURE [dbo].[usp_Inventory_Material]
    @QueryType INT,
    @CategoryId UNIQUEIDENTIFIER = NULL,
    @Name NVARCHAR(255) = NULL,
    @Shape NVARCHAR(100) = NULL,
    @Thickness DECIMAL(18, 2) = NULL,
    @Width DECIMAL(18, 2) = NULL,
    @StandardLength DECIMAL(18, 2) = NULL,
    @SoldBy NVARCHAR(50) = NULL,
    @GlobalRate DECIMAL(18, 2) = 0,
    @Id UNIQUEIDENTIFIER = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- QueryType 1: Insert Material
    IF @QueryType = 1
    BEGIN
        SET @Id = NEWID();
        INSERT INTO Materials (Id, CategoryId, Name, Shape, Thickness, Width, StandardLength, SoldBy, GlobalRate, StockMT, IsActive, CreatedAt, UpdatedAt)
        VALUES (@Id, @CategoryId, @Name, @Shape, @Thickness, @Width, @StandardLength, @SoldBy, @GlobalRate, 0.0, 1, SYSDATETIME(), SYSDATETIME());
    END

    -- QueryType 2: Get Active Materials
    IF @QueryType = 2
    BEGIN
        SELECT m.*, c.Name as CategoryName 
        FROM Materials m
        INNER JOIN Categories c ON m.CategoryId = c.Id
        WHERE m.IsActive = 1 
        ORDER BY m.CreatedAt DESC;
    END
END
GO
PRINT 'Created usp_Inventory_Material'
GO

-- 9. Insert Super Admin User
DECLARE @UserId UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleId INT;
DECLARE @Email NVARCHAR(255) = 'superadmin@admin.com';
DECLARE @PasswordHash NVARCHAR(MAX) = 'tqJSY4/fcKJ1Ol07SFn0q/4oQV3LRRpBoSxnczfdfnWQLjMnUXseLNtbmo3+sRl17XN742g=';

-- Get or Create Super Admin Role
SELECT @RoleId = id FROM roles WHERE name = 'Super Admin';
IF @RoleId IS NULL
BEGIN
    INSERT INTO roles (name, description) VALUES ('Super Admin', 'Full system access');
    SET @RoleId = SCOPE_IDENTITY();
END

-- Insert User if not exists
IF NOT EXISTS (SELECT 1 FROM users WHERE email = @Email)
BEGIN
    INSERT INTO users (
        id, FirstName, LastName, PhoneNumber, email, password_hash, is_active, created_at, updated_at
    )
    VALUES (
        @UserId, 'Super', 'Admin', '1234567890', @Email, @PasswordHash, 1, SYSDATETIME(), SYSDATETIME()
    );
    
    INSERT INTO user_roles (user_id, role_id) VALUES (@UserId, @RoleId);
    PRINT 'Super Admin inserted'
END
ELSE
BEGIN
    PRINT 'Super Admin already exists'
END
GO

