
CREATE DATABASE TrackerDB


USE [TrackerDB]
GO

CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(50) NOT NULL CHECK (Type IN ('Income', 'Expense'))
);

CREATE TABLE Transaction (
    TransactionId INT PRIMARY KEY IDENTITY(1,1),
    Amount DECIMAL(18, 2) NOT NULL,
    Type NVARCHAR(50) NOT NULL CHECK (Type IN ('Income', 'Expense')),
    CategoryId INT NOT NULL,
    Description NVARCHAR(500),
    TransactionDate DATE NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
);

GO