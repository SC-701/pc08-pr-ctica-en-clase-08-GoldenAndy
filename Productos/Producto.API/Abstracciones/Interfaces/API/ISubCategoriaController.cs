using Microsoft.AspNetCore.Mvc;

namespace Abstracciones.Interfaces.API
{
    public interface ISubCategoriaController
    {
        Task<IActionResult> Obtener();
        Task<IActionResult> ObtenerPorCategoria(Guid idCategoria);
    }
}