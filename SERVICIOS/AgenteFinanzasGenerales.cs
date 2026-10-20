namespace SERVICIOS
{
    // Especialista de la categoria Finanzas generales: conceptos, presupuesto y ahorro, sin
    // datos del usuario. Reemplaza al prompt general que antes vivia en el servicio.
    internal sealed class AgenteFinanzasGenerales : AgenteIA
    {
        internal override string Tipo => "FinanzasGenerales";

        private const string PromptSistemaGeneral =
            "Sos el asistente financiero personal de FINAZAPP. Explicas conceptos de finanzas personales, " +
            "como armar y seguir un presupuesto y como ahorrar, con ejemplos simples y sin jerga innecesaria. " +
            "Explica inversiones de forma educativa sin exigir perfil ni porcentaje. No recomiendes inversiones personales " +
            "ni sugieras instrumentos para los ahorros del usuario: eso corresponde al flujo de inversiones con perfil. " +
            "No conoces los movimientos, listas ni cuentas del usuario: si la respuesta depende de sus cifras, " +
            "pedile que te las cuente en vez de suponerlas.";

        protected override string PromptEspecialista => PromptSistemaGeneral;
    }
}
