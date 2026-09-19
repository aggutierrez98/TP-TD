using System.Collections.Generic;
using BE;
using DAL;

namespace REPOSITORIO
{
    public class CuentaFinancieraRepositorio : ICuentaFinancieraRepositorio
    {
        private readonly CuentaFinancieraDAL _cuentaFinancieraDAL;
        private readonly AccesoDatosSqlServer _accesoDatos;

        public CuentaFinancieraRepositorio()
            : this(null)
        {
        }

        internal CuentaFinancieraRepositorio(AccesoDatosSqlServer accesoDatos)
        {
            _cuentaFinancieraDAL = new CuentaFinancieraDAL();
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

        private void Hidratar(CuentaFinanciera cuenta)
        {
            if (cuenta == null)
            {
                return;
            }

            cuenta.Usuario = new UsuarioRepositorio(_accesoDatos).ObtenerPorId(cuenta.Usuario.Id);
            cuenta.EntidadFinanciera = new EntidadFinancieraRepositorio(_accesoDatos).ObtenerPorId(cuenta.EntidadFinanciera.Id);
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
    }
}
