using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Web.Pages.Productos
{
    [Authorize]
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

            using var cliente = ObtenerClienteConToken();
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

            Producto = JsonSerializer.Deserialize<ProductoDetalle>(resultado, opciones)
                       ?? new ProductoDetalle();

            return Page();
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