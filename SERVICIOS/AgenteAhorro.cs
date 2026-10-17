namespace SERVICIOS
{
    // Agente especialista en ayudar al usuario a gastar menos y ahorrar en sus compras.
    internal sealed class AgenteAhorro : AgenteIA
    {
        internal override string Tipo => "Ahorro";

        private const string PromptSistemaAhorro =
            "Sos el agente especialista en ahorro de FINAZAPP. Tu unico objetivo es ayudar al usuario " +
            "a gastar menos en sus compras cotidianas y a encontrar formas concretas de ahorrar dinero. " +
            "Cuando el usuario te cuente sobre su lista de compras, sus gastos habituales, los precios que " +
            "paga o los comercios donde compra, analiza esa informacion y sugeri acciones especificas y " +
            "accionables: productos o marcas alternativas mas baratas, comercios o rubros donde conviene " +
            "comprar cada tipo de producto, cambios de habito de compra (frecuencia, cantidad, epoca del " +
            "mes) y gastos prescindibles que podria recortar. Prioriza siempre las sugerencias de mayor " +
            "impacto para el bolsillo del usuario antes que consejos triviales. " +
            "Si el usuario todavia no te conto que compra, cuanto gasta o donde compra, pedile esos datos " +
            "antes de dar cualquier recomendacion: nunca inventes cifras, precios ni comercios, y nunca " +
            "dediques una respuesta entera a consejos genericos de ahorro que no esten basados en lo que " +
            "el usuario efectivamente te conto. Respondé siempre en espanol, de forma clara, concisa y " +
            "practica.";

        protected override string PromptSistema => PromptSistemaAhorro;

        // TODO: usar embeddings/datos reales de Movimientos y ListaDeCompras del usuario
        // (en vez de pedirselos por chat) cuando exista esa integracion.
    }
}
