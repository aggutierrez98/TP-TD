using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class IdiomaBLL
    {
        private readonly IdiomaService idiomaService;

        public IdiomaBLL()
        {
            IIdiomaRepository idiomaRepository = new IdiomaDAL();
            IUsuarioPreferenciaRepository usuarioPreferenciaRepository = new UsuarioPreferenciaDAL();

            idiomaService = new IdiomaService(idiomaRepository, usuarioPreferenciaRepository);
        }

        public void InicializarAplicacion()
        {
            idiomaService.InicializarAplicacion();
        }

        public List<Idioma> ObtenerIdiomas()
        {
            return idiomaService.ObtenerIdiomas().ToList();
        }

        public Idioma ObtenerIdioma(int idiomaId)
        {
            return idiomaService.ObtenerIdioma(idiomaId);
        }

        public Idioma ObtenerIdiomaDefault()
        {
            return idiomaService.ObtenerIdiomaDefault();
        }

        public void AplicarIdiomaSesionUsuario(int usuarioId)
        {
            idiomaService.AplicarIdiomaSesionUsuario(usuarioId);
        }

        public bool GuardarPreferencia(int usuarioId, int idiomaId)
        {
            bool guardado = idiomaService.GuardarPreferencia(usuarioId, idiomaId);
            if (guardado)
            {
                AplicarIdiomaSesion(idiomaId);
            }

            return guardado;
        }

        public int CrearIdioma(string nombre, bool idiomaDefault)
        {
            return idiomaService.CrearIdioma(nombre, idiomaDefault);
        }

        public bool ActualizarIdioma(int idiomaId, string nombre, bool idiomaDefault)
        {
            return idiomaService.ActualizarIdioma(idiomaId, nombre, idiomaDefault);
        }

        public bool EliminarIdioma(int idiomaId)
        {
            return idiomaService.EliminarIdioma(idiomaId);
        }

        public void GuardarTraduccion(int idiomaId, string key, string valor)
        {
            idiomaService.GuardarTraduccion(idiomaId, key, valor);
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            return idiomaService.EliminarTraduccion(idiomaId, palabraId);
        }

        public void ImportarJson(int idiomaId, string rutaArchivo)
        {
            idiomaService.ImportarJson(idiomaId, rutaArchivo);
        }

        public bool EstaCompleto(int idiomaId)
        {
            return idiomaService.EstaCompleto(idiomaId);
        }

        public void AplicarIdiomaSesion(int idiomaId)
        {
            idiomaService.ValidarIdiomaSeleccionable(idiomaId);
            Idioma idioma = idiomaService.ObtenerIdioma(idiomaId);
            Sesion.GetInstance().CambiarIdioma(idioma);
        }

        public void GenerarDiccionarios()
        {
            idiomaService.GenerarDiccionarios();
        }
    }
}
