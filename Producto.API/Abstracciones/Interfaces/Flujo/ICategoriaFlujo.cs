using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Flujo
{
    public interface ICategoriaFlujo
    {
        Task<IEnumerable<CategoriaResponse>> Obtener();
    }
}