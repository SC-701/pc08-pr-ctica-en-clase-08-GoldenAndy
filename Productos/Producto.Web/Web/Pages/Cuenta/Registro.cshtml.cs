using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos.Seguridad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;

namespace Web.Pages.Cuenta
{
    public class RegistroModel : PageModel
    {
        [BindProperty]
        public Usuario usuario { get; set; } = new();

        private readonly IConfiguracion _configuracion;

        public RegistroModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var hash = Autenticacion.GenerarHash(usuario.Password);
            usuario.PasswordHash = Autenticacion.ObtenerHash(hash);

            string urlBase = _configuracion.ObtenerValor("ApiEndPointsSeguridad:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPointsSeguridad", "Registro");
            string endpoint = $"{urlBase}{metodo}";

            using var cliente = new HttpClient();
            var respuesta = await cliente.PostAsJsonAsync(endpoint, usuario);

            if (respuesta.IsSuccessStatusCode)
            {
                return RedirectToPage("/Cuenta/Login");
            }

            var detalle = await respuesta.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Error al registrar: {respuesta.StatusCode} - {detalle}");
            return Page();
        }
    }
}