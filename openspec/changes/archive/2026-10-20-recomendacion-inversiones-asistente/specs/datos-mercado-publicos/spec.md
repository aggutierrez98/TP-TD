# Spec Delta

## Purpose

Obtener cotizaciones, tasas, fondos y precios de cripto de fuentes publicas gratuitas sin API key, tolerando su falta de garantias de servicio.

## ADDED Requirements

### Requirement: Consulta acotada y con tiempo maximo
Cada consulta a una fuente publica SHALL tener un tiempo maximo explicito y poder cancelarse, y MUST NOT bloquear la interfaz.

#### Scenario: Fuente lenta
- **WHEN** una fuente no responde dentro del tiempo maximo
- **THEN** la consulta se corta y se trata como fuente no disponible

### Requirement: Cache de datos recientes
El sistema SHALL reutilizar los datos de una fuente obtenidos dentro de su tiempo de vigencia en lugar de volver a pedirlos.

#### Scenario: Dos preguntas seguidas
- **WHEN** el usuario hace dos preguntas de inversiones en pocos minutos
- **THEN** la segunda usa los datos ya obtenidos sin nuevas llamadas a esas fuentes

### Requirement: Degradacion ante fallas
Si una fuente falla o devuelve datos invalidos, el sistema SHALL recomendar con las fuentes disponibles e informar con un texto fijo traducible que datos faltaron.

#### Scenario: Cripto no disponible
- **WHEN** falla la fuente de precios de cripto en el nivel alto
- **THEN** la recomendacion incluye CEDEARs y acciones e informa que faltaron los precios de cripto

#### Scenario: Todas las fuentes caidas
- **WHEN** fallan todas las fuentes del nivel
- **THEN** el asistente da la orientacion del nivel sin instrumentos concretos e informa que no pudo obtener datos de mercado
