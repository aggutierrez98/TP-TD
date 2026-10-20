# Spec Delta

## ADDED Requirements

### Requirement: Perfil requerido para recomendaciones personales
El sistema SHALL exigir un perfil guardado para recomendaciones personales de inversion. Sin perfil MUST responder solamente un mensaje fijo traducible que indique completar el cuestionario desde Mi Configuracion, antes de pedir porcentaje, calcular montos o consultar instrumentos. El mensaje del usuario MUST NOT sustituir el perfil persistido.

#### Scenario: Recomendacion sin perfil ni porcentaje
- **WHEN** un usuario sin perfil pregunta en que le conviene invertir
- **THEN** recibe solamente la indicacion de completar el cuestionario, sin pedir porcentaje ni recomendar instrumentos

#### Scenario: Recomendacion sin perfil con porcentaje
- **WHEN** un usuario sin perfil pide invertir el 20 % y dice en el chat que acepta riesgo alto
- **THEN** recibe el mismo mensaje fijo y no se consultan herramientas de recomendacion, movimientos, mercado ni redactor

#### Scenario: Error al leer el perfil
- **WHEN** falla la consulta del perfil
- **THEN** se informa un error tecnico sin continuar con una recomendacion ni asumir un perfil

### Requirement: Educacion financiera disponible sin perfil
El sistema SHALL permitir explicaciones financieras generales sin perfil y sin porcentaje. MUST distinguir explicacion de recomendacion personal; una consulta mixta que incluya que le conviene invertir al usuario MUST seguir el flujo de recomendaciones y su requisito de perfil.

#### Scenario: Explicacion educativa
- **WHEN** un usuario sin perfil pregunta que es un plazo fijo
- **THEN** recibe una explicacion educativa sin invitacion obligatoria al cuestionario ni consulta de sus movimientos

#### Scenario: Consulta mixta
- **WHEN** un usuario sin perfil pregunta que es un CEDEAR y si le conviene comprarlo con sus ahorros
- **THEN** recibe solamente la indicacion de completar el perfil

### Requirement: Perfil vigente en cada consulta
Cada recomendacion SHALL usar el perfil persistido del usuario que envio el mensaje. El historial y las instrucciones del chat MUST NOT ampliar su riesgo ni las referencias permitidas. Un perfil cambiado MUST aplicarse al siguiente envio sin requerir reiniciar la aplicacion.

#### Scenario: Perfil reducido
- **WHEN** el usuario cambia de Alto a Bajo y vuelve a pedir una recomendacion
- **THEN** el nuevo contexto usa Bajo y no reutiliza instrumentos incompatibles mencionados en conversaciones anteriores

#### Scenario: Mensaje ya iniciado
- **WHEN** el usuario cambia su perfil mientras una recomendacion esta procesandose
- **THEN** ese envio usa la evaluacion capturada al prepararlo y el siguiente envio usa la nueva

## MODIFIED Requirements

### Requirement: Porcentaje indicado en la pregunta
Con perfil guardado, el sistema SHALL tomar de la pregunta el porcentaje a invertir y MUST validarlo entre 0 (excluido) y 100. Si falta o es invalido, el asistente MUST pedirlo con un texto fijo traducible y no asumir ningun valor. Sin perfil MUST prevalecer exclusivamente la indicacion de completar el cuestionario.

#### Scenario: Porcentaje presente
- **WHEN** el usuario con perfil escribe "quiero invertir el 20% de lo que me sobra"
- **THEN** el sistema usa 20%

#### Scenario: Porcentaje ausente
- **WHEN** el usuario con perfil escribe "en que invierto lo que me sobra?"
- **THEN** el asistente pregunta que porcentaje quiere invertir y no recomienda instrumentos

#### Scenario: Porcentaje invalido
- **WHEN** el usuario con perfil pide invertir el 150%
- **THEN** el asistente pide un porcentaje valido

### Requirement: Nivel definido por reglas en codigo
El sistema SHALL decidir viabilidad y monto por saldo y gasto, y limitar el riesgo por el perfil inversor. El riesgo efectivo MUST ser el menor entre el perfil y el maximo economico: Bajo en Ajustado, Moderado en Comodo y Alto en Alto economico. Con saldo no positivo MUST NOT recomendar instrumentos. El modelo MUST NOT decidir estos niveles.

#### Scenario: Saldo negativo
- **WHEN** el usuario tiene perfil y el saldo mensual promedio es negativo
- **THEN** el asistente no recomienda invertir y da consejos de ahorro

#### Scenario: Saldo ajustado
- **WHEN** el saldo es positivo pero bajo respecto del gasto y el usuario tiene perfil Alto
- **THEN** recomienda primero formar una reserva de 3 a 6 meses de gastos, con candidatos liquidos compatibles de riesgo Bajo

#### Scenario: Saldo comodo
- **WHEN** el saldo esta en el rango comodo y el perfil es Alto
- **THEN** el riesgo efectivo no supera Moderado y no se habilitan acciones ni cripto

#### Scenario: Saldo alto
- **WHEN** el saldo esta en el rango alto y el perfil es Alto
- **THEN** puede recomendar diversificar con CEDEARs, acciones y cripto compatibles con el horizonte y liquidez, ademas de alternativas de menor riesgo

#### Scenario: Saldo alto y perfil bajo
- **WHEN** el usuario tiene mucho saldo disponible y perfil Bajo
- **THEN** el riesgo efectivo es Bajo y no recibe recomendaciones de acciones, CEDEARs, cripto ni fondos de renta variable

### Requirement: Instrumentos del nivel con datos vigentes
El sistema SHALL consultar solo familias elegibles por riesgo efectivo, horizonte y liquidez, y citar solo instrumentos y valores del resumen actual. MUST excluir candidatos cuya compatibilidad necesaria no pueda verificarse y MUST NOT ampliar el riesgo ante fallas de fuentes. Bajo admite liquidez y plazo fijo compatible; Moderado suma renta fija; Alto suma renta variable, CEDEARs, acciones y cripto.

#### Scenario: Nivel comodo
- **WHEN** el nivel economico es Comodo y el perfil es Moderado
- **THEN** se consultan familias de riesgo Bajo y Moderado compatibles y no cotizaciones de acciones ni cripto

#### Scenario: Instrumento inventado
- **WHEN** el modelo menciona un instrumento ausente del resumen que identifica la validacion de salida
- **THEN** la respuesta se descarta segun la validacion del asistente

#### Scenario: Disponibilidad inmediata
- **WHEN** el perfil requiere disponer del dinero en cualquier momento
- **THEN** no se recomienda un plazo fijo tradicional ni un candidato sin disponibilidad compatible verificada

#### Scenario: Necesidad parcial durante el proximo año
- **WHEN** el usuario indica que necesitara retirar una parte durante el proximo año sin indicar su importe
- **THEN** se exige disponibilidad de como maximo un dia habil al monto completo, sin preaviso ni permanencia adicionales, y no se inventa una distribucion de cartera

#### Scenario: Horizonte expresado como intervalo
- **WHEN** el usuario indica que necesitara el dinero entre tres y doce meses
- **THEN** se excluye cualquier candidato con inmovilizacion contractual mayor a tres meses, incluso si seria menor a doce meses

#### Scenario: Sin datos de compatibilidad
- **WHEN** no hay candidatos cuya liquidez requerida pueda verificarse
- **THEN** se informa la limitacion con orientacion general, sin inventar condiciones ni recomendar productos incompatibles

#### Scenario: Referencia anterior incompatible
- **WHEN** el historial menciona un instrumento que quedo excluido por el riesgo actual
- **THEN** no integra las referencias permitidas para la nueva recomendacion

### Requirement: Aviso legal agregado por el sistema
Toda respuesta del flujo de inversiones con perfil SHALL terminar con el texto fijo traducible "Informacion orientativa, no constituye asesoramiento financiero", agregado por el sistema y no por el modelo. La indicacion de completar perfil MUST ser exclusivamente el mensaje fijo de ese requisito, sin anexar este aviso.

#### Scenario: Respuesta con recomendacion
- **WHEN** el asistente responde una recomendacion de inversiones a un usuario con perfil
- **THEN** el ultimo parrafo es el aviso legal en el idioma activo

#### Scenario: Respuesta de saldo negativo
- **WHEN** el asistente responde a un usuario con perfil que no conviene invertir
- **THEN** la respuesta tambien termina con el aviso legal

#### Scenario: Perfil ausente
- **WHEN** se bloquea una recomendacion por falta de perfil
- **THEN** la respuesta contiene solamente la indicacion de completar el cuestionario
