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

    -- QueryType 2: Get User By Reset Token (Valid & Unused)
    ELSE IF @QueryType = 2
    BEGIN
        SELECT 
            u.id as UserId,
            u.Email as Email,
            u.FirstName,
            u.LastName,
            u.PasswordHash as Password,
            u.IsActive as IsActive
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
        SET PasswordHash = @Password,
            UpdatedAt = SYSDATETIME()
        WHERE id = @UserId;
    END
END

