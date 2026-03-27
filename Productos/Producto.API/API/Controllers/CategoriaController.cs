using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;
//Comentario de Prueba para Workflow 2026
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase, ICategoriaController
    {
        private readonly ICategoriaFlujo _categoriaFlujo;

        public CategoriaController(ICategoriaFlujo categoriaFlujo)
        {
            _categoriaFlujo = categoriaFlujo;
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var resultado = await _categoriaFlujo.Obtener();

            if (!resultado.Any())
                return NoContent();

            return Ok(resultado);
        }
    }
}