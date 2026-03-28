using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Web.Pages.Productos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public IList<ProductoResponse> Productos { get; set; } = new List<ProductoResponse>();

        public IndexModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnGet()
        {
            var cliente = await ObtenerClienteConToken();

            if (cliente == null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToPage("/Cuenta/Login");
            }

            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProductos");
            string endpoint = $"{urlBase}{metodo}";

            using (cliente)
            using (var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint))
            {
                var respuesta = await cliente.SendAsync(solicitud);

                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadAsStringAsync();
                    var opciones = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Productos = JsonSerializer.Deserialize<List<ProductoResponse>>(resultado, opciones)
                                ?? new List<ProductoResponse>();
                }
                else if (respuesta.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Productos = new List<ProductoResponse>();
                }
                else if (respuesta.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await HttpContext.SignOutAsync();
                    return RedirectToPage("/Cuenta/Login");
                }
                else
                {
                    throw new Exception($"Error al obtener los productos. Código: {respuesta.StatusCode}");
                }
            }

            return Page();
        }

        private Task<HttpClient?> ObtenerClienteConToken()
        {
            var tokenClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "AccessToken");

            if (tokenClaim == null || string.IsNullOrWhiteSpace(tokenClaim.Value))
            {
                return Task.FromResult<HttpClient?>(null);
            }

            var cliente = new HttpClient();
            cliente.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenClaim.Value);

            return Task.FromResult<HttpClient?>(cliente);
        }
    }
}