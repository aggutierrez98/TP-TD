using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class ReglaCategorizacionBLL
    {
        private readonly IReglaCategorizacionRepositorio _reglaRepositorio;
        private readonly IComercioReglaCategorizacionRepositorio _comercioReglaRepositorio;
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IComercioRepositorio _comercioRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal ReglaCategorizacionBLL(
            IReglaCategorizacionRepositorio reglaRepositorio,
            IComercioReglaCategorizacionRepositorio comercioReglaRepositorio,
            ICategoriaRepositorio categoriaRepositorio,
            IComercioRepositorio comercioRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL)
        {
            _reglaRepositorio = reglaRepositorio;
            _comercioReglaRepositorio = comercioReglaRepositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _comercioRepositorio = comercioRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<ReglaCategorizacion> ObtenerPorUsuario(int usuarioId)
        {
            return _reglaRepositorio.ObtenerPorUsuario(usuarioId);
        }

        public List<int> ObtenerComercioIdsPorRegla(int reglaId)
        {
            return _comercioReglaRepositorio.ObtenerComercioIdsPorReglaCategorizacion(reglaId);
        }

        public int Crear(string nombre, int categoriaId, string palabrasClave, List<int> comercioIds = null)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            string nombreNormalizado = ValidarNombre(nombre);
            string palabrasClaveNormalizadas = ValidarPalabrasClave(palabrasClave);
            ValidarCategoriaVisible(categoriaId, usuarioActualId);
            ValidarComercios(comercioIds);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ReglaCategorizacion nuevaRegla = new ReglaCategorizacion
                    {
                        Usuario = new Usuario { Id = usuarioActualId },
                        Categoria = new Categoria { Id = categoriaId },
                        Nombre = nombreNormalizado,
                        PalabrasClave = palabrasClaveNormalizadas
                    };

                    int id = unidad.ReglasCategorizacion.InsertarYObtenerId(nuevaRegla);

                    if (comercioIds != null && comercioIds.Count > 0)
                    {
                        unidad.ComercioReglaCategorizacion.ReemplazarComerciosDeReglaCategorizacion(id, comercioIds.Distinct().ToList());
                    }

                    _bitacoraBLL.RegistrarOperacionInterno(
                        "FormCategorizationRules.Value.Activity.RuleCreation",
                        "FormCategorizationRules.Value.Info.RuleCreation",
                        "ReglaCategorizacionBLL",
                        unidad);

                    unidad.Save();
                    return id;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Rule.CreateFailed", ex);
                }
            }
        }

        public bool Actualizar(int id, string nombre, int categoriaId, string palabrasClave, List<int> comercioIds = null)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            string nombreNormalizado = ValidarNombre(nombre);
            string palabrasClaveNormalizadas = ValidarPalabrasClave(palabrasClave);
            ValidarCategoriaVisible(categoriaId, usuarioActualId);
            ValidarComercios(comercioIds);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ReglaCategorizacion regla = unidad.ReglasCategorizacion.ObtenerPorId(id);
                    ValidarPropiedad(regla, usuarioActualId);

                    regla.Nombre = nombreNormalizado;
                    regla.Categoria = new Categoria { Id = categoriaId };
                    regla.PalabrasClave = palabrasClaveNormalizadas;

                    bool actualizado = unidad.ReglasCategorizacion.Actualizar(regla);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.Rule.UpdateFailed");
                    }

                    List<int> comerciosAAsociar = comercioIds == null ? new List<int>() : comercioIds.Distinct().ToList();
                    unidad.ComercioReglaCategorizacion.ReemplazarComerciosDeReglaCategorizacion(id, comerciosAAsociar);

                    _bitacoraBLL.RegistrarOperacionInterno(
                        "FormCategorizationRules.Value.Activity.RuleUpdate",
                        "FormCategorizationRules.Value.Info.RuleUpdate",
                        "ReglaCategorizacionBLL",
                        unidad);

                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Rule.UpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ReglaCategorizacion regla = unidad.ReglasCategorizacion.ObtenerPorId(id);
                    ValidarPropiedad(regla, usuarioActualId);

                    unidad.ComercioReglaCategorizacion.ReemplazarComerciosDeReglaCategorizacion(id, new List<int>());
                    bool eliminado = unidad.ReglasCategorizacion.Eliminar(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.Rule.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno(
                        "FormCategorizationRules.Value.Activity.RuleDeletion",
                        "FormCategorizationRules.Value.Info.RuleDeletion",
                        "ReglaCategorizacionBLL",
                        unidad);

                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Rule.DeleteFailed", ex);
                }
            }
        }

        // Sugiere la categoria para una descripcion y comercio opcional segun las reglas del usuario actual.
        // Devuelve null si no hay coincidencia o si hay empate residual, exigiendo seleccion manual.
        public Categoria SugerirCategoria(string descripcion, int? comercioId = null)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                return null;
            }

            int? usuarioActualId = _sesion.GetUsuarioActualId();
            if (!usuarioActualId.HasValue)
            {
                return null;
            }

            List<ReglaCategorizacion> reglas = _reglaRepositorio.ObtenerPorUsuario(usuarioActualId.Value);
            if (reglas == null || reglas.Count == 0)
            {
                return null;
            }

            string descripcionNormalizada = NormalizarTexto(descripcion);

            List<CandidatoRegla> candidatos = new List<CandidatoRegla>();

            foreach (ReglaCategorizacion regla in reglas)
            {
                List<int> comercioIds = _comercioReglaRepositorio.ObtenerComercioIdsPorReglaCategorizacion(regla.Id);
                bool tieneComerciosAsociados = comercioIds != null && comercioIds.Count > 0;

                // Elegibilidad:
                // Regla general (sin comercios) -> siempre elegible, TieneMatchComercio = false
                // Regla con comercios -> solo elegible si el movimiento tiene uno de esos comercios, TieneMatchComercio = true
                bool esElegible;
                bool tieneMatchComercio;

                if (tieneComerciosAsociados)
                {
                    if (comercioId.HasValue && comercioIds.Contains(comercioId.Value))
                    {
                        esElegible = true;
                        tieneMatchComercio = true;
                    }
                    else
                    {
                        esElegible = false;
                        tieneMatchComercio = false;
                    }
                }
                else
                {
                    esElegible = true;
                    tieneMatchComercio = false;
                }

                if (!esElegible)
                {
                    continue;
                }

                // Matching de palabras clave
                List<string> palabrasClave = (regla.PalabrasClave ?? string.Empty)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(NormalizarTexto)
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Distinct()
                    .ToList();

                int matchCount = 0;
                int coberturaTexto = 0;

                foreach (string palabra in palabrasClave)
                {
                    string pattern = @"\b" + Regex.Escape(palabra) + @"\b";
                    if (Regex.IsMatch(descripcionNormalizada, pattern, RegexOptions.IgnoreCase))
                    {
                        matchCount++;
                        coberturaTexto += palabra.Length;
                    }
                }

                if (matchCount > 0)
                {
                    candidatos.Add(new CandidatoRegla
                    {
                        Regla = regla,
                        MatchCount = matchCount,
                        CoberturaTexto = coberturaTexto,
                        TieneMatchComercio = tieneMatchComercio
                    });
                }
            }

            if (candidatos.Count == 0)
            {
                return null;
            }

            // Ordenamiento por especificidad:
            // 1. Mayor cantidad de coincidencias
            // 2. Mayor cobertura de texto
            // 3. Match de comercio ante empate textual
            candidatos.Sort((a, b) =>
            {
                int cmpMatch = b.MatchCount.CompareTo(a.MatchCount);
                if (cmpMatch != 0) return cmpMatch;

                int cmpCobertura = b.CoberturaTexto.CompareTo(a.CoberturaTexto);
                if (cmpCobertura != 0) return cmpCobertura;

                return b.TieneMatchComercio.CompareTo(a.TieneMatchComercio);
            });

            if (candidatos.Count == 1)
            {
                return candidatos[0].Regla.Categoria;
            }

            // Comprobar empate entre el 1° y 2°
            CandidatoRegla primero = candidatos[0];
            CandidatoRegla segundo = candidatos[1];

            if (primero.MatchCount == segundo.MatchCount &&
                primero.CoberturaTexto == segundo.CoberturaTexto &&
                primero.TieneMatchComercio == segundo.TieneMatchComercio)
            {
                // Empate residual: no se asigna categoria automaticamente
                return null;
            }

            return primero.Regla.Categoria;
        }

        public static string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            string formD = texto.Trim().Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        private static string ValidarNombre(string nombre)
        {
            string normalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizado))
            {
                throw new ExcepcionNegocio("Error.Rule.NameRequired");
            }

            return normalizado;
        }

        private static string ValidarPalabrasClave(string palabrasClave)
        {
            string normalizado = (palabrasClave ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizado))
            {
                throw new ExcepcionNegocio("Error.Rule.KeywordsRequired");
            }

            return normalizado;
        }

        private void ValidarCategoriaVisible(int categoriaId, int usuarioActualId)
        {
            Categoria categoria = _categoriaRepositorio.ObtenerPorId(categoriaId);
            if (categoria == null)
            {
                throw new ExcepcionNegocio("Error.Rule.CategoryNotFound");
            }

            if (categoria.Usuario != null && categoria.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.Rule.CategoryNotAllowed");
            }
        }

        private void ValidarComercios(List<int> comercioIds)
        {
            if (comercioIds == null || comercioIds.Count == 0)
            {
                return;
            }

            foreach (int id in comercioIds)
            {
                Comercio comercio = _comercioRepositorio.ObtenerPorId(id);
                if (comercio == null)
                {
                    throw new ExcepcionNegocio("Error.Rule.CommerceNotFound");
                }
            }
        }

        private static void ValidarPropiedad(ReglaCategorizacion regla, int usuarioActualId)
        {
            if (regla == null)
            {
                throw new ExcepcionNegocio("Error.Rule.NotFound");
            }

            if (regla.Usuario == null || regla.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.Rule.NotOwned");
            }
        }

        private sealed class CandidatoRegla
        {
            public ReglaCategorizacion Regla { get; set; }
            public int MatchCount { get; set; }
            public int CoberturaTexto { get; set; }
            public bool TieneMatchComercio { get; set; }
        }
    }
}
