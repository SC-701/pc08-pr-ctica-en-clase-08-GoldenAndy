using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Abstracciones.Modelos
{
    public class ProductoBase
    {
        [Required(ErrorMessage = "La propiedad nombre es requerida")]
        [StringLength(80, ErrorMessage = "La propiedad nombre debe ser mayor a 2 caracteres y menor a 80", MinimumLength = 3)]
        [JsonPropertyOrder(3)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La propiedad descripción es requerida")]
        [StringLength(200, ErrorMessage = "La propiedad descripción debe ser mayor a 5 caracteres y menor a 200", MinimumLength = 6)]
        [JsonPropertyOrder(4)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La propiedad precio es requerida")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El formato del precio no es válido")]
        [JsonPropertyOrder(5)]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La propiedad stock es requerida")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El stock debe ser un número entero válido")]
        [JsonPropertyOrder(7)]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La propiedad codigoBarras es requerida")]
        [RegularExpression(@"^\d{8,13}$", ErrorMessage = "El código de barras debe tener solo números (8 a 13 dígitos)")]
        [JsonPropertyOrder(8)]
        public string CodigoBarras { get; set; }
    }

    public class ProductoRequest : ProductoBase
    {
        public Guid IdSubCategoria { get; set; }
    }

    public class ProductoResponse : ProductoBase
    {
        [JsonPropertyOrder(0)]
        public Guid Id { get; set; }

        [JsonPropertyOrder(1)]
        public string SubCategoria { get; set; }

        [JsonPropertyOrder(2)]
        public string Categoria { get; set; }
    }

    public class ProductoDetalle : ProductoResponse
    {
        [JsonPropertyOrder(6)]
        public decimal PrecioUSD { get; set; }
    }
}
