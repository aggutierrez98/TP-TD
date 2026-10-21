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

        public List<CuentaFinanciera> ObtenerPorUsuario(Usuario usuario)
        {
            List<CuentaFinanciera> cuentas = _cuentaFinancieraDAL.ObtenerPorUsuario(usuario, _accesoDatos);
            cuentas.ForEach(Hidratar);
            return cuentas;
        }

        public List<CuentaFinanciera> ObtenerPorEntidadFinanciera(EntidadFinanciera entidad)
        {
            List<CuentaFinanciera> cuentas = _cuentaFinancieraDAL.ObtenerPorEntidadFinanciera(entidad, _accesoDatos);
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
            foreach (MedioPagoEntidad referencia in _cuentaFinancieraMedioPagoEntidadDAL.ObtenerMediosPagoEntidadPorCuenta(cuenta, _accesoDatos))
            {
                MedioPagoEntidad medioPagoEntidad = medioPagoEntidadRepositorio.ObtenerPorId(referencia.Id);
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

        public bool Eliminar(CuentaFinanciera cuenta)
        {
            return _cuentaFinancieraDAL.Eliminar(cuenta, _accesoDatos);
        }

        public bool ReemplazarMediosPagoEntidadDeCuenta(CuentaFinanciera cuenta, IEnumerable<MedioPagoEntidad> medios)
        {
            return _cuentaFinancieraMedioPagoEntidadDAL.ReemplazarMediosPagoEntidadDeCuenta(cuenta, medios, _accesoDatos);
        }

        public int ContarPorMedioPagoEntidad(MedioPagoEntidad medio)
        {
            return _cuentaFinancieraMedioPagoEntidadDAL.ContarPorMedioPagoEntidad(medio, _accesoDatos);
        }
    }
}
