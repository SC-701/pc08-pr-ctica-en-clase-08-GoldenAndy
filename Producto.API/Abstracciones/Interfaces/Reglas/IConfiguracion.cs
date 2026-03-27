namespace Abstracciones.Interfaces.Reglas
{
    public interface IConfiguracion
    {
        string ObtenerMetodo(string seccion, string nombre);
        string ObtenerValor(string llave);
    }
}
