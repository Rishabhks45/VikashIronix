IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_Customer_Management')
    DROP PROCEDURE usp_Customer_Management
GO

CREATE PROCEDURE [dbo].[usp_Customer_Management]
(
    @QueryType          INT,                    
    @Id                 UNIQUEIDENTIFIER = NULL,
    @UserId             UNIQUEIDENTIFIER = NULL,
    @FirstName          NVARCHAR(100) = NULL,
    @LastName           NVARCHAR(100) = NULL,
    @PhoneNumber        NVARCHAR(20) = NULL,
    @Email              NVARCHAR(255) = NULL,
    @ShopName           NVARCHAR(255) = NULL,
    @ShopAddress        NVARCHAR(MAX) = NULL,
    @Bhada              DECIMAL(18,2) = NULL,
    @DisplayIndex       INT = NULL,
    @Password NVARCHAR(MAX) = NULL,
    @RoleId             INT = NULL,
    @IsActive           BIT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------
    -- 1. GET ALL CUSTOMERS
    ----------------------------------------------------
    IF (@QueryType = 1)
    BEGIN
        SELECT 
            c.Id AS Id,
            c.UserId AS UserId,
            u.FirstName AS FirstName,
            u.LastName AS LastName,
            u.FirstName + ' ' + u.LastName AS CustomerName,
            u.PhoneNumber AS PhoneNumber,
            u.Email AS Email,
            u.role_id AS RoleId,
            c.ShopName AS ShopName,
            c.ShopAddress AS ShopAddress,
            c.Bhada AS Bhada,
            c.DisplayIndex AS DisplayIndex,
            c.IsActive AS IsActive
        FROM customers c
        INNER JOIN users u ON c.UserId = u.Id
        ORDER BY c.DisplayIndex ASC, c.CreatedAt DESC;

        RETURN;
    END

    ----------------------------------------------------
    -- 2. INSERT CUSTOMER
    ----------------------------------------------------
    IF (@QueryType = 2)
    BEGIN
        DECLARE @NewUserId UNIQUEIDENTIFIER = NEWID();

        -- 1. Insert into Users 
        INSERT INTO users (id, FirstName, LastName, PhoneNumber, email, password_hash, role_id, is_active, created_at, updated_at)
        VALUES (@NewUserId, @FirstName, ISNULL(@LastName, ''), @PhoneNumber, @Email, @Password, @RoleId, ISNULL(@IsActive, 1), SYSDATETIME(), SYSDATETIME());

        -- 2. Insert into Customers
        INSERT INTO customers (Id, UserId, ShopName, ShopAddress, Bhada, DisplayIndex, IsActive, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @NewUserId, @ShopName, @ShopAddress, ISNULL(@Bhada, 0.00), @DisplayIndex, ISNULL(@IsActive, 1), SYSDATETIME(), SYSDATETIME());
        
        RETURN;
    END

    ----------------------------------------------------
    -- 3. UPDATE CUSTOMER
    ----------------------------------------------------
    IF (@QueryType = 3)
    BEGIN
        -- Update linked User contact details
        UPDATE users
        SET FirstName = ISNULL(@FirstName, FirstName),
            LastName = ISNULL(@LastName, LastName),
            PhoneNumber = ISNULL(@PhoneNumber, PhoneNumber),
            email = ISNULL(@Email, email),
            role_id = ISNULL(@RoleId, role_id),
            is_active = ISNULL(@IsActive, is_active),
            updated_at = SYSDATETIME()
        WHERE id = @UserId;

        -- Update specific Customer profiles details
        UPDATE customers
        SET ShopName = ISNULL(@ShopName, ShopName),
            ShopAddress = ISNULL(@ShopAddress, ShopAddress),
            Bhada = ISNULL(@Bhada, Bhada),
            DisplayIndex = ISNULL(@DisplayIndex, DisplayIndex),
            IsActive = ISNULL(@IsActive, IsActive),
            UpdatedAt = SYSDATETIME()
        WHERE Id = @Id;
        
        RETURN;
    END

    ----------------------------------------------------
    -- 4. DELETE CUSTOMER
    ----------------------------------------------------
    IF (@QueryType = 4)
    BEGIN
        -- Option: Delete User linked to this Customer or just delete Customer profile. We delete the profile mapping first.
        DELETE FROM customers WHERE Id = @Id;
        RETURN;
    END

END
GO
