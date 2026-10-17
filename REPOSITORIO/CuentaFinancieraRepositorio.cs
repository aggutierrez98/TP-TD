using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class CuentaFinancieraRepositorio : ICuentaFinancieraRepositorio
    {
        private readonly CuentaFinancieraDAL _cuentaFinancieraDAL;
        private readonly CuentaFinancieraMedioPagoEntidadDAL _cuentaFinancieraMedioPagoEntidadDAL;
        private readonly EntidadFinancieraDAL _entidadFinancieraDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public CuentaFinancieraRepositorio()
            : this(null)
        {
        }

        internal CuentaFinancieraRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _cuentaFinancieraDAL = new CuentaFinancieraDAL();
            _cuentaFinancieraMedioPagoEntidadDAL = new CuentaFinancieraMedioPagoEntidadDAL();
            _entidadFinancieraDAL = new EntidadFinancieraDAL();
            _accesoDatos = accesoDatos;
        }

        public CuentaFinanciera ObtenerPorId(int id)
        {
            CuentaFinanciera cuenta = _cuentaFinancieraDAL.ObtenerPorId(id, _accesoDatos);
            Hidratar(cuenta);
            return cuenta;
        }

        public List<CuentaFinanciera> ObtenerPorUsuario(int usuarioId)
        {
            List<CuentaFinanciera> cuentas = _cuentaFinancieraDAL.ObtenerPorUsuario(usuarioId, _accesoDatos);
            cuentas.ForEach(Hidratar);
            return cuentas;
        }

        public List<CuentaFinanciera> ObtenerPorEntidadFinanciera(int entidadFinancieraId)
        {
            List<CuentaFinanciera> cuentas = _cuentaFinancieraDAL.ObtenerPorEntidadFinanciera(entidadFinancieraId, _accesoDatos);
            cuentas.ForEach(Hidratar);
            return cuentas;
        }

        // EntidadFinanciera liviana (Id + Nombre) via DAL: el catalogo completo de la entidad no
        // hace falta aca, solo los MediosPagoEntidad contratados. MedioPagoEntidadRepositorio se
        // puede usar porque su hidratacion compone solo DALs (no vuelve a pedir cuentas).
        private void Hidratar(CuentaFinanciera cuenta)
        {
            if (cuenta == null)
            {
                return;
            }

            cuenta.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(cuenta.Usuario.Id);
            cuenta.EntidadFinanciera = _entidadFinancieraDAL.ObtenerPorId(cuenta.EntidadFinanciera.Id, _accesoDatos);

            MedioPagoEntidadRepositorio medioPagoEntidadRepositorio = new MedioPagoEntidadRepositorio(_accesoDatos);
            cuenta.MediosPagoEntidad = new List<MedioPagoEntidad>();
            foreach (int medioPagoEntidadId in _cuentaFinancieraMedioPagoEntidadDAL.ObtenerMedioPagoEntidadIdsPorCuenta(cuenta.Id, _accesoDatos))
            {
                MedioPagoEntidad medioPagoEntidad = medioPagoEntidadRepositorio.ObtenerPorId(medioPagoEntidadId);
                if (medioPagoEntidad != null)
                {
                    cuenta.MediosPagoEntidad.Add(medioPagoEntidad);
                }
            }
        }

        public int InsertarYObtenerId(CuentaFinanciera cuenta)
        {
            return _cuentaFinancieraDAL.InsertarYObtenerId(cuenta, _accesoDatos);
        }

        public bool Actualizar(CuentaFinanciera cuenta)
        {
            return _cuentaFinancieraDAL.Actualizar(cuenta, _accesoDatos);
        }

        public bool Eliminar(int id)
        {
            return _cuentaFinancieraDAL.Eliminar(id, _accesoDatos);
        }

        public bool ReemplazarMediosPagoEntidadDeCuenta(int cuentaFinancieraId, List<int> medioPagoEntidadIds)
        {
            return _cuentaFinancieraMedioPagoEntidadDAL.ReemplazarMediosPagoEntidadDeCuenta(cuentaFinancieraId, medioPagoEntidadIds, _accesoDatos);
        }
    }
}
