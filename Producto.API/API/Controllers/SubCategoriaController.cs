using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoriaController : ControllerBase, ISubCategoriaController
    {
        private readonly ISubCategoriaFlujo _subCategoriaFlujo;

        public SubCategoriaController(ISubCategoriaFlujo subCategoriaFlujo)
        {
            _subCategoriaFlujo = subCategoriaFlujo;
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var resultado = await _subCategoriaFlujo.Obtener();

            if (!resultado.Any())
                return NoContent();

            return Ok(resultado);
        }

        [HttpGet("PorCategoria/{idCategoria}")]
        public async Task<IActionResult> ObtenerPorCategoria(Guid idCategoria)
        {
            var resultado = await _subCategoriaFlujo.ObtenerPorCategoria(idCategoria);

            if (!resultado.Any())
                return NoContent();

            return Ok(resultado);
        }
    }
}