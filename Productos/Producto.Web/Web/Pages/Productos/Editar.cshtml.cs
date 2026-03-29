using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Web.Pages.Productos
{
    [Authorize(Roles = "2")]
    public class EditarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public EditarModel(IConfiguracion configuracion)
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

        public async Task<IActionResult> OnGet(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
                return RedirectToPage("./Index");

            await CargarCategorias();

            var productoDetalle = await ObtenerProducto(id.Value);
            if (productoDetalle == null)
                return RedirectToPage("./Index");

            Producto.Nombre = productoDetalle.Nombre;
            Producto.Descripcion = productoDetalle.Descripcion;
            Producto.Precio = productoDetalle.Precio;
            Producto.Stock = productoDetalle.Stock;
            Producto.CodigoBarras = productoDetalle.CodigoBarras;

            var categoriaItem = Categorias.FirstOrDefault(c => c.Text == productoDetalle.Categoria);
            if (categoriaItem != null && Guid.TryParse(categoriaItem.Value, out Guid idCategoria))
            {
                CategoriaSeleccionada = idCategoria;

                var subcategorias = await ObtenerSubCategorias(idCategoria);

                SubCategorias = subcategorias
                    .Select(sc => new SelectListItem
                    {
                        Value = sc.Id.ToString(),
                        Text = sc.Nombre,
                        Selected = sc.Nombre == productoDetalle.SubCategoria
                    })
                    .ToList();

                var subCategoriaItem = SubCategorias.FirstOrDefault(sc => sc.Text == productoDetalle.SubCategoria);
                if (subCategoriaItem != null && Guid.TryParse(subCategoriaItem.Value, out Guid idSubCategoria))
                {
                    Producto.IdSubCategoria = idSubCategoria;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(Guid? id)
        {
            if (!id.HasValue || id.Value == Guid.Empty)
                return RedirectToPage("./Index");

            await CargarCategorias();

            if (CategoriaSeleccionada != Guid.Empty)
            {
                await CargarSubCategorias(CategoriaSeleccionada);
            }

            if (Producto.IdSubCategoria == Guid.Empty)
            {
                ModelState.AddModelError("Producto.IdSubCategoria", "Debe seleccionar una subcategoría.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "EditarProducto");
            string endpoint = $"{urlBase}{string.Format(metodo, id.Value)}";

            using var cliente = ObtenerClienteConToken();

            var json = JsonSerializer.Serialize(Producto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var solicitud = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = content
            };

            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            var detalleError = await respuesta.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"No se pudo actualizar el producto. {detalleError}");
            return Page();
        }

        private async Task<ProductoDetalle?> ObtenerProducto(Guid id)
        {
            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProducto");
            string endpoint = $"{urlBase}{string.Format(metodo, id)}";

            using var cliente = ObtenerClienteConToken();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);
            if (!respuesta.IsSuccessStatusCode)
                return null;

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<ProductoDetalle>(resultado, opciones);
        }

        private async Task CargarCategorias()
        {
            string urlBase = _configuracion.ObtenerValor("ApiEndPoints:UrlBase");
            string metodo = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerCategorias");
            string endpoint = $"{urlBase}{metodo}";

            using var cliente = ObtenerClienteConToken();
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

            using var cliente = ObtenerClienteConToken();
            using var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);
            if (!respuesta.IsSuccessStatusCode)
                return new List<SubCategoriaResponse>();

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