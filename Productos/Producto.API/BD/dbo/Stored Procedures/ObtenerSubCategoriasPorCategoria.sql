CREATE   PROCEDURE ObtenerSubCategoriasPorCategoria
    @IdCategoria UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sc.Id,
        sc.Nombre,
        c.Nombre AS Categoria
    FROM dbo.SubCategorias sc
    INNER JOIN dbo.Categorias c
        ON sc.IdCategoria = c.Id
    WHERE sc.IdCategoria = @IdCategoria
    ORDER BY sc.Nombre;
END