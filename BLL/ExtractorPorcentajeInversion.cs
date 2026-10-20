using System.Text.Json;
using ABSTRACCIONES;
using SERVICIOS;

namespace BLL
{
    internal enum EstadoPorcentaje { Ausente, Valido, Invalido }

    // Resultado de leer el porcentaje de la llamada de herramienta. Valor solo existe si
    // el estado es Valido: ausencia e invalidez nunca se representan con 0 ni 100.
    internal sealed class ResultadoPorcentaje
    {
        internal static readonly ResultadoPorcentaje Ausente = new ResultadoPorcentaje(EstadoPorcentaje.Ausente, null);
        internal static readonly ResultadoPorcentaje Invalido = new ResultadoPorcentaje(EstadoPorcentaje.Invalido, null);

        private ResultadoPorcentaje(EstadoPorcentaje estado, decimal? valor)
        {
            Estado = estado;
            Valor = valor;
        }

        internal EstadoPorcentaje Estado { get; }

        internal decimal? Valor { get; }

        internal static ResultadoPorcentaje DeValor(decimal valor) => new ResultadoPorcentaje(EstadoPorcentaje.Valido, valor);
    }

    // Distingue cuatro situaciones que antes se colapsaban en null: porcentaje ausente (no
    // se nombro), valido (0 excluido a 100 incluido), invalido (se indico pero no sirve) y
    // error tecnico (la salida del modelo es ilegible, se informa como ExcepcionTecnica;
    // una falla del propio servicio ya se propaga sin pasar por aca).
    internal static class ExtractorPorcentajeInversion
    {
        internal static ResultadoPorcentaje Extraer(LlamadaHerramientaAsistente llamada, string nombreHerramienta)
        {
            if (llamada == null || llamada.Nombre != nombreHerramienta || string.IsNullOrWhiteSpace(llamada.ArgumentosJson))
                return ResultadoPorcentaje.Ausente;
            try
            {
                using (JsonDocument json = JsonDocument.Parse(llamada.ArgumentosJson))
                {
                    if (json.RootElement.ValueKind != JsonValueKind.Object)
                        throw new ExcepcionTecnica("Error.Assistant.ResponseFailed");
                    JsonElement valor;
                    if (!json.RootElement.TryGetProperty("porcentaje", out valor) || valor.ValueKind == JsonValueKind.Null || valor.ValueKind == JsonValueKind.Undefined)
                        return ResultadoPorcentaje.Ausente;
                    decimal porcentaje;
                    if (valor.ValueKind != JsonValueKind.Number || !valor.TryGetDecimal(out porcentaje) || porcentaje <= 0 || porcentaje > 100)
                        return ResultadoPorcentaje.Invalido;
                    return ResultadoPorcentaje.DeValor(porcentaje);
                }
            }
            catch (JsonException ex)
            {
                throw new ExcepcionTecnica("Error.Assistant.ResponseFailed", ex);
            }
        }
    }
}
