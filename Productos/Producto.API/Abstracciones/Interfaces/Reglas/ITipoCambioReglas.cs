namespace Abstracciones.Interfaces.Reglas
{
    public interface ITipoCambioReglas
    {
        Task<decimal> CalcularPrecioUSD(decimal precioCRC);
    }
}
