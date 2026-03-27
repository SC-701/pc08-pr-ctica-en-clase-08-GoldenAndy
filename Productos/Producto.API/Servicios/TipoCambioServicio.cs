using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Reglas;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Servicios
{
    public class TipoCambioServicio : ITipoCambioServicio
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguracion _configuracion;

        public TipoCambioServicio(
            HttpClient httpClient,
            IConfiguracion configuracion)
        {
            _httpClient = httpClient;
            _configuracion = configuracion;
        }

        public async Task<decimal> ObtenerTipoCambioUSD()
        {
            var fecha = DateTime.Now.ToString("yyyy/MM/dd");

            var urlBase = _configuracion.ObtenerValor("BancoCentralCR:UrlBase");
            var token = _configuracion.ObtenerValor("BancoCentralCR:BearerToken");

            var url = $"{urlBase}" +
                      $"?fechaInicio={fecha}" +
                      $"&fechaFin={fecha}" +
                      $"&idioma=es";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("datos")[0]
                .GetProperty("indicadores")[0]
                .GetProperty("series")[0]
                .GetProperty("valorDatoPorPeriodo")
                .GetDecimal();
        }
    }
}
