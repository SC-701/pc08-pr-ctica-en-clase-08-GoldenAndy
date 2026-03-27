CREATE PROCEDURE [dbo].[EliminarProducto]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Producto
    WHERE Id = @Id;

    SELECT @Id;
END