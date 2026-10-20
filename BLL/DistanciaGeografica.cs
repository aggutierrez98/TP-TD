using System;

namespace BLL
{
    // Distancia sobre la superficie terrestre por la formula de Haversine (esfera de radio medio),
    // calculada en codigo y sin servicios externos. El error frente al elipsoide es menor al 0,5%,
    // suficiente para decidir si un comercio cae dentro del radio de una zona del usuario.
    public static class DistanciaGeografica
    {
        public const double RadioTerrestreMetros = 6371008.8;

        public static double CalcularMetros(decimal latitudOrigen, decimal longitudOrigen, decimal latitudDestino, decimal longitudDestino)
        {
            double latitudOrigenRadianes = ARadianes((double)latitudOrigen);
            double latitudDestinoRadianes = ARadianes((double)latitudDestino);
            double deltaLatitud = latitudDestinoRadianes - latitudOrigenRadianes;
            double deltaLongitud = ARadianes((double)longitudDestino - (double)longitudOrigen);

            double senoMedioDeltaLatitud = Math.Sin(deltaLatitud / 2);
            double senoMedioDeltaLongitud = Math.Sin(deltaLongitud / 2);
            double a = (senoMedioDeltaLatitud * senoMedioDeltaLatitud)
                + (Math.Cos(latitudOrigenRadianes) * Math.Cos(latitudDestinoRadianes) * senoMedioDeltaLongitud * senoMedioDeltaLongitud);

            // Min evita que el redondeo deje a apenas por encima de 1 en puntos antipodas.
            return 2 * RadioTerrestreMetros * Math.Asin(Math.Min(1d, Math.Sqrt(a)));
        }

        private static double ARadianes(double grados)
        {
            return grados * Math.PI / 180d;
        }
    }
}
