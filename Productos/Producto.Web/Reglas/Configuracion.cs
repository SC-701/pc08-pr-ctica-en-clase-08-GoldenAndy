using Abstracciones.Interfaces.Reglas;
using Microsoft.Extensions.Configuration;

namespace Reglas
{
    public class Configuracion : IConfiguracion
    {
        private readonly IConfiguration _configuration;

        public Configuracion(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ObtenerValor(string llave)
        {
            return _configuration[llave] ?? string.Empty;
        }

        public string ObtenerMetodo(string seccion, string nombre)
        {
            var metodos = _configuration.GetSection($"{seccion}:Metodos").GetChildren();

            foreach (var metodo in metodos)
            {
                var nombreMetodo = metodo["Nombre"];
                var valorMetodo = metodo["Valor"];

                if (nombreMetodo == nombre)
                {
                    return valorMetodo ?? string.Empty;
                }
            }

            return string.Empty;
        }
    }
}