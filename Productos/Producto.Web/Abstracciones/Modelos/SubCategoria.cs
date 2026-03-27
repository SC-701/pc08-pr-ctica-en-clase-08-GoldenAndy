using System.Text.Json.Serialization;

namespace Abstracciones.Modelos
{
    public class SubCategoriaResponse
    {
        [JsonPropertyOrder(0)]
        public Guid Id { get; set; }

        [JsonPropertyOrder(1)]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyOrder(2)]
        public string Categoria { get; set; } = string.Empty;
    }
}