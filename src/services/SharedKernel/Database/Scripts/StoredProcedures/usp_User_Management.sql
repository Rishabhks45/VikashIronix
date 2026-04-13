IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_User_Management')
    DROP PROCEDURE usp_User_Management
GO

CREATE PROCEDURE [dbo].[usp_User_Management]
(
    @QueryType          INT,                    
    @Id                 UNIQUEIDENTIFIER = NULL,
    @FirstName          NVARCHAR(100) = NULL,
    @LastName           NVARCHAR(100) = NULL,
    @Email              NVARCHAR(255) = NULL,
    @RoleId             INT = NULL,
    @IsActive           BIT = NULL,
    @PasswordHash       NVARCHAR(MAX) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------
    -- 1. GET ALL USERS (List)
    ----------------------------------------------------
    IF (@QueryType = 1)
    BEGIN
        SELECT 
            u.id AS Id,
            u.FirstName,
            u.LastName,
            u.email AS Email,
            r.name AS Role,
            u.role_id AS RoleId,
            u.LastLoginAt,
            u.is_active AS IsActive
        FROM users u
        LEFT JOIN roles r ON u.role_id = r.id
        WHERE ISNULL(r.name, '') != 'Customer'
        ORDER BY u.created_at DESC;

        RETURN;
    END

    ----------------------------------------------------
    -- 2. INSERT USER
    ----------------------------------------------------
    IF (@QueryType = 2)
    BEGIN
        INSERT INTO users (id, FirstName, LastName, email, role_id, is_active, password_hash, created_at, updated_at)
        VALUES (NEWID(), @FirstName, @LastName, @Email, @RoleId, @IsActive, @PasswordHash, SYSDATETIME(), SYSDATETIME());
        RETURN;
    END

    ----------------------------------------------------
    -- 3. UPDATE USER
    ----------------------------------------------------
    IF (@QueryType = 3)
    BEGIN
        UPDATE users
        SET FirstName = ISNULL(@FirstName, FirstName),
            LastName = ISNULL(@LastName, LastName),
            email = ISNULL(@Email, email),
            role_id = ISNULL(@RoleId, role_id),
            is_active = ISNULL(@IsActive, is_active),
            updated_at = SYSDATETIME()
        WHERE id = @Id;
        RETURN;
    END

END
GO
