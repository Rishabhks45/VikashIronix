CREATE OR ALTER PROCEDURE [dbo].[usp_Auth_GetScopes]
    @RoleId INT
AS
BEGIN
    SELECT s.name 
    FROM scopes s
    JOIN role_scopes rs ON s.id = rs.scope_id
    WHERE rs.role_id = @RoleId
END

