using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DA
{
    public class SubCategoriaDA : ISubCategoriaDA
    {
        private readonly IRepositorioDapper _repositorioDapper;
        private readonly SqlConnection _sqlConnection;

        public SubCategoriaDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _sqlConnection = _repositorioDapper.ObtenerRepositorio();
        }

        public async Task<IEnumerable<SubCategoriaResponse>> Obtener()
        {
            string query = @"ObtenerSubCategorias";
            var resultadoConsulta = await _sqlConnection.QueryAsync<SubCategoriaResponse>(query);
            return resultadoConsulta;
        }

        public async Task<IEnumerable<SubCategoriaResponse>> ObtenerPorCategoria(Guid idCategoria)
        {
            string query = @"ObtenerSubCategoriasPorCategoria";

            var resultadoConsulta = await _sqlConnection.QueryAsync<SubCategoriaResponse>(
                query,
                new { IdCategoria = idCategoria });

            return resultadoConsulta;
        }
    }
}