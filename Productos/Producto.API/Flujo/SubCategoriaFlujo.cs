using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;

namespace Flujo
{
    public class SubCategoriaFlujo : ISubCategoriaFlujo
    {
        private readonly ISubCategoriaDA _subCategoriaDA;

        public SubCategoriaFlujo(ISubCategoriaDA subCategoriaDA)
        {
            _subCategoriaDA = subCategoriaDA;
        }

        public Task<IEnumerable<SubCategoriaResponse>> Obtener()
        {
            return _subCategoriaDA.Obtener();
        }

        public Task<IEnumerable<SubCategoriaResponse>> ObtenerPorCategoria(Guid idCategoria)
        {
            return _subCategoriaDA.ObtenerPorCategoria(idCategoria);
        }
    }
}