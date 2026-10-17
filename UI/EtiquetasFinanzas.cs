using System;
using System.Collections.Generic;
using BE;

namespace TpIDS
{
    // Keys de traduccion de los enums de Entidades Financieras / Medios de Pago / Promociones.
    // Se escriben como literales (no concatenadas) para que sync-i18n-base-seed.ps1 las detecte.
    internal static class EtiquetasFinanzas
    {
        private static readonly Dictionary<TipoMedioPago, string> ClavesTipoMedioPago = new Dictionary<TipoMedioPago, string>
        {
            { TipoMedioPago.TarjetaCredito, "FormPaymentMethodManagement.Value.Type.TarjetaCredito" },
            { TipoMedioPago.TarjetaDebito, "FormPaymentMethodManagement.Value.Type.TarjetaDebito" },
            { TipoMedioPago.TarjetaPrepaga, "FormPaymentMethodManagement.Value.Type.TarjetaPrepaga" },
            { TipoMedioPago.CuentaBilleteraVirtual, "FormPaymentMethodManagement.Value.Type.CuentaBilleteraVirtual" },
            { TipoMedioPago.CuentaDNI, "FormPaymentMethodManagement.Value.Type.CuentaDNI" }
        };

        private static readonly Dictionary<TipoCuentaFinanciera, string> ClavesTipoCuentaFinanciera = new Dictionary<TipoCuentaFinanciera, string>
        {
            { TipoCuentaFinanciera.BilleteraDigital, "FormFinancialAccountManagement.Value.Type.BilleteraDigital" },
            { TipoCuentaFinanciera.CuentaCorriente, "FormFinancialAccountManagement.Value.Type.CuentaCorriente" },
            { TipoCuentaFinanciera.CajaAhorro, "FormFinancialAccountManagement.Value.Type.CajaAhorro" }
        };

        private static readonly Dictionary<DiaSemana, string> ClavesDiaSemana = new Dictionary<DiaSemana, string>
        {
            { DiaSemana.Lunes, "FormMerchantDetail.Value.Day.Lunes" },
            { DiaSemana.Martes, "FormMerchantDetail.Value.Day.Martes" },
            { DiaSemana.Miercoles, "FormMerchantDetail.Value.Day.Miercoles" },
            { DiaSemana.Jueves, "FormMerchantDetail.Value.Day.Jueves" },
            { DiaSemana.Viernes, "FormMerchantDetail.Value.Day.Viernes" },
            { DiaSemana.Sabado, "FormMerchantDetail.Value.Day.Sabado" },
            { DiaSemana.Domingo, "FormMerchantDetail.Value.Day.Domingo" }
        };

        public const string ClaveTodosLosDias = "FormMerchantDetail.Value.Day.All";

        public static string ClaveTipoMedioPago(TipoMedioPago tipo)
        {
            return ClavesTipoMedioPago.TryGetValue(tipo, out string clave) ? clave : tipo.ToString();
        }

        public static string ClaveTipoCuentaFinanciera(TipoCuentaFinanciera tipo)
        {
            return ClavesTipoCuentaFinanciera.TryGetValue(tipo, out string clave) ? clave : tipo.ToString();
        }

        public static string ClaveDiaSemana(DiaSemana dia)
        {
            return ClavesDiaSemana.TryGetValue(dia, out string clave) ? clave : dia.ToString();
        }

        public static IEnumerable<TipoMedioPago> TiposMedioPago
        {
            get { return (TipoMedioPago[])Enum.GetValues(typeof(TipoMedioPago)); }
        }

        public static IEnumerable<TipoCuentaFinanciera> TiposCuentaFinanciera
        {
            get { return (TipoCuentaFinanciera[])Enum.GetValues(typeof(TipoCuentaFinanciera)); }
        }
    }
}
