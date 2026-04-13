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
        -- Option to soft delete children as well could go here. Let's start with single row.
        UPDATE Categories SET IsActive = 0, UpdatedAt = SYSDATETIME() WHERE Id = @Id;
        -- Also soft delete children
        UPDATE Categories SET IsActive = 0, UpdatedAt = SYSDATETIME() WHERE ParentId = @Id;
    END
END
GO

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

    -- QueryType 3: Update Material
    IF @QueryType = 3
    BEGIN
        UPDATE Materials
        SET CategoryId = @CategoryId,
            Name = @Name,
            Shape = @Shape,
            Thickness = @Thickness,
            Width = @Width,
            StandardLength = @StandardLength,
            SoldBy = @SoldBy,
            GlobalRate = @GlobalRate,
            UpdatedAt = SYSDATETIME()
        WHERE Id = @Id;
    END

    -- QueryType 4: Soft Delete Material
    IF @QueryType = 4
    BEGIN
        UPDATE Materials SET IsActive = 0, UpdatedAt = SYSDATETIME() WHERE Id = @Id;
    END

    -- QueryType 5: Add Stock Entry
    IF @QueryType = 5
    BEGIN
        -- @Thickness is reused as AddedQuantity and @GlobalRate as PurchaseRate for this query to map variables cleanly
        DECLARE @AddedQty DECIMAL(18,2) = @Thickness;
        DECLARE @PurchRate DECIMAL(18,2) = @GlobalRate;

        INSERT INTO MaterialStockLedger (MaterialId, AddedQuantity, PurchaseRate)
        VALUES (@Id, @AddedQty, @PurchRate);

        UPDATE Materials 
        SET StockMT = StockMT + @AddedQty, 
            LastPurchaseRate = @PurchRate, 
            UpdatedAt = SYSDATETIME() 
        WHERE Id = @Id;
    END
END
GO
