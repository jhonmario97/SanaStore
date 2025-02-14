

use SanaDev


CREATE TABLE Categories(
CategoryID INT PRIMARY KEY IDENTITY,
CategoryName NVARCHAR(100) NOT NULL,
);

go

CREATE TABLE Products(h
ProductID INT PRIMARY KEY IDENTITY,
ProductCode NVARCHAR(50) not null,
Title NVARCHAR(100) NOT NULL,
[Description] NVARCHAR(500) NOT NULL,
Price DECIMAL(18,2) NOT NULL,
Stock INT NOT NULL,
[Image] VARBINARY(MAX)
);
go




CREATE TABLE ProductCategories (
    ProductID INT,
    CategoryID INT ,
    PRIMARY KEY (ProductID, CategoryID)
);
go

ALTER TABLE ProductCategories 
ADD CONSTRAINT FK_ProductCategories_ProductID  FOREIGN KEY(ProductID)
REFERENCES Products(ProductID);
go

ALTER TABLE ProductCategories 
ADD CONSTRAINT FK_ProductCategories_CategoryID FOREIGN KEY(CategoryID)
REFERENCES Categories(CategoryID);

go

CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(255) NOT NULL,
	ZipCode NVARCHAR(50) NOT NULL,
);
go
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY,
    CustomerID INT ,
    OrderDate DATETIME NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL
);
go
ALTER TABLE Orders 
ADD CONSTRAINT FK_Orders_CustomerID FOREIGN KEY(CustomerID)
REFERENCES Customers(CustomerID);
go
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY IDENTITY,
    OrderID INT ,
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL
);
go
ALTER TABLE OrderDetails 
ADD CONSTRAINT FK_OrderDetails_OrderID FOREIGN KEY(OrderID)
REFERENCES Orders(OrderID);
go
ALTER TABLE OrderDetails 
ADD CONSTRAINT FK_OrderDetails_ProductID FOREIGN KEY(ProductID)
REFERENCES Products(ProductID);
go

