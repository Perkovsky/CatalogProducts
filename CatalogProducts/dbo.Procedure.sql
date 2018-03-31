CREATE PROCEDURE [dbo].[InsertUpdateCatalogProducts]
	@productID INT,
	@name NVARCHAR (MAX),
    @category NVARCHAR (MAX),
    @description NVARCHAR (MAX),
    @inStock INT,
    @price DECIMAL (18, 2)
AS
	UPDATE dbo.Products SET Name=@name, Category=@category, Description=@description, InStock=@inStock, Price=@price
		WHERE ProductID=@productID
	IF @@rowcount = 0
		INSERT INTO dbo.Products (ProductID, Name, Category, Description, InStock, Price) 
			VALUES (@productID, @name, @category, @description, @inStock, @price)