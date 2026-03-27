CREATE   PROCEDURE ObtenerSubCategorias
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
    ORDER BY c.Nombre, sc.Nombre;
END