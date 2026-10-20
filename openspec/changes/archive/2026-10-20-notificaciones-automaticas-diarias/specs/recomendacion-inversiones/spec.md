# Spec Delta

## MODIFIED Requirements

### Requirement: Porcentaje indicado en la pregunta
El sistema SHALL aceptar consultas de inversion sin porcentaje y recomendar alternativas compatibles con el perfil y la situacion financiera sin asumir porcentaje ni monto. Si se indica porcentaje, MUST validarlo entre 0 (excluido) y 100. Un valor explicitamente invalido SHALL solicitar correccion con texto fijo traducible, sin calcular un monto.

#### Scenario: Porcentaje presente
- **WHEN** el usuario escribe "quiero invertir el 20% de lo que me sobra"
- **THEN** el sistema usa 20%

#### Scenario: Porcentaje ausente
- **WHEN** el usuario escribe "en que invierto lo que me sobra?"
- **THEN** recomienda alternativas si cumple los requisitos de perfil y aptitud, sin exigir porcentaje ni inventar un monto

#### Scenario: Porcentaje invalido
- **WHEN** el usuario pide invertir el 150%
- **THEN** el asistente pide un porcentaje valido

### Requirement: Monto desde el saldo del usuario
El sistema SHALL calcular el saldo mensual como el promedio de ingresos menos egresos de los ultimos 3 meses calendario completos del usuario de la sesion. Solo con un porcentaje valido explicitamente indicado SHALL calcular el monto como saldo positivo por porcentaje; sin porcentaje SHALL omitir porcentaje y monto, conservando la evaluacion economica y de perfil.

#### Scenario: Saldo positivo
- **WHEN** el saldo mensual promedio es 300000 y el porcentaje es 20%
- **THEN** la recomendacion se basa en un monto de 60000

#### Scenario: Monto no solicitado
- **WHEN** el saldo mensual promedio es 300000 y no se indico porcentaje
- **THEN** no se presenta un monto ni se usa 0% o 100% como sustituto
