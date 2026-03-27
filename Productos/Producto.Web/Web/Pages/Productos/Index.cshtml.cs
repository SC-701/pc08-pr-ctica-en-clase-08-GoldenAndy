using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Web.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public IList<ProductoResponse> Productos { get; set; } = new List<ProductoResponse>();

        public IndexModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task OnGet()
        {
            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProductos");
            string endpoint = $"{urlBase}{metodo}";

            using var cliente = new HttpClient();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

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
            else
            {
                throw new Exception($"Error al obtener los productos. Código: {respuesta.StatusCode}");
            }
        }
    }
}