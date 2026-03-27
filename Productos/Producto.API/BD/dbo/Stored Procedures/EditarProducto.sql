CREATE PROCEDURE [dbo].[EditarProducto]
    @Id UNIQUEIDENTIFIER,
    @IdSubCategoria UNIQUEIDENTIFIER,
    @Nombre VARCHAR(MAX),
    @Descripcion VARCHAR(MAX),
    @Precio DECIMAL(18,2),
    @Stock INT,
    @CodigoBarras VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Producto
    SET
        IdSubCategoria = @IdSubCategoria,
        Nombre = @Nombre,
        Descripcion = @Descripcion,
        Precio = @Precio,
        Stock = @Stock,
        CodigoBarras = @CodigoBarras
    WHERE Id = @Id;

    SELECT @Id;
END