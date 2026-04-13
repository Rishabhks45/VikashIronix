IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_Role_Management')
    DROP PROCEDURE usp_Role_Management
GO

CREATE PROCEDURE [dbo].[usp_Role_Management]
(
    @QueryType          INT,                    
    @Id                 INT = NULL,
    @Name               NVARCHAR(255) = NULL,
    @Description        NVARCHAR(MAX) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------
    -- 1. GET ALL ROLES
    ----------------------------------------------------
    IF (@QueryType = 1)
    BEGIN
        SELECT id AS Id, name AS Name, description AS Description FROM roles;
        RETURN;
    END

    ----------------------------------------------------
    -- 2. INSERT ROLE
    ----------------------------------------------------
    IF (@QueryType = 2)
    BEGIN
        INSERT INTO roles (name, description)
        VALUES (@Name, @Description);
        RETURN;
    END

    ----------------------------------------------------
    -- 3. UPDATE ROLE
    ----------------------------------------------------
    IF (@QueryType = 3)
    BEGIN
        UPDATE roles
        SET name = ISNULL(@Name, name),
            description = ISNULL(@Description, description)
        WHERE id = @Id;
        RETURN;
    END
    ----------------------------------------------------
    -- 4. DELETE ROLE
    ----------------------------------------------------
    IF (@QueryType = 4)
    BEGIN
        DELETE FROM roles WHERE id = @Id;
        RETURN;
    END

END
GO