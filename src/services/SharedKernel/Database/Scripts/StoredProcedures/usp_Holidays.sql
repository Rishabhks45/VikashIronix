-- Create/Update Holiday SP
CREATE OR ALTER PROCEDURE [dbo].[usp_Holidays]
    @QueryType INT,
    @Id UNIQUEIDENTIFIER = NULL,
    @Name NVARCHAR(255) = NULL,
    @Date DATE = NULL,
    @Type INT = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- QueryType 1: Insert Holiday
    IF @QueryType = 1
    BEGIN
        INSERT INTO Holidays (Id, Name, Date, Type, Description, IsActive, CreatedAt, UpdatedAt)
        VALUES (ISNULL(@Id, NEWID()), @Name, @Date, @Type, @Description, ISNULL(@IsActive, 1), SYSDATETIME(), SYSDATETIME());
    END

    -- QueryType 2: Get All Active Holidays
    IF @QueryType = 2
    BEGIN
        SELECT * FROM Holidays 
        WHERE IsActive = 1 
        ORDER BY Date ASC;
    END

    -- QueryType 3: Update Holiday
    IF @QueryType = 3
    BEGIN
        UPDATE Holidays 
        SET Name = @Name,
            Date = @Date,
            Type = @Type,
            Description = @Description,
            IsActive = ISNULL(@IsActive, 1),
            UpdatedAt = SYSDATETIME()
        WHERE Id = @Id;
    END

    -- QueryType 4: Soft Delete Holiday
    IF @QueryType = 4
    BEGIN
        UPDATE Holidays 
        SET IsActive = 0, 
            UpdatedAt = SYSDATETIME() 
        WHERE Id = @Id;
    END

    -- QueryType 5: Get Single Holiday By Id
    IF @QueryType = 5
    BEGIN
        SELECT * FROM Holidays WHERE Id = @Id;
    END
END
GO
