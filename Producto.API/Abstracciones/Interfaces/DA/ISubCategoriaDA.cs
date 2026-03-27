using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.DA
{
    public interface ISubCategoriaDA
    {
        Task<IEnumerable<SubCategoriaResponse>> Obtener();
        Task<IEnumerable<SubCategoriaResponse>> ObtenerPorCategoria(Guid idCategoria);
    }
}