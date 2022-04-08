CREATE PROCEDURE [dbo].[spProductGetById]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id, ProductName, [Description], RetailPrice, QuentityInStock, IsTaxable
	FROM dbo.Product
	WHERE Id = @Id
END