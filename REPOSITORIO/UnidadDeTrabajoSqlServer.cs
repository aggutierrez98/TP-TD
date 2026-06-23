using System;
using DAL;

namespace REPOSITORIO
{
    public sealed class UnidadDeTrabajoSqlServer : IUnidadDeTrabajo
    {
        private readonly AccesoDatosSqlServer _accesoDatos;

        public UnidadDeTrabajoSqlServer()
        {
            _accesoDatos = new AccesoDatosSqlServer();
        }

        internal AccesoDatosSqlServer AccesoDatosSqlServer
        {
            get { return _accesoDatos; }
        }

        public void Begin()
        {
            _accesoDatos.BeginTransactionScope();
        }

        public void Commit()
        {
            _accesoDatos.CommitTransactionScope();
        }

        public void Rollback()
        {
            _accesoDatos.RollbackTransactionScope();
        }

        public void Dispose()
        {
            _accesoDatos.Dispose();
        }
    }
}
