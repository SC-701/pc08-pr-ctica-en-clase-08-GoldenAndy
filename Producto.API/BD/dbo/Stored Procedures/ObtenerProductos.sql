CREATE PROCEDURE [dbo].[ObtenerProductos]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
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
    ORDER BY p.Nombre;
END