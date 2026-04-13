-- Create/Update Salary Management SP (Simplified)
CREATE OR ALTER PROCEDURE [dbo].[usp_Salary_Management]
    @QueryType INT,
    @Id UNIQUEIDENTIFIER = NULL,
    @UserId UNIQUEIDENTIFIER = NULL,
    @MonthlySalary DECIMAL(18, 2) = NULL,
    @EffectiveDate DATE = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- QueryType 1: Get All Salary Configurations
    IF @QueryType = 1
    BEGIN
        SELECT sc.*, u.FirstName + ' ' + u.LastName as StaffName
        FROM SalaryConfigurations sc
        INNER JOIN users u ON sc.UserId = u.id
        WHERE sc.IsActive = 1
        ORDER BY u.FirstName, sc.EffectiveDate DESC;
    END

    -- QueryType 2: Insert/Upsert Salary Configuration
    IF @QueryType = 2
    BEGIN
        IF @Id IS NULL OR NOT EXISTS (SELECT 1 FROM SalaryConfigurations WHERE Id = @Id)
        BEGIN
            INSERT INTO SalaryConfigurations (Id, UserId, MonthlySalary, EffectiveDate, IsActive, CreatedAt, UpdatedAt)
            VALUES (ISNULL(@Id, NEWID()), @UserId, @MonthlySalary, @EffectiveDate, ISNULL(@IsActive, 1), SYSDATETIME(), SYSDATETIME());
        END
        ELSE
        BEGIN
            UPDATE SalaryConfigurations
            SET MonthlySalary = @MonthlySalary,
                EffectiveDate = @EffectiveDate,
                IsActive = ISNULL(@IsActive, 1),
                UpdatedAt = SYSDATETIME()
            WHERE Id = @Id;
        END
    END

    -- QueryType 3: Get Salary Configuration by UserId
    IF @QueryType = 3
    BEGIN
        SELECT TOP 1 sc.*, u.FirstName + ' ' + u.LastName as StaffName
        FROM SalaryConfigurations sc
        INNER JOIN users u ON sc.UserId = u.id
        WHERE sc.UserId = @UserId AND sc.IsActive = 1
        ORDER BY sc.EffectiveDate DESC;
    END

    -- QueryType 4: Delete (Soft)
    IF @QueryType = 4
    BEGIN
        UPDATE SalaryConfigurations SET IsActive = 0, UpdatedAt = SYSDATETIME() WHERE Id = @Id;
    END
END
GO
