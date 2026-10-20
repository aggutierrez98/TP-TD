using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    // Lectura y escritura de la metadata de automatizacion de Notificacion (TipoAutomatico,
    // FechaJornada, ClaveEjemplo), compartida por los tres DAL de la jerarquia TPH. Todas las
    // columnas son nullable: los avisos manuales y los recordatorios individuales las dejan nulas.
    internal static class MetadataNotificacionDAL
    {
        internal const string Columnas = "TipoAutomatico, FechaJornada, ClaveEjemplo";

        internal static void Leer(DataRow row, Notificacion notificacion)
        {
            notificacion.TipoAutomatico = row.IsNull("TipoAutomatico")
                ? (TipoNotificacionAutomatica?)null
                : (TipoNotificacionAutomatica)Enum.Parse(typeof(TipoNotificacionAutomatica), (string)row["TipoAutomatico"]);
            notificacion.FechaJornada = row.IsNull("FechaJornada") ? (DateTime?)null : (DateTime)row["FechaJornada"];
            notificacion.ClaveEjemplo = row.IsNull("ClaveEjemplo") ? null : (string)row["ClaveEjemplo"];
        }

        internal static void AgregarParametros(AccesoDatosSqlServer accesoDatos, List<SqlParameter> parametros, Notificacion notificacion)
        {
            parametros.Add(accesoDatos.CreateParameter("@TipoAutomatico", SqlDbType.VarChar, notificacion.TipoAutomatico.HasValue ? notificacion.TipoAutomatico.Value.ToString() : null));
            parametros.Add(accesoDatos.CreateParameter("@FechaJornada", SqlDbType.Date, notificacion.FechaJornada));
            parametros.Add(accesoDatos.CreateParameter("@ClaveEjemplo", SqlDbType.VarChar, notificacion.ClaveEjemplo));
        }
    }
}
