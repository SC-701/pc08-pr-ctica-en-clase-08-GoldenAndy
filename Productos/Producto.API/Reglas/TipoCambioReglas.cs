using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.API;

namespace Reglas
{
    public class TipoCambioReglas : ITipoCambioReglas
    {
        private readonly ITipoCambioServicio _tipoCambioServicio;

        public TipoCambioReglas(ITipoCambioServicio tipoCambioServicio)
        {
            _tipoCambioServicio = tipoCambioServicio;
        }

        public async Task<decimal> CalcularPrecioUSD(decimal precioCRC)
        {
            var tipoCambio = await _tipoCambioServicio.ObtenerTipoCambioUSD();
            return Math.Round(precioCRC / tipoCambio, 2);
        }
    }
}
