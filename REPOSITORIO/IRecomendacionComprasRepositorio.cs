using System;
using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    // Lecturas de solo lectura para la recomendacion de compras del asistente, resueltas con
    // consultas con JOIN para no hidratar fila por fila. Las que dependen del usuario reciben
    // su Id desde el BLL (que lo toma de la sesion), nunca desde el modelo de lenguaje.
    public interface IRecomendacionComprasRepositorio
    {
        // Items no comprados de todas las listas del usuario, con Id, nombre y dueno de su lista.
        List<ItemLista> ObtenerItemsPendientesPorUsuario(int usuarioId);

        // Catalogo de productos con Id y nombre de su categoria.
        List<Producto> ObtenerProductosConCategoria();

        // Precio de cada producto en cada comercio: comercio con rubro y coordenadas (null si no
        // tiene ubicacion fisica), producto con su categoria y moneda con su codigo.
        List<ComercioProducto> ObtenerOfertas();

        // Promociones cuya vigencia se superpone con [desde, hasta], con todos sus filtros.
        List<Promocion> ObtenerPromocionesVigentesEntre(DateTime desde, DateTime hasta);

        // Cuentas del usuario con su entidad y los medios de pago contratados. Nunca trae
        // CredencialesCifradas.
        List<CuentaFinanciera> ObtenerCuentasConMediosPorUsuario(int usuarioId);
    }
}
