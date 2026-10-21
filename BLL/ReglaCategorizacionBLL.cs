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

        public List<ReglaCategorizacion> ObtenerParaUsuarioActual()
        {
            return _reglaRepositorio.ObtenerPorUsuario(_sesion.ObtenerUsuarioActual());
        }

        public List<Comercio> ObtenerComerciosPorRegla(ReglaCategorizacion regla)
        {
            return _comercioReglaRepositorio.ObtenerComerciosPorReglaCategorizacion(regla);
        }

        // Devuelve la misma instancia recibida, con Id, Usuario (el de sesion) y la categoria
        // persistida cargados. Sin comercios la regla es general.
        public ReglaCategorizacion Crear(ReglaCategorizacion regla, IEnumerable<Comercio> comercios = null)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();
            string nombreNormalizado = ValidarNombre(regla?.Nombre);
            string palabrasClaveNormalizadas = ValidarPalabrasClave(regla.PalabrasClave);
            Categoria categoria = ValidarCategoriaVisible(regla.Categoria, usuarioActual);
            List<Comercio> comerciosAAsociar = ValidarComercios(comercios);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    regla.Usuario = usuarioActual;
                    regla.Categoria = categoria;
                    regla.Nombre = nombreNormalizado;
                    regla.PalabrasClave = palabrasClaveNormalizadas;

                    regla.Id = unidad.ReglasCategorizacion.InsertarYObtenerId(regla);

                    if (comerciosAAsociar.Count > 0)
                    {
                        unidad.ComercioReglaCategorizacion.ReemplazarComerciosDeReglaCategorizacion(regla, comerciosAAsociar);
                    }

                    _bitacoraBLL.RegistrarOperacionInterno(
                        "FormCategorizationRules.Value.Activity.RuleCreation",
                        "FormCategorizationRules.Value.Info.RuleCreation",
                        "ReglaCategorizacionBLL",
                        unidad);

                    unidad.Save();
                    return regla;
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

        // Recibe la regla con los valores nuevos. Valida la propiedad sobre la persistida
        // (no confia en el Usuario recibido), copia solo los campos editables y reemplaza
        // sus comercios.
        public bool Actualizar(ReglaCategorizacion regla, IEnumerable<Comercio> comercios = null)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();
            string nombreNormalizado = ValidarNombre(regla?.Nombre);
            string palabrasClaveNormalizadas = ValidarPalabrasClave(regla.PalabrasClave);
            Categoria categoria = ValidarCategoriaVisible(regla.Categoria, usuarioActual);
            List<Comercio> comerciosAAsociar = ValidarComercios(comercios);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ReglaCategorizacion persistida = unidad.ReglasCategorizacion.ObtenerPorId(regla.Id);
                    ValidarPropiedad(persistida, usuarioActual);

                    persistida.Nombre = nombreNormalizado;
                    persistida.Categoria = categoria;
                    persistida.PalabrasClave = palabrasClaveNormalizadas;

                    bool actualizado = unidad.ReglasCategorizacion.Actualizar(persistida);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.Rule.UpdateFailed");
                    }

                    unidad.ComercioReglaCategorizacion.ReemplazarComerciosDeReglaCategorizacion(persistida, comerciosAAsociar);

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

        public bool Eliminar(ReglaCategorizacion regla)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ReglaCategorizacion persistida = regla == null ? null : unidad.ReglasCategorizacion.ObtenerPorId(regla.Id);
                    ValidarPropiedad(persistida, usuarioActual);

                    unidad.ComercioReglaCategorizacion.ReemplazarComerciosDeReglaCategorizacion(persistida, new List<Comercio>());
                    bool eliminado = unidad.ReglasCategorizacion.Eliminar(persistida);
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
        public Categoria SugerirCategoria(string descripcion, Comercio comercio = null)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                return null;
            }

            if (!_sesion.HaySesionActiva())
            {
                return null;
            }

            List<ReglaCategorizacion> reglas = _reglaRepositorio.ObtenerPorUsuario(_sesion.ObtenerUsuarioActual());
            if (reglas == null || reglas.Count == 0)
            {
                return null;
            }

            string descripcionNormalizada = NormalizarTexto(descripcion);

            List<CandidatoRegla> candidatos = new List<CandidatoRegla>();

            foreach (ReglaCategorizacion regla in reglas)
            {
                List<Comercio> comerciosRegla = _comercioReglaRepositorio.ObtenerComerciosPorReglaCategorizacion(regla);
                bool tieneComerciosAsociados = comerciosRegla != null && comerciosRegla.Count > 0;

                // Elegibilidad:
                // Regla general (sin comercios) -> siempre elegible, TieneMatchComercio = false
                // Regla con comercios -> solo elegible si el movimiento tiene uno de esos comercios, TieneMatchComercio = true
                bool esElegible;
                bool tieneMatchComercio;

                if (tieneComerciosAsociados)
                {
                    if (comercio != null && comerciosRegla.Any(item => item.Id == comercio.Id))
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

        // Devuelve la categoria persistida: debe existir y ser global o del usuario actual.
        private Categoria ValidarCategoriaVisible(Categoria recibida, Usuario usuarioActual)
        {
            Categoria categoria = recibida == null ? null : _categoriaRepositorio.ObtenerPorId(recibida.Id);
            if (categoria == null)
            {
                throw new ExcepcionNegocio("Error.Rule.CategoryNotFound");
            }

            if (categoria.Usuario != null && categoria.Usuario.Id != usuarioActual.Id)
            {
                throw new ExcepcionNegocio("Error.Rule.CategoryNotAllowed");
            }

            return categoria;
        }

        // Devuelve los comercios persistidos, sin repetidos; ninguno = regla general.
        private List<Comercio> ValidarComercios(IEnumerable<Comercio> comercios)
        {
            List<Comercio> persistidos = new List<Comercio>();
            if (comercios == null)
            {
                return persistidos;
            }

            foreach (Comercio recibido in comercios)
            {
                Comercio comercio = recibido == null ? null : _comercioRepositorio.ObtenerPorId(recibido.Id);
                if (comercio == null)
                {
                    throw new ExcepcionNegocio("Error.Rule.CommerceNotFound");
                }

                if (!persistidos.Any(item => item.Id == comercio.Id))
                {
                    persistidos.Add(comercio);
                }
            }

            return persistidos;
        }

        private static void ValidarPropiedad(ReglaCategorizacion regla, Usuario usuarioActual)
        {
            if (regla == null)
            {
                throw new ExcepcionNegocio("Error.Rule.NotFound");
            }

            if (regla.Usuario == null || regla.Usuario.Id != usuarioActual.Id)
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
