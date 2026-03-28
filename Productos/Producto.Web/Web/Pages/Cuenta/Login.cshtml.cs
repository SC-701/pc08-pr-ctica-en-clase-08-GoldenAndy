using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Abstracciones.Modelos.Seguridad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Web.Pages.Cuenta
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginRequest loginInfo { get; set; } = new();

        public Token token { get; set; } = new();

        private readonly IConfiguracion _configuracion;

        public LoginModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnPost()
        {
            var erroresModelState = ModelState
                .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                .Select(x => $"{x.Key}: {string.Join(" | ", x.Value!.Errors.Select(e => e.ErrorMessage))}")
                .ToList();

            if (!ModelState.IsValid)
            {
                foreach (var error in erroresModelState)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            var hash = Autenticacion.GenerarHash(loginInfo.Password);
            loginInfo.PasswordHash = Autenticacion.ObtenerHash(hash);

            string urlBase = _configuracion.ObtenerValor("ApiEndPointsSeguridad:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPointsSeguridad", "Login");
            string endpoint = $"{urlBase}{metodo}";

            var payload = new LoginBase
            {
                NombreUsuario = loginInfo.NombreUsuario,
                PasswordHash = loginInfo.PasswordHash
            };

            try
            {
                using var client = new HttpClient();

                var respuesta = await client.PostAsJsonAsync(endpoint, payload);
                var contenido = await respuesta.Content.ReadAsStringAsync();

                ModelState.AddModelError(string.Empty, $"Endpoint usado: {endpoint}");
                ModelState.AddModelError(string.Empty, $"Usuario enviado: {payload.NombreUsuario}");
                ModelState.AddModelError(string.Empty, $"StatusCode: {(int)respuesta.StatusCode}");
                ModelState.AddModelError(string.Empty, $"Respuesta API: {contenido}");

                if (!respuesta.IsSuccessStatusCode)
                {
                    return Page();
                }

                var opciones = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                token = JsonSerializer.Deserialize<Token>(contenido, opciones) ?? new Token();

                if (token.ValidacionExitosa && !string.IsNullOrWhiteSpace(token.AccessToken))
                {
                    JwtSecurityToken? jwtToken = Autenticacion.leerToken(token.AccessToken);
                    var claims = Autenticacion.GenerarClaims(jwtToken, token.AccessToken);

                    if (!claims.Any(c => c.Type == "AccessToken"))
                    {
                        claims.Add(new Claim("AccessToken", token.AccessToken));
                    }

                    await EstablecerAutenticacion(claims);

                    var urlRedirigir = $"{HttpContext.Request.Query["ReturnUrl"]}";
                    if (string.IsNullOrWhiteSpace(urlRedirigir))
                        return Redirect("/");

                    return Redirect(urlRedirigir);
                }

                ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Excepción al conectar con la API: {ex.Message}");

                if (ex.InnerException != null)
                {
                    ModelState.AddModelError(string.Empty, $"Detalle interno: {ex.InnerException.Message}");
                }

                return Page();
            }
        }

        private async Task EstablecerAutenticacion(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}