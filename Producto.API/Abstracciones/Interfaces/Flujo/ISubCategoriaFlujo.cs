using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Flujo
{
    public interface ISubCategoriaFlujo
    {
        Task<IEnumerable<SubCategoriaResponse>> Obtener();
        Task<IEnumerable<SubCategoriaResponse>> ObtenerPorCategoria(Guid idCategoria);
    }
}