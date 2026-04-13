-- Create/Update Login SP
CREATE OR ALTER PROCEDURE [dbo].[usp_Login_Users]
(
    @QueryType          INT,                    
    @Id                 UNIQUEIDENTIFIER = NULL,
    @Email              NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- 2. GET SINGLE RECORD BY EMAIL (Used during Login)
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
            -- Prioritize the role_id on the users table, fallback to user_roles
            COALESCE(u.role_id, (SELECT TOP 1 role_id FROM user_roles WHERE user_id = u.id)) AS RoleId,
            -- Join with roles table based on the selected RoleId
            r.name AS RoleName
        FROM users u
        LEFT JOIN roles r ON r.id = COALESCE(u.role_id, (SELECT TOP 1 role_id FROM user_roles WHERE user_id = u.id))
        WHERE u.email = @Email AND u.is_active = 1;
        RETURN;
    END

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
            u.is_active AS IsActive,
            u.created_at AS CreatedAt,
            u.updated_at AS UpdatedAt,
            r.id AS RoleId,
            r.name AS RoleName
        FROM users u
        LEFT JOIN roles r ON r.id = COALESCE(u.role_id, (SELECT TOP 1 role_id FROM user_roles WHERE user_id = u.id))
        WHERE u.id = @Id;
        RETURN;
    END
END
GO