# Spec Delta

## Purpose

Definir independencia de estado y resultados completos para la suite UI, de modo que cada caso sea verificable solo, repetido o junto a los demas.

## ADDED Requirements

### Requirement: Estado inicial independiente por caso
Cada caso UI SHALL establecer sus precondiciones sobre un estado conocido independiente de las escrituras, configuracion y archivos dejados por otros casos. SHALL obtener el mismo resultado funcional al ejecutarse individualmente, repetirse o cambiar de orden, sin necesitar una clase previa ni una base regenerada manualmente.

#### Scenario: Catalogo vacio despues de altas de categorias
- **WHEN** un caso que comprueba ausencia de categorias se ejecuta despues de otro que creo categorias
- **THEN** el caso inicia con su catalogo vacio y verifica la orientacion y el alta deshabilitada sin declararse inconcluso por datos ajenos

#### Scenario: Repeticion y orden alternativo
- **WHEN** se repite un caso o se invierte el orden de dos casos que modifican los mismos datos
- **THEN** sus precondiciones y resultados no dependen de las ejecuciones anteriores

### Requirement: Aislamiento de recursos y fallos de preparacion
La suite SHALL cerrar la aplicacion y liberar recursos de cada caso incluso ante fallos. La siguiente prueba SHALL recuperar un estado conocido antes de actuar. La preparacion destructiva SHALL limitarse a bases de prueba y no modificar la configuracion ni los datos productivos.

#### Scenario: Fallo despues de una escritura
- **WHEN** un caso falla tras modificar datos y termina su limpieza
- **THEN** el siguiente caso no hereda esas modificaciones ni una aplicacion abierta

#### Scenario: Preparacion fallida
- **WHEN** no se puede establecer el estado inicial requerido
- **THEN** el caso informa un fallo de preparacion y no ejecuta acciones sobre una base de estado desconocido

### Requirement: Resultado completo verificable
El runner SHALL devolver un codigo distinto de cero cuando algun caso seleccionado no haya terminado aprobado, incluidos resultados inconclusos o no ejecutados, aunque el ejecutor subyacente devuelva cero. SHALL contrastar contadores con resultados individuales del TRX y mostrar totales, ejecutados, aprobados, fallidos y omitidos. Una seleccion vacia SHALL fallar.

#### Scenario: Inconcluso con exit code cero
- **WHEN** el TRX contiene un resultado inconcluso o no ejecutado y el ejecutor devuelve cero
- **THEN** el runner falla e identifica el caso y su motivo sin reportar suite completa en verde

#### Scenario: Todos los seleccionados aprobados
- **WHEN** todos los casos seleccionados se ejecutan y aprueban
- **THEN** el runner devuelve cero y publica sus contadores reales
