
USE [TrackerDB]
GO
-- Get all transactions with category name
CREATE PROCEDURE GetAllTransactions_prc
AS
BEGIN

    SELECT t.TransactionId, t.Amount, t.Type, t.CategoryId, t.Description, t.TransactionDate,
           c.Name AS CategoryName
    FROM Transactions t
    INNER JOIN Categories c ON t.CategoryId = c.CategoryId
    ORDER BY t.TransactionDate DESC;

END

GO

USE [TrackerDB]
GO
-- Insert a new transaction
CREATE PROCEDURE AddTransaction_prc
    @Amount DECIMAL(10,2),
    @Type NVARCHAR(10),
    @CategoryId INT,
    @Description NVARCHAR(200),
    @TransactionDate DATE
AS
BEGIN
    INSERT INTO Transactions (Amount, Type, CategoryId, Description, TransactionDate)
    VALUES (@Amount, @Type, @CategoryId, @Description, @TransactionDate);
END

GO

USE [TrackerDB]
GO
-- Get all categories with category name
CREATE PROCEDURE GetAllCategories_prc
AS
BEGIN
    SELECT CategoryId, Name, Type FROM Categories;
END

GO


USE [TrackerDB]
GO
-- Get summary totals
CREATE PROCEDURE GetTransactionSummary_prc
AS
BEGIN
    SELECT Type, SUM(Amount) AS Total
    FROM Transactions
    GROUP BY Type
END

GO


USE [TrackerDB]
GO
-- Get monthly summary for chart
CREATE PROC GetMonthlySummary_prc
AS
BEGIN
SELECT FORMAT(TransactionDate, 'yyyy-MM') AS Month,Type,SUM(Amount) AS Total
FROM Transactions
WHERE YEAR(TransactionDate) = YEAR(GETDATE())
GROUP BY FORMAT(TransactionDate, 'yyyy-MM'), Type
ORDER BY Month;
END

GO