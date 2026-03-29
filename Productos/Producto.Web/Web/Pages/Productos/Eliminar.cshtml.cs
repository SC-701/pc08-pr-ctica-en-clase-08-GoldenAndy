using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Web.Pages.Productos
{
    [Authorize(Roles = "2")]
    public class EliminarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public EliminarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public ProductoDetalle Producto { get; set; } = new();

        public async Task<IActionResult> OnGet(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
                return RedirectToPage("./Index");

            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodoObtener = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProducto");
            string endpoint = $"{urlBase}{string.Format(metodoObtener, id.Value)}";

            using var cliente = ObtenerClienteConToken();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            Producto = JsonSerializer.Deserialize<ProductoDetalle>(resultado, opciones) ?? new ProductoDetalle();

            if (Producto == null || Producto.Id == Guid.Empty)
                return RedirectToPage("./Index");

            return Page();
        }

        public async Task<IActionResult> OnPost(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
                return RedirectToPage("./Index");

            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodoEliminar = _configuracion.ObtenerMetodo("ApiEndPoints", "EliminarProducto");
            string endpoint = $"{urlBase}{string.Format(metodoEliminar, id.Value)}";

            using var cliente = ObtenerClienteConToken();
            using var solicitud = new HttpRequestMessage(HttpMethod.Delete, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el producto.");

                string metodoObtener = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProducto");
                string endpointObtener = $"{urlBase}{string.Format(metodoObtener, id.Value)}";

                using var solicitudGet = new HttpRequestMessage(HttpMethod.Get, endpointObtener);
                var respuestaGet = await cliente.SendAsync(solicitudGet);

                if (respuestaGet.IsSuccessStatusCode)
                {
                    var resultado = await respuestaGet.Content.ReadAsStringAsync();
                    var opciones = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Producto = JsonSerializer.Deserialize<ProductoDetalle>(resultado, opciones) ?? new ProductoDetalle();
                }

                return Page();
            }

            return RedirectToPage("./Index");
        }

        private HttpClient ObtenerClienteConToken()
        {
            var tokenClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "AccessToken");

            if (tokenClaim == null || string.IsNullOrWhiteSpace(tokenClaim.Value))
            {
                throw new Exception("No se encontró el claim AccessToken en el usuario autenticado.");
            }

            var cliente = new HttpClient();
            cliente.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenClaim.Value);

            return cliente;
        }
    }
}