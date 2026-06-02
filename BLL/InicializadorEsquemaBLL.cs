using System;
using System.IO;
using DAL;
using SERVICIOS;

namespace BLL
{
    public static class InicializadorEsquemaBLL
    {
        private static bool _ejecutado;
        private static readonly object Sync = new object();

        public static void AsegurarEsquema()
        {
            if (_ejecutado)
            {
                return;
            }

            lock (Sync)
            {
                if (_ejecutado)
                {
                    return;
                }

                string rutaBase = AppDomain.CurrentDomain.BaseDirectory;
                string rutaScript = Path.GetFullPath(Path.Combine(rutaBase, @"..\..\script_bd.sql"));
                if (!File.Exists(rutaScript))
                {
                    rutaScript = Path.GetFullPath(Path.Combine(rutaBase, @"..\..\..\script_bd.sql"));
                }

                try
                {
                    new InicializadorEsquemaDAL().EjecutarScript(rutaScript);
                    _ejecutado = true;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("No se pudo inicializar el esquema de base de datos.", ex);
                }
            }
        }
    }
}
