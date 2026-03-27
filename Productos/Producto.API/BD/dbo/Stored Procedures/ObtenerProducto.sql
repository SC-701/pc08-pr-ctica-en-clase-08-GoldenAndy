CREATE PROCEDURE [dbo].[ObtenerProducto]
    @IdProducto UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.Stock,
        p.CodigoBarras,
        sc.Nombre AS SubCategoria,
        c.Nombre  AS Categoria
    FROM dbo.Producto p
    INNER JOIN dbo.SubCategorias sc ON sc.Id = p.IdSubCategoria
    INNER JOIN dbo.Categorias c     ON c.Id = sc.IdCategoria
    WHERE p.Id = @IdProducto;
END