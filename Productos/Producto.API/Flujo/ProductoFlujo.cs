using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;

namespace Flujo
{
    public class ProductoFlujo : IProductoFlujo
    {
        private readonly IProductoDA _productoDA;
        private readonly ITipoCambioReglas _tipoCambioReglas;

        public ProductoFlujo(
            IProductoDA productoDA,
            ITipoCambioReglas tipoCambioReglas)
        {
            _productoDA = productoDA;
            _tipoCambioReglas = tipoCambioReglas;
        }

        public Task<Guid> Agregar(ProductoRequest producto)
        {
            return _productoDA.Agregar(producto);
        }

        public Task<Guid> Editar(Guid Id, ProductoRequest producto)
        {
            return _productoDA.Editar(Id, producto);
        }

        public Task<Guid> Eliminar(Guid Id)
        {
            return _productoDA.Eliminar(Id);
        }

        public Task<IEnumerable<ProductoResponse>> Obtener()
        {
            return _productoDA.Obtener();
        }

        public async Task<ProductoDetalle> Obtener(Guid Id)
        {
            var producto = await _productoDA.Obtener(Id);

            var detalle = new ProductoDetalle
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CodigoBarras = producto.CodigoBarras,
                Categoria = producto.Categoria,
                SubCategoria = producto.SubCategoria,
                PrecioUSD = await _tipoCambioReglas
                    .CalcularPrecioUSD(producto.Precio)
            };

            return detalle;
        }
    }
}
