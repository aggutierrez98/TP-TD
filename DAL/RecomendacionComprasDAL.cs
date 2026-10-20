using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    // Consultas de solo lectura con JOIN para la recomendacion de compras. Cada metodo trae en
    // una sola lectura lo que el BLL necesita, sin hidratar entidad por entidad.
    public class RecomendacionComprasDAL
    {
        public List<ItemLista> ObtenerItemsPendientesPorUsuario(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT i.Id, i.Descripcion, i.Cantidad, i.Comprado,
                       l.Id AS ListaId, l.Nombre AS ListaNombre, l.UsuarioId, l.FechaCreacion
                FROM ItemLista i
                INNER JOIN ListaDeCompras l ON l.Id = i.ListaDeComprasId
                WHERE l.UsuarioId = @UsuarioId AND i.Comprado = 0
                ORDER BY l.Nombre, l.Id, i.Id";

            List<SqlParameter> parametros = new List<SqlParameter>();
            DataTable tabla = Leer(query, accesoDatos, acceso => parametros.Add(acceso.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)), parametros);

            List<ItemLista> items = new List<ItemLista>();
            foreach (DataRow row in tabla.Rows)
            {
                items.Add(new ItemLista
                {
                    Id = (int)row["Id"],
                    Descripcion = row["Descripcion"].ToString(),
                    Cantidad = (int)row["Cantidad"],
                    Comprado = (bool)row["Comprado"],
                    ListaDeCompras = new ListaDeCompras
                    {
                        Id = (int)row["ListaId"],
                        Nombre = row["ListaNombre"].ToString(),
                        Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                        FechaCreacion = (DateTime)row["FechaCreacion"]
                    }
                });
            }

            return items;
        }

        public List<Producto> ObtenerProductosConCategoria(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT p.Id, p.Nombre, c.Id AS CategoriaId, c.Nombre AS CategoriaNombre
                FROM Producto p
                INNER JOIN CategoriaProducto c ON c.Id = p.CategoriaProductoId
                ORDER BY p.Id";

            DataTable tabla = Leer(query, accesoDatos, null, null);

            List<Producto> productos = new List<Producto>();
            foreach (DataRow row in tabla.Rows)
            {
                productos.Add(new Producto
                {
                    Id = (int)row["Id"],
                    Nombre = row["Nombre"].ToString(),
                    CategoriaProducto = new CategoriaProducto
                    {
                        Id = (int)row["CategoriaId"],
                        Nombre = row["CategoriaNombre"].ToString()
                    }
                });
            }

            return productos;
        }

        public List<ComercioProducto> ObtenerOfertas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT cp.Precio,
                       co.Id AS ComercioId, co.Nombre AS ComercioNombre, co.Direccion, co.Localidad, co.Provincia,
                       co.Latitud, co.Longitud, r.Id AS RubroId, r.Nombre AS RubroNombre,
                       p.Id AS ProductoId, p.Nombre AS ProductoNombre, c.Id AS CategoriaId, c.Nombre AS CategoriaNombre,
                       m.Id AS MonedaId, m.Codigo AS MonedaCodigo, m.Nombre AS MonedaNombre
                FROM ComercioProducto cp
                INNER JOIN Comercio co ON co.Id = cp.ComercioId
                INNER JOIN RubroComercio r ON r.Id = co.RubroComercioId
                INNER JOIN Producto p ON p.Id = cp.ProductoId
                INNER JOIN CategoriaProducto c ON c.Id = p.CategoriaProductoId
                INNER JOIN Moneda m ON m.Id = cp.MonedaId
                ORDER BY p.Id, co.Id";

            DataTable tabla = Leer(query, accesoDatos, null, null);

            List<ComercioProducto> ofertas = new List<ComercioProducto>();
            foreach (DataRow row in tabla.Rows)
            {
                ofertas.Add(new ComercioProducto
                {
                    Precio = (decimal)row["Precio"],
                    Comercio = new Comercio
                    {
                        Id = (int)row["ComercioId"],
                        Nombre = row["ComercioNombre"].ToString(),
                        Direccion = row.IsNull("Direccion") ? null : row["Direccion"].ToString(),
                        Localidad = row.IsNull("Localidad") ? null : row["Localidad"].ToString(),
                        Provincia = row.IsNull("Provincia") ? null : row["Provincia"].ToString(),
                        Latitud = row.IsNull("Latitud") ? (decimal?)null : (decimal)row["Latitud"],
                        Longitud = row.IsNull("Longitud") ? (decimal?)null : (decimal)row["Longitud"],
                        RubroComercio = new RubroComercio { Id = (int)row["RubroId"], Nombre = row["RubroNombre"].ToString() }
                    },
                    Producto = new Producto
                    {
                        Id = (int)row["ProductoId"],
                        Nombre = row["ProductoNombre"].ToString(),
                        CategoriaProducto = new CategoriaProducto { Id = (int)row["CategoriaId"], Nombre = row["CategoriaNombre"].ToString() }
                    },
                    Moneda = new Moneda
                    {
                        Id = (int)row["MonedaId"],
                        Codigo = row["MonedaCodigo"].ToString(),
                        Nombre = row["MonedaNombre"].ToString()
                    }
                });
            }

            return ofertas;
        }

        // Solo la fila base de cada promocion; los filtros los hidrata el repositorio con los
        // DAL de cada tabla intermedia.
        public List<Promocion> ObtenerPromocionesVigentesEntre(DateTime desde, DateTime hasta, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Descripcion, Descuento, TopeDescuento, VigenciaDesde, VigenciaHasta
                FROM Promocion
                WHERE VigenciaDesde <= @Hasta AND VigenciaHasta >= @Desde
                ORDER BY Id";

            List<SqlParameter> parametros = new List<SqlParameter>();
            DataTable tabla = Leer(query, accesoDatos, acceso =>
            {
                parametros.Add(acceso.CreateParameter("@Desde", SqlDbType.Date, desde.Date));
                parametros.Add(acceso.CreateParameter("@Hasta", SqlDbType.Date, hasta.Date));
            }, parametros);

            List<Promocion> promociones = new List<Promocion>();
            foreach (DataRow row in tabla.Rows)
            {
                promociones.Add(new Promocion
                {
                    Id = (int)row["Id"],
                    Descripcion = row["Descripcion"].ToString(),
                    Descuento = (decimal)row["Descuento"],
                    TopeDescuento = row.IsNull("TopeDescuento") ? (decimal?)null : (decimal)row["TopeDescuento"],
                    VigenciaDesde = (DateTime)row["VigenciaDesde"],
                    VigenciaHasta = (DateTime)row["VigenciaHasta"]
                });
            }

            return promociones;
        }

        // CredencialesCifradas no se selecciona: nunca sale de la base para este caso de uso.
        // LEFT JOIN para conservar las cuentas sin medios contratados.
        public List<CuentaFinanciera> ObtenerCuentasConMediosPorUsuario(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT cf.Id, cf.UsuarioId, cf.Alias, cf.Tipo,
                       ef.Id AS EntidadId, ef.Nombre AS EntidadNombre,
                       mpe.Id AS MedioPagoEntidadId, mpe.Segmento, mpe.EntidadFinancieraId AS MedioEntidadId,
                       mp.Id AS MedioPagoId, mp.Tipo AS MedioTipo, mp.Marca AS MedioMarca
                FROM CuentaFinanciera cf
                INNER JOIN EntidadFinanciera ef ON ef.Id = cf.EntidadFinancieraId
                LEFT JOIN CuentaFinancieraMedioPagoEntidad cm ON cm.CuentaFinancieraId = cf.Id
                LEFT JOIN MedioPagoEntidad mpe ON mpe.Id = cm.MedioPagoEntidadId
                LEFT JOIN MedioPago mp ON mp.Id = mpe.MedioPagoId
                WHERE cf.UsuarioId = @UsuarioId
                ORDER BY cf.Alias, cf.Id, mpe.Id";

            List<SqlParameter> parametros = new List<SqlParameter>();
            DataTable tabla = Leer(query, accesoDatos, acceso => parametros.Add(acceso.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)), parametros);

            List<CuentaFinanciera> cuentas = new List<CuentaFinanciera>();
            Dictionary<int, CuentaFinanciera> porId = new Dictionary<int, CuentaFinanciera>();
            foreach (DataRow row in tabla.Rows)
            {
                int cuentaId = (int)row["Id"];
                CuentaFinanciera cuenta;
                if (!porId.TryGetValue(cuentaId, out cuenta))
                {
                    cuenta = new CuentaFinanciera
                    {
                        Id = cuentaId,
                        Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                        Alias = row["Alias"].ToString(),
                        Tipo = (TipoCuentaFinanciera)Enum.Parse(typeof(TipoCuentaFinanciera), row["Tipo"].ToString()),
                        EntidadFinanciera = new EntidadFinanciera { Id = (int)row["EntidadId"], Nombre = row["EntidadNombre"].ToString() }
                    };
                    porId.Add(cuentaId, cuenta);
                    cuentas.Add(cuenta);
                }

                if (!row.IsNull("MedioPagoEntidadId"))
                {
                    cuenta.MediosPagoEntidad.Add(new MedioPagoEntidad
                    {
                        Id = (int)row["MedioPagoEntidadId"],
                        Segmento = row.IsNull("Segmento") ? null : row["Segmento"].ToString(),
                        EntidadFinanciera = new EntidadFinanciera
                        {
                            Id = (int)row["MedioEntidadId"],
                            Nombre = (int)row["MedioEntidadId"] == (int)row["EntidadId"] ? row["EntidadNombre"].ToString() : null
                        },
                        MedioPago = new MedioPago
                        {
                            Id = (int)row["MedioPagoId"],
                            Tipo = (TipoMedioPago)Enum.Parse(typeof(TipoMedioPago), row["MedioTipo"].ToString()),
                            Marca = row.IsNull("MedioMarca") ? null : row["MedioMarca"].ToString()
                        }
                    });
                }
            }

            return cuentas;
        }

        private static DataTable Leer(string query, AccesoDatosSqlServer accesoDatos, Action<AccesoDatosSqlServer> cargarParametros, List<SqlParameter> parametros)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                if (cargarParametros != null)
                {
                    cargarParametros(accesoDatos);
                }

                return accesoDatos.Read(query, parametros);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }
    }
}
