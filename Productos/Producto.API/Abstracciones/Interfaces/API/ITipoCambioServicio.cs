namespace Abstracciones.Interfaces.API
{
    public interface ITipoCambioServicio
    {
        Task<decimal> ObtenerTipoCambioUSD();
    }
}
