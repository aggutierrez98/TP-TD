# Spec Delta

## Purpose

Limitar el asistente personal a temas financieros del propio usuario, rechazar de forma segura todo lo demas y evitar que una respuesta exponga datos ajenos o inventados.

## ADDED Requirements

### Requirement: Alcance tematico cerrado
El sistema SHALL responder solo preguntas de las categorias Compras (donde, cuando o con que medio de pago comprar lo de la lista del usuario), Inversiones (que hacer con el saldo del usuario) y Finanzas generales (conceptos, presupuesto y ahorro, sin datos del usuario). Toda otra pregunta, incluidas las mixtas, MUST rechazarse.

#### Scenario: Pregunta financiera admitida
- **WHEN** el usuario pregunta "conviene armar un presupuesto mensual?"
- **THEN** el asistente responde como Finanzas generales

#### Scenario: Pregunta fuera de tema
- **WHEN** el usuario pide una receta de cocina o un programa en algun lenguaje
- **THEN** el asistente muestra el mensaje fijo de rechazo y no consulta al modelo para responder

#### Scenario: Pedido mixto
- **WHEN** el usuario pide "dame una receta barata con lo de mi lista"
- **THEN** el asistente rechaza la pregunta completa

### Requirement: Prefiltro local antes del modelo
El sistema SHALL rechazar sin llamar a ningun servicio externo las preguntas que superen el largo maximo o contengan patrones obvios de inyeccion o de temas excluidos.

#### Scenario: Pregunta demasiado larga
- **WHEN** la pregunta supera el largo maximo configurado
- **THEN** se rechaza localmente y no se hace ninguna llamada a la IA

#### Scenario: Intento de inyeccion evidente
- **WHEN** la pregunta contiene "ignora las instrucciones anteriores"
- **THEN** se rechaza localmente como inyeccion

### Requirement: Clasificacion fail-closed
El sistema SHALL clasificar cada pregunta que pasa el prefiltro con una salida estructurada de valores cerrados y MUST rechazarla ante una categoria no admitida, una salida invalida o cualquier fallo de la clasificacion.

#### Scenario: Clasificador no disponible
- **WHEN** la clasificacion falla por red, cuota o respuesta mal formada
- **THEN** la pregunta se rechaza y no se responde como tema general

#### Scenario: Inyeccion detectada por el clasificador
- **WHEN** el clasificador devuelve la categoria de inyeccion
- **THEN** la pregunta se rechaza

### Requirement: Mensaje de rechazo fijo y traducible
El rechazo SHALL mostrarse con un texto fijo del catalogo de traducciones, en el idioma activo, y MUST NOT ser generado por el modelo.

#### Scenario: Rechazo en otro idioma
- **WHEN** el idioma activo es ingles y una pregunta se rechaza
- **THEN** el asistente muestra el texto de rechazo traducido al ingles

### Requirement: Registro de rechazos en Bitacora
Cada rechazo SHALL registrarse en la Bitacora con el usuario de la sesion y el motivo (prefiltro, categoria, fallo de clasificacion, limite de uso o validacion de salida), sin guardar la respuesta del modelo.

#### Scenario: Rechazo auditado
- **WHEN** se rechaza una pregunta fuera de tema
- **THEN** la Bitacora tiene un evento del usuario actual con el motivo del rechazo

### Requirement: Rechazos fuera del historial conversacional
Las preguntas rechazadas MUST NOT enviarse como historial al modelo en turnos posteriores.

#### Scenario: Turno siguiente a un rechazo
- **WHEN** el usuario envia una pregunta admitida despues de una rechazada
- **THEN** el contexto enviado al modelo no incluye la pregunta rechazada

### Requirement: Aislamiento de datos por sesion
El sistema SHALL obtener el usuario solo de la sesion activa. Las funciones que el modelo puede invocar MUST NOT recibir un identificador de usuario, el modelo MUST NOT generar consultas a la base, y solo datos ya filtrados del usuario actual MUST entrar al contexto.

#### Scenario: Pregunta por datos de otro usuario
- **WHEN** el usuario pide los movimientos o la lista de otro usuario
- **THEN** la respuesta no contiene datos de ningun otro usuario

### Requirement: Validacion de la respuesta en codigo
El sistema SHALL descartar una respuesta del modelo que contenga bloques de codigo o que cite comercios, promociones, cuentas o instrumentos ausentes del contexto entregado, y mostrar en su lugar un texto fijo traducible.

#### Scenario: Respuesta con entidad inventada
- **WHEN** la respuesta menciona un comercio que no estaba en el contexto
- **THEN** se descarta, se muestra el texto fijo y se registra en Bitacora

#### Scenario: Respuesta con bloque de codigo
- **WHEN** la respuesta incluye un bloque de codigo
- **THEN** se descarta y se muestra el texto fijo

### Requirement: Limite de uso por usuario
El sistema SHALL limitar la cantidad de preguntas por minuto de cada usuario y rechazar con un texto fijo traducible las que excedan el limite, sin llamar a la IA.

#### Scenario: Exceso de preguntas
- **WHEN** el usuario supera el limite de preguntas por minuto
- **THEN** la pregunta no llega a la IA y el asistente pide esperar
