using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages.Productos
{
    public class DetalleModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public ProductoDetalle Producto { get; set; } = new();

        public DetalleModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnGet(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return RedirectToPage("./Index");
            }

            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProducto");
            string endpoint = $"{urlBase}{string.Format(metodo, id)}";

            using var cliente = new HttpClient();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            var resultado = await respuesta.Content.ReadAsStringAsync();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            Producto = JsonSerializer.Deserialize<ProductoDetalle>(resultado, opciones) ?? new ProductoDetalle();

            return Page();
        }
    }
}