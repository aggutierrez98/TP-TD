using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class IdiomaBLL
    {
        private readonly IdiomaDAL idiomaDAL;
        private readonly UsuarioPreferenciaDAL usuarioPreferenciaDAL;
        private readonly IdiomaService idiomaService;

        public IdiomaBLL()
        {
            idiomaDAL = new IdiomaDAL();
            usuarioPreferenciaDAL = new UsuarioPreferenciaDAL();
            idiomaService = new IdiomaService();
        }

        public void InicializarAplicacion()
        {
            GenerarDiccionarios();

            Idioma idiomaDefault = ObtenerIdiomaDefault();
            if (idiomaDefault != null)
            {
                Sesion.GetInstance().CambiarIdioma(idiomaDefault);
            }
        }

        public List<Idioma> ObtenerIdiomas()
        {
            GenerarDiccionarios();
            return idiomaDAL.ObtenerIdiomas();
        }

        public Idioma ObtenerIdioma(int idiomaId)
        {
            return idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
        }

        public Idioma ObtenerIdiomaDefault()
        {
            Idioma idiomaDefault = idiomaDAL.ObtenerIdiomaDefault();
            if (idiomaDefault == null)
            {
                GenerarDiccionarios();
                idiomaDefault = idiomaDAL.ObtenerIdiomaDefault();
            }

            return idiomaDefault;
        }

        public void AplicarIdiomaSesionUsuario(int usuarioId)
        {
            Idioma idioma = ObtenerIdiomaParaUsuario(usuarioId);
            if (idioma == null)
            {
                throw new Exception("No se encontro un idioma default configurado.");
            }

            Sesion.GetInstance().CambiarIdioma(idioma);
        }

        public bool GuardarPreferencia(int usuarioId, int idiomaId)
        {
            Idioma idioma = idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
            idiomaService.ValidarIdiomaSeleccionable(idioma);

            bool guardado = usuarioPreferenciaDAL.Guardar(new UsuarioPreferencia
            {
                UsuarioId = usuarioId,
                IdiomaId = idiomaId
            });

            if (guardado)
            {
                AplicarIdiomaSesion(idiomaId);
            }

            return guardado;
        }

        public int CrearIdioma(string nombre, bool idiomaDefault)
        {
            string nombreNormalizado = idiomaService.NormalizarNombre(nombre);
            idiomaService.ValidarNombreDuplicado(idiomaDAL.ObtenerIdiomas(), nombreNormalizado, null);

            int idiomaId = idiomaDAL.InsertarIdioma(new Idioma
            {
                Nombre = nombreNormalizado,
                Default = false
            });

            if (idiomaDefault)
            {
                idiomaDAL.EstablecerIdiomaDefault(idiomaId);
            }

            AsegurarUnicoIdiomaDefault();
            return idiomaId;
        }

        public bool ActualizarIdioma(int idiomaId, string nombre, bool idiomaDefault)
        {
            string nombreNormalizado = idiomaService.NormalizarNombre(nombre);
            idiomaService.ValidarNombreDuplicado(idiomaDAL.ObtenerIdiomas(), nombreNormalizado, idiomaId);

            bool actualizado = idiomaDAL.ActualizarIdioma(new Idioma
            {
                Id = idiomaId,
                Nombre = nombreNormalizado
            });

            if (!actualizado)
            {
                return false;
            }

            if (idiomaDefault)
            {
                idiomaDAL.EstablecerIdiomaDefault(idiomaId);
            }
            else
            {
                Idioma idiomaActual = idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
                if (idiomaActual != null && idiomaActual.Default)
                {
                    throw new Exception("Debe existir exactamente un idioma default.");
                }
            }

            AsegurarUnicoIdiomaDefault();
            return true;
        }

        public bool EliminarIdioma(int idiomaId)
        {
            List<Idioma> idiomas = idiomaDAL.ObtenerIdiomas();
            Idioma idioma = idiomas.FirstOrDefault(item => item.Id == idiomaId);
            if (idioma == null)
            {
                return false;
            }

            if (idiomas.Count <= 1)
            {
                throw new Exception("No se puede eliminar el unico idioma existente.");
            }

            if (idiomaService.EsNombreIdiomaBase(idioma.Nombre))
            {
                throw new Exception("No se puede eliminar el idioma base del sistema.");
            }

            bool eliminado = idiomaDAL.EliminarIdioma(idiomaId);
            if (!eliminado)
            {
                return false;
            }

            if (idioma.Default)
            {
                Idioma reemplazo = idiomaDAL.ObtenerIdiomas()
                    .OrderBy(item => item.Nombre)
                    .FirstOrDefault();

                if (reemplazo != null)
                {
                    idiomaDAL.EstablecerIdiomaDefault(reemplazo.Id);
                }
            }

            AsegurarUnicoIdiomaDefault();
            return true;
        }

        public void GuardarTraduccion(int idiomaId, string key, string valor)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("La key no puede estar vacia.");
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new Exception("La traduccion no puede estar vacia.");
            }

            int palabraId = idiomaDAL.ObtenerOCrearPalabra(key.Trim());
            idiomaDAL.GuardarTraduccion(idiomaId, palabraId, valor.Trim());
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            Idioma idioma = idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
            if (idioma == null)
            {
                throw new Exception("No se encontro el idioma seleccionado.");
            }

            Traduccion traduccion = idioma.Traducciones
                .OfType<Traduccion>()
                .FirstOrDefault(item => item.PalabraId == palabraId);

            if (traduccion == null || traduccion.Palabra == null)
            {
                return false;
            }

            if (idiomaService.EsKeyBase(traduccion.Palabra.Key))
            {
                throw new Exception("No se pueden eliminar traducciones de keys base del sistema.");
            }

            return idiomaDAL.EliminarTraduccion(idiomaId, palabraId);
        }

        public void ImportarJson(int idiomaId, string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
            {
                throw new Exception("No se encontro el archivo JSON seleccionado.");
            }

            string contenido = File.ReadAllText(rutaArchivo);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, string> traducciones;

            try
            {
                traducciones = serializer.Deserialize<Dictionary<string, string>>(contenido);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("El archivo JSON no tiene un formato valido.", ex);
            }

            if (traducciones == null || traducciones.Count == 0)
            {
                throw new Exception("El archivo JSON no contiene traducciones.");
            }

            foreach (KeyValuePair<string, string> item in traducciones)
            {
                GuardarTraduccion(idiomaId, item.Key, item.Value);
            }
        }

        public bool EstaCompleto(int idiomaId)
        {
            return idiomaService.EstaCompleto(idiomaDAL.ObtenerIdiomaCompleto(idiomaId));
        }

        public void AplicarIdiomaSesion(int idiomaId)
        {
            Idioma idioma = idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
            idiomaService.ValidarIdiomaSeleccionable(idioma);
            Sesion.GetInstance().CambiarIdioma(idioma);
        }

        public void GenerarDiccionarios()
        {
            Idioma idiomaBase = AsegurarIdiomaBase();

            foreach (KeyValuePair<string, string> item in idiomaService.ObtenerTraduccionesBase())
            {
                if (idiomaBase.TieneTraduccion(item.Key))
                {
                    continue;
                }

                int palabraId = idiomaDAL.ObtenerOCrearPalabra(item.Key);
                idiomaDAL.GuardarTraduccion(idiomaBase.Id, palabraId, item.Value);
            }

            idiomaBase = idiomaDAL.ObtenerIdiomaCompleto(idiomaBase.Id);

            if (!idiomaService.EstaCompleto(idiomaBase))
            {
                throw new Exception("No se pudo completar el diccionario base del sistema.");
            }
        }

        private Idioma ObtenerIdiomaParaUsuario(int usuarioId)
        {
            GenerarDiccionarios();

            UsuarioPreferencia preferencia = usuarioPreferenciaDAL.ObtenerPorUsuarioId(usuarioId);
            if (preferencia != null)
            {
                Idioma idiomaPreferido = idiomaDAL.ObtenerIdiomaCompleto(preferencia.IdiomaId);
                if (idiomaPreferido != null && idiomaService.EstaCompleto(idiomaPreferido))
                {
                    return idiomaPreferido;
                }
            }

            return ObtenerIdiomaDefault();
        }

        private Idioma AsegurarIdiomaBase()
        {
            List<Idioma> idiomas = idiomaDAL.ObtenerIdiomas();
            List<Idioma> idiomasBase = idiomas
                .Where(item => idiomaService.EsNombreIdiomaBase(item.Nombre))
                .OrderBy(item => item.Id)
                .ToList();

            Idioma idiomaBase = idiomasBase
                .FirstOrDefault(item =>
                    string.Equals(item.Nombre, idiomaService.ObtenerIdiomaBaseNombre(), StringComparison.OrdinalIgnoreCase))
                ?? idiomasBase.FirstOrDefault();

            if (idiomaBase == null)
            {
                int idiomaId = idiomaDAL.InsertarIdioma(new Idioma
                {
                    Nombre = idiomaService.ObtenerIdiomaBaseNombre(),
                    Default = true
                });

                idiomaDAL.EstablecerIdiomaDefault(idiomaId);
                idiomaBase = idiomaDAL.ObtenerIdiomaCompleto(idiomaId);
            }
            else
            {
                if (!string.Equals(idiomaBase.Nombre, idiomaService.ObtenerIdiomaBaseNombre(), StringComparison.Ordinal))
                {
                    idiomaDAL.ActualizarIdioma(new Idioma
                    {
                        Id = idiomaBase.Id,
                        Nombre = idiomaService.ObtenerIdiomaBaseNombre()
                    });
                }

                foreach (Idioma duplicado in idiomasBase.Where(item => item.Id != idiomaBase.Id).ToList())
                {
                    idiomaDAL.ConsolidarIdioma(duplicado.Id, idiomaBase.Id);
                }

                idiomaDAL.EstablecerIdiomaDefault(idiomaBase.Id);
                idiomaBase = idiomaDAL.ObtenerIdiomaCompleto(idiomaBase.Id);
            }

            return idiomaBase;
        }

        private void AsegurarUnicoIdiomaDefault()
        {
            List<Idioma> idiomas = idiomaDAL.ObtenerIdiomas();
            int? idiomaDefaultId = idiomas.Where(item => item.Default)
                .Select(item => (int?)item.Id)
                .FirstOrDefault();

            if (!idiomaDefaultId.HasValue && idiomas.Count > 0)
            {
                idiomaDefaultId = idiomas
                    .OrderBy(item => item.Nombre)
                    .Select(item => (int?)item.Id)
                    .FirstOrDefault();
            }

            if (idiomaDefaultId.HasValue)
            {
                idiomaDAL.EstablecerIdiomaDefault(idiomaDefaultId.Value);
            }
        }
    }
}
