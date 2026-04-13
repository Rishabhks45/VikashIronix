CREATE OR ALTER PROCEDURE [dbo].[usp_User_Profile_Management]
    @Action NVARCHAR(50),
    @UserId UNIQUEIDENTIFIER = NULL,
    @FirstName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100) = NULL,
    @Email NVARCHAR(256) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,
    @Bio NVARCHAR(MAX) = NULL,
    @Location NVARCHAR(200) = NULL,
    @PasswordHash NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'GET'
    BEGIN
        SELECT 
            u.id,
            u.FirstName,
            u.LastName,
            u.email,
            u.PhoneNumber,
            u.Bio,
            u.Location,
            -- Fixed alias to match UserDto property 'Role'
            r.Name AS Role,
            u.is_active,
            u.created_at
        FROM users u
        LEFT JOIN roles r ON u.role_id = r.id
        WHERE u.id = @UserId;
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE users
        SET 
            FirstName = COALESCE(@FirstName, FirstName),
            LastName = COALESCE(@LastName, LastName),
            email = COALESCE(@Email, email),
            PhoneNumber = COALESCE(@PhoneNumber, PhoneNumber),
            Bio = COALESCE(@Bio, Bio),
            Location = COALESCE(@Location, Location),
            updated_at = SYSDATETIME()
        WHERE id = @UserId;
        
        SELECT 'Profile updated successfully' AS Message;
    END
    ELSE IF @Action = 'CHANGE_PASSWORD'
    BEGIN
        UPDATE users
        SET 
            password_hash = @PasswordHash,
            updated_at = SYSDATETIME()
        WHERE id = @UserId;
        
        SELECT 'Password updated successfully' AS Message;
    END
END
GO
