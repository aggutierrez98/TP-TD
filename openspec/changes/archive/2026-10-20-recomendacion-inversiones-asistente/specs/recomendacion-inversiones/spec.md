# Spec Delta

## Purpose

Orientar al usuario sobre que hacer con una parte de su saldo mensual, con reglas expertas en codigo, datos de mercado vigentes y un aviso legal fijo.

## ADDED Requirements

### Requirement: Porcentaje indicado en la pregunta
El sistema SHALL tomar de la pregunta el porcentaje a invertir y MUST validarlo entre 0 (excluido) y 100. Si falta o es invalido, el asistente MUST pedirlo con un texto fijo traducible y no asumir ningun valor.

#### Scenario: Porcentaje presente
- **WHEN** el usuario escribe "quiero invertir el 20% de lo que me sobra"
- **THEN** el sistema usa 20%

#### Scenario: Porcentaje ausente
- **WHEN** el usuario escribe "en que invierto lo que me sobra?"
- **THEN** el asistente pregunta que porcentaje quiere invertir y no recomienda instrumentos

#### Scenario: Porcentaje invalido
- **WHEN** el usuario pide invertir el 150%
- **THEN** el asistente pide un porcentaje valido

### Requirement: Monto desde el saldo del usuario
El sistema SHALL calcular el saldo mensual como el promedio de ingresos menos egresos de los ultimos 3 meses del usuario de la sesion, y el monto a invertir como saldo por porcentaje.

#### Scenario: Saldo positivo
- **WHEN** el saldo mensual promedio es 300000 y el porcentaje es 20%
- **THEN** la recomendacion se basa en un monto de 60000

### Requirement: Nivel definido por reglas en codigo
El nivel de la recomendacion SHALL decidirse con reglas fijas del sistema segun el saldo y el gasto del usuario, y MUST NOT decidirlo el modelo.

#### Scenario: Saldo negativo
- **WHEN** el saldo mensual promedio es negativo
- **THEN** el asistente no recomienda invertir y da consejos de ahorro

#### Scenario: Saldo ajustado
- **WHEN** el saldo es positivo pero bajo respecto del gasto
- **THEN** recomienda primero un fondo de emergencia de 3 a 6 meses de gastos en instrumentos liquidos

#### Scenario: Saldo comodo
- **WHEN** el saldo esta en el rango comodo
- **THEN** recomienda plazo fijo y fondos de renta fija

#### Scenario: Saldo alto
- **WHEN** el saldo esta en el rango alto
- **THEN** recomienda diversificar con CEDEARs, acciones y cripto

### Requirement: Instrumentos del nivel con datos vigentes
El sistema SHALL consultar solo los tipos de instrumento del nivel calculado y la respuesta MUST citar solo instrumentos y valores presentes en el resumen entregado al modelo.

#### Scenario: Nivel comodo
- **WHEN** el nivel es comodo
- **THEN** se consultan tasas de plazo fijo y fondos de renta fija, y no cotizaciones de acciones ni cripto

#### Scenario: Instrumento inventado
- **WHEN** el modelo menciona un instrumento ausente del resumen
- **THEN** la respuesta se descarta segun la validacion de salida del asistente

### Requirement: Aviso legal agregado por el sistema
Toda respuesta de inversiones SHALL terminar con el texto fijo traducible "Informacion orientativa, no constituye asesoramiento financiero", agregado por el sistema y no por el modelo.

#### Scenario: Respuesta con recomendacion
- **WHEN** el asistente responde una pregunta de inversiones
- **THEN** el ultimo parrafo es el aviso legal en el idioma activo

#### Scenario: Respuesta de saldo negativo
- **WHEN** el asistente responde que no conviene invertir
- **THEN** la respuesta tambien termina con el aviso legal
