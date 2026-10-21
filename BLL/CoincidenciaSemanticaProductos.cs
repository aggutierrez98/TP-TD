using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;

namespace BLL
{
    internal sealed class CoincidenciaSemanticaProductos
    {
        internal const double UmbralSemantico = 0.85;
        internal const int LimiteTextosPorConsulta = 90;
        private readonly IServicioEmbeddings _servicio;
        private readonly IProductoEmbeddingRepositorio _repositorio;
        private readonly IUnidadDeTrabajoFactory _unidades;
        private readonly Func<DateTime> _ahora;

        internal CoincidenciaSemanticaProductos(IServicioEmbeddings servicio, IProductoEmbeddingRepositorio repositorio, IUnidadDeTrabajoFactory unidades, Func<DateTime> ahora)
        {
            _servicio = servicio;
            _repositorio = repositorio;
            _unidades = unidades;
            _ahora = ahora;
        }

        internal async Task<Dictionary<int, List<CoincidenciaProducto>>> BuscarAsync(IEnumerable<ItemLista> items, IEnumerable<Producto> productos, CancellationToken cancellationToken)
        {
            List<ItemLista> pendientes = items.ToList();
            Dictionary<int, List<CoincidenciaProducto>> resultado = pendientes.ToDictionary(item => item.Id, item => new List<CoincidenciaProducto>());
            if (pendientes.Count == 0) return resultado;
            cancellationToken.ThrowIfCancellationRequested();
            List<Producto> catalogo = productos.OrderBy(producto => producto.Id).ToList();
            Dictionary<int, ProductoEmbedding> vigentes = _repositorio.ObtenerTodos()
                .Where(vector => vector.Modelo == _servicio.Modelo && catalogo.Any(producto => producto.Id == vector.Producto.Id && TextoFuente(producto) == vector.TextoFuente))
                .ToDictionary(vector => vector.Producto.Id);
            List<ItemLista> consultados = pendientes.Take(LimiteTextosPorConsulta).ToList();
            List<Producto> recalcular = catalogo.Where(producto => !vigentes.ContainsKey(producto.Id))
                .Take(LimiteTextosPorConsulta - consultados.Count).ToList();
            List<string> textos = consultados.Select(item => item.Descripcion).Concat(recalcular.Select(TextoFuente)).ToList();
            IReadOnlyList<float[]> vectores;
            try
            {
                vectores = await _servicio.CalcularAsync(textos, cancellationToken).ConfigureAwait(false);
                if (vectores == null || vectores.Count != textos.Count) return resultado;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception) { return resultado; }

            if (recalcular.Count > 0)
            {
                using (IUnidadDeTrabajo unidad = _unidades.Crear())
                {
                    for (int i = 0; i < recalcular.Count; i++)
                    {
                        Producto producto = recalcular[i];
                        ProductoEmbedding vector = new ProductoEmbedding
                        {
                            Producto = producto, Modelo = _servicio.Modelo, TextoFuente = TextoFuente(producto),
                            Vector = vectores[consultados.Count + i], FechaCalculo = _ahora()
                        };
                        unidad.ProductoEmbeddings.Guardar(vector);
                        vigentes[producto.Id] = vector;
                    }
                    unidad.Save();
                }
            }
            for (int i = 0; i < consultados.Count; i++)
            {
                float[] consulta = vectores[i];
                resultado[consultados[i].Id] = catalogo.Where(producto => vigentes.ContainsKey(producto.Id))
                    .Select(producto => new CoincidenciaProducto(producto, Coseno(consulta, vigentes[producto.Id].Vector), true))
                    .Where(coincidencia => coincidencia.Puntaje >= UmbralSemantico)
                    .OrderByDescending(coincidencia => coincidencia.Puntaje).ThenBy(coincidencia => coincidencia.Producto.Id)
                    .Take(CoincidenciaLocalProductos.MaximoProductosPorItem).ToList();
            }
            return resultado;
        }

        private static string TextoFuente(Producto producto)
        {
            return (producto.Nombre + " " + producto.CategoriaProducto?.Nombre).Trim();
        }

        internal static double Coseno(float[] primero, float[] segundo)
        {
            if (primero == null || segundo == null || primero.Length != segundo.Length) return 0;
            double producto = 0, normaPrimero = 0, normaSegundo = 0;
            for (int i = 0; i < primero.Length; i++)
            {
                producto += (double)primero[i] * segundo[i];
                normaPrimero += (double)primero[i] * primero[i];
                normaSegundo += (double)segundo[i] * segundo[i];
            }
            return normaPrimero > 0 && normaSegundo > 0 ? producto / Math.Sqrt(normaPrimero * normaSegundo) : 0;
        }
    }
}
