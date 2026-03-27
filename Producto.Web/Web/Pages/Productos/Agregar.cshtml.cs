using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace Web.Pages.Productos
{
    public class AgregarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public AgregarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        [BindProperty]
        public ProductoRequest Producto { get; set; } = new();

        [BindProperty]
        public Guid CategoriaSeleccionada { get; set; }

        [BindProperty]
        public List<SelectListItem> Categorias { get; set; } = new();

        [BindProperty]
        public List<SelectListItem> SubCategorias { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            await CargarCategorias();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await CargarCategorias();

            if (Producto.IdSubCategoria == Guid.Empty)
            {
                ModelState.AddModelError("Producto.IdSubCategoria", "Debe seleccionar una subcategoría.");
            }

            if (!ModelState.IsValid)
            {
                if (CategoriaSeleccionada != Guid.Empty)
                {
                    await CargarSubCategorias(CategoriaSeleccionada);
                }

                return Page();
            }

            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "AgregarProducto");
            string endpoint = $"{urlBase}{metodo}";

            using var cliente = new HttpClient();
            var respuesta = await cliente.PostAsJsonAsync(endpoint, Producto);

            if (respuesta.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            var detalleError = await respuesta.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"No se pudo guardar el producto. {detalleError}");

            if (CategoriaSeleccionada != Guid.Empty)
            {
                await CargarSubCategorias(CategoriaSeleccionada);
            }

            return Page();
        }

        private async Task CargarCategorias()
        {
            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerCategorias");
            string endpoint = $"{urlBase}{metodo}";

            using var cliente = new HttpClient();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
            {
                Categorias = new List<SelectListItem>();
                return;
            }

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var categorias = JsonSerializer.Deserialize<List<CategoriaResponse>>(resultado, opciones)
                              ?? new List<CategoriaResponse>();

            Categorias = categorias
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                })
                .ToList();
        }

        private async Task<List<SubCategoriaResponse>> ObtenerSubCategorias(Guid idCategoria)
        {
            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerSubCategoriasPorCategoria");
            string endpoint = $"{urlBase}{string.Format(metodo, idCategoria)}";

            using var cliente = new HttpClient();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
            {
                return new List<SubCategoriaResponse>();
            }

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<SubCategoriaResponse>>(resultado, opciones)
                   ?? new List<SubCategoriaResponse>();
        }

        private async Task CargarSubCategorias(Guid idCategoria)
        {
            var subcategorias = await ObtenerSubCategorias(idCategoria);

            SubCategorias = subcategorias
                .Select(sc => new SelectListItem
                {
                    Value = sc.Id.ToString(),
                    Text = sc.Nombre
                })
                .ToList();
        }

        public async Task<JsonResult> OnGetObtenerSubCategorias(Guid idCategoria)
        {
            var subcategorias = await ObtenerSubCategorias(idCategoria);
            return new JsonResult(subcategorias);
        }
    }
}