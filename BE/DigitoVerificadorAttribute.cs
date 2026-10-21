using System;
using ABSTRACCIONES;

namespace BE
{
    // Marca una propiedad para que participe en el calculo del digito verificador
    // horizontal. El Orden define de forma univoca la posicion del atributo dentro
    // de la entidad y participa en el calculo, lo que permite detectar el intercambio
    // de columnas (no solo el contenido).
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DigitoVerificadorAttribute : Attribute, IDigitoVerificador
    {
        public DigitoVerificadorAttribute(int orden)
        {
            Orden = orden;
        }

        // Posicion del atributo dentro de la entidad (1-based).
        public int Orden { get; }
    }
}
