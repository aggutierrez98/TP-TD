using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class PromocionRepositorio : IPromocionRepositorio
    {
        private readonly PromocionDAL _promocionDAL;
        private readonly PromocionDiaSemanaDAL _promocionDiaSemanaDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public PromocionRepositorio()
            : this(null)
        {
        }

        internal PromocionRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _promocionDAL = new PromocionDAL();
            _promocionDiaSemanaDAL = new PromocionDiaSemanaDAL();
            _accesoDatos = accesoDatos;
        }

        public Promocion ObtenerPorId(int id)
        {
            Promocion promocion = _promocionDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(promocion);
            return promocion;
        }

        public List<Promocion> ObtenerTodas()
        {
            List<Promocion> promociones = _promocionDAL.ObtenerTodas(_accesoDatos);
            promociones.ForEach(Hidratar);
            return promociones;
        }

        private void Hidratar(Promocion promocion)
        {
            if (promocion == null)
            {
                return;
            }

            promocion.DiasSemana = _promocionDiaSemanaDAL.ObtenerDiasPorPromocion(promocion.Id, _accesoDatos);
        }

        public int InsertarYObtenerId(Promocion promocion)
        {
            return _promocionDAL.InsertarYObtenerId(promocion, _accesoDatos);
        }

        public bool Actualizar(Promocion promocion)
        {
            return _promocionDAL.Actualizar(promocion, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _promocionDAL.Eliminar(id, _accesoDatos);
        }

        public bool ReemplazarDiasDePromocion(int promocionId, List<DiaSemana> dias)
        {
            return _promocionDiaSemanaDAL.ReemplazarDiasDePromocion(promocionId, dias ?? new List<DiaSemana>(), _accesoDatos);
        }
    }
}
