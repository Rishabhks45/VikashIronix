IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_Bill_Management')
    DROP PROCEDURE usp_Bill_Management
GO

CREATE PROCEDURE [dbo].[usp_Bill_Management]
(
    @QueryType          INT,                    
    @Id                 UNIQUEIDENTIFIER = NULL,
    @BillNumber         NVARCHAR(50) = NULL,
    @CustomerId         UNIQUEIDENTIFIER = NULL,
    @BillDate           DATETIME2 = NULL,
    @SubTotal           DECIMAL(18,2) = NULL,
    @CGSTAmount         DECIMAL(18,2) = NULL,
    @SGSTAmount         DECIMAL(18,2) = NULL,
    @IGSTAmount         DECIMAL(18,2) = NULL,
    @BhadaAmount        DECIMAL(18,2) = NULL,
    @LabourAmount       DECIMAL(18,2) = NULL,
    @RoundingAdjustment DECIMAL(18,2) = NULL,
    @GrandTotal         DECIMAL(18,2) = NULL,
    @CashPaid           DECIMAL(18,2) = NULL,
    @AmountPaid         DECIMAL(18,2) = NULL,
    @BalanceDue         DECIMAL(18,2) = NULL,
    @TotalWeightKg      DECIMAL(18,3) = NULL,
    @Notes              NVARCHAR(MAX) = NULL,
    @Status             NVARCHAR(50) = NULL,
    @ItemsJson          NVARCHAR(MAX) = NULL -- JSON Array of Bill Items
)
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------
    -- 1. GET ALL BILLS (Dashboard List)
    ----------------------------------------------------
    IF (@QueryType = 1)
    BEGIN
        SELECT 
            b.Id,
            b.BillNumber,
            b.CustomerId,
            u.FirstName + ' ' + ISNULL(u.LastName, '') AS CustomerName,
            c.ShopName,
            b.BillDate,
            b.GrandTotal,
            b.CashPaid,
            b.AmountPaid,
            (ISNULL(b.GrandTotal, 0) - (ISNULL(b.CashPaid, 0) + ISNULL(b.AmountPaid, 0))) AS BalanceDue,
            b.TotalWeightKg,
            b.Status,
            b.CreatedAt
        FROM bills b
        INNER JOIN customers c ON b.CustomerId = c.Id
        INNER JOIN users u ON c.UserId = u.Id
        ORDER BY b.CreatedAt DESC;

        RETURN;
    END

    ----------------------------------------------------
    -- 2. GET BILL BY ID (Include Items)
    ----------------------------------------------------
    IF (@QueryType = 2 AND @Id IS NOT NULL)
    BEGIN
        -- Header
        SELECT 
            b.*,
            u.FirstName + ' ' + ISNULL(u.LastName, '') AS CustomerName,
            c.ShopName,
            c.ShopAddress
        FROM bills b
        INNER JOIN customers c ON b.CustomerId = c.Id
        INNER JOIN users u ON c.UserId = u.Id
        WHERE b.Id = @Id;

        -- Items
        SELECT 
            bi.*,
            m.Name AS MaterialName,
            m.Shape,
            m.CategoryId
        FROM bill_items bi
        INNER JOIN materials m ON bi.MaterialId = m.Id
        WHERE bi.BillId = @Id;

        RETURN;
    END

    ----------------------------------------------------
    -- 3. CREATE NEW BILL (With Items from JSON)
    ----------------------------------------------------
    IF (@QueryType = 3)
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            DECLARE @NewBillId UNIQUEIDENTIFIER = ISNULL(@Id, NEWID());
            DECLARE @GenBillNum NVARCHAR(50) = @BillNumber;

            -- Auto-Generate Bill Number if empty
            IF (@GenBillNum IS NULL OR @GenBillNum = '')
            BEGIN
                DECLARE @CurrentYear INT = YEAR(ISNULL(@BillDate, GETDATE()));
                DECLARE @NextSeq INT;
                SELECT @NextSeq = ISNULL(MAX(CAST(RIGHT(BillNumber, 4) AS INT)), 0) + 1 
                FROM bills WHERE YEAR(BillDate) = @CurrentYear;
                SET @GenBillNum = '#VI-' + CAST(@CurrentYear AS NVARCHAR(4)) + '-' + RIGHT('0000' + CAST(@NextSeq AS NVARCHAR(10)), 4);
            END

            -- Insert Header
            INSERT INTO bills (
                Id, BillNumber, CustomerId, BillDate, SubTotal, CGSTAmount, SGSTAmount, IGSTAmount, 
                BhadaAmount, LabourAmount, RoundingAdjustment, GrandTotal, CashPaid, AmountPaid, 
                BalanceDue, TotalWeightKg, Notes, Status, CreatedAt, UpdatedAt
            )
            VALUES (
                @NewBillId, @GenBillNum, @CustomerId, ISNULL(@BillDate, SYSDATETIME()), ISNULL(@SubTotal, 0),
                0, 0, 0, ISNULL(@BhadaAmount, 0), ISNULL(@LabourAmount, 0), 0, ISNULL(@GrandTotal, 0), 
                ISNULL(@CashPaid, 0), ISNULL(@AmountPaid, 0), ISNULL(@BalanceDue, 0), ISNULL(@TotalWeightKg, 0),
                @Notes, ISNULL(@Status, 'Draft'), SYSDATETIME(), SYSDATETIME()
            );

            -- Parse and Insert Items
            IF (@ItemsJson IS NOT NULL AND @ItemsJson != '')
            BEGIN
                INSERT INTO bill_items (Id, BillId, MaterialId, Quantity, UnitPrice, TaxPercentage, TotalPrice, CreatedAt, UpdatedAt)
                SELECT NEWID(), @NewBillId, MaterialId, Quantity, UnitPrice, TaxPercentage, TotalPrice, SYSDATETIME(), SYSDATETIME()
                FROM OPENJSON(@ItemsJson) WITH (
                    MaterialId UNIQUEIDENTIFIER '$.materialId',
                    Quantity DECIMAL(18,3) '$.quantity',
                    UnitPrice DECIMAL(18,2) '$.unitPrice',
                    TaxPercentage DECIMAL(18,2) '$.taxPercentage',
                    TotalPrice DECIMAL(18,2) '$.totalPrice'
                );

                -- Deduct Stock if not a draft
                IF (ISNULL(@Status, 'Draft') != 'Draft')
                BEGIN
                    UPDATE m SET m.StockMT = m.StockMT - (i.Quantity)
                    FROM Materials m
                    INNER JOIN (
                        SELECT MaterialId, SUM(Quantity) as Quantity 
                        FROM OPENJSON(@ItemsJson) WITH (MaterialId UNIQUEIDENTIFIER '$.materialId', Quantity DECIMAL(18,3) '$.quantity')
                        GROUP BY MaterialId
                    ) i ON m.Id = i.MaterialId;
                END
            END

            COMMIT TRANSACTION;
            SELECT @NewBillId AS InsertedId, @GenBillNum AS BillNumber;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION; THROW;
        END CATCH
        RETURN;
    END

    ----------------------------------------------------
    -- 4. UPDATE EXISTING BILL (Rewrite Items)
    ----------------------------------------------------
    IF (@QueryType = 4 AND @Id IS NOT NULL)
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            DECLARE @OldStatus NVARCHAR(50);
            SELECT @OldStatus = Status FROM bills WHERE Id = @Id;

            -- 1. If previously NOT a draft, revert the old stock before changing anything
            IF (@OldStatus != 'Draft')
            BEGIN
                UPDATE m SET m.StockMT = m.StockMT + (bi.Quantity)
                FROM Materials m
                INNER JOIN bill_items bi ON m.Id = bi.MaterialId
                WHERE bi.BillId = @Id;
            END

            -- 2. Update Header
            UPDATE bills SET
                CustomerId = ISNULL(@CustomerId, CustomerId),
                BillDate = ISNULL(@BillDate, BillDate),
                GrandTotal = ISNULL(@GrandTotal, GrandTotal),
                CashPaid = ISNULL(@CashPaid, CashPaid),
                AmountPaid = ISNULL(@AmountPaid, AmountPaid),
                BalanceDue = ISNULL(@BalanceDue, BalanceDue),
                TotalWeightKg = ISNULL(@TotalWeightKg, TotalWeightKg),
                Status = ISNULL(@Status, Status),
                UpdatedAt = SYSDATETIME()
            WHERE Id = @Id;

            -- 3. Rewrite Items
            IF (@ItemsJson IS NOT NULL)
            BEGIN
                DELETE FROM bill_items WHERE BillId = @Id;
                INSERT INTO bill_items (Id, BillId, MaterialId, Quantity, UnitPrice, TaxPercentage, TotalPrice, CreatedAt, UpdatedAt)
                SELECT NEWID(), @Id, MaterialId, Quantity, UnitPrice, TaxPercentage, TotalPrice, SYSDATETIME(), SYSDATETIME()
                FROM OPENJSON(@ItemsJson) WITH (
                    MaterialId UNIQUEIDENTIFIER '$.materialId',
                    Quantity DECIMAL(18,3) '$.quantity',
                    UnitPrice DECIMAL(18,2) '$.unitPrice',
                    TaxPercentage DECIMAL(18,2) '$.taxPercentage',
                    TotalPrice DECIMAL(18,2) '$.totalPrice'
                );
            END

            -- 4. If current status is NOT a draft, subtract the new stock
            IF (ISNULL(@Status, @OldStatus) != 'Draft')
            BEGIN
                UPDATE m SET m.StockMT = m.StockMT - (bi.Quantity)
                FROM Materials m
                INNER JOIN bill_items bi ON m.Id = bi.MaterialId
                WHERE bi.BillId = @Id;
            END

            COMMIT TRANSACTION;
            SELECT @Id AS UpdatedId;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION; THROW;
        END CATCH
        RETURN;
    END

    ----------------------------------------------------
    -- 5. DELETE BILL (Include Stock Reversion)
    ----------------------------------------------------
    IF (@QueryType = 5 AND @Id IS NOT NULL)
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            DECLARE @DelStatus NVARCHAR(50);
            SELECT @DelStatus = Status FROM bills WHERE Id = @Id;

            -- Revert stock if it was a finalized sale
            IF (@DelStatus != 'Draft')
            BEGIN
                UPDATE m SET m.StockMT = m.StockMT + (bi.Quantity)
                FROM Materials m
                INNER JOIN bill_items bi ON m.Id = bi.MaterialId
                WHERE bi.BillId = @Id;
            END

            DELETE FROM bills WHERE Id = @Id;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION; THROW;
        END CATCH
        RETURN;
    END
END
GO
