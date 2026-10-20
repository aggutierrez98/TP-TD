# perfil-inversor Specification

## Purpose
Permitir que cada usuario configure y actualice un perfil inversor explicable mediante un cuestionario opcional, para orientar las recomendaciones segun su capacidad y tolerancia al riesgo.

## Requirements

### Requirement: Cuestionario opcional inmediatamente posterior al registro
El sistema SHALL ofrecer el cuestionario despues de confirmar el alta, con las acciones Comenzar y Omitir por ahora. Omitir o cerrar MUST conservar la cuenta creada y MUST NOT asignar un perfil por defecto ni impedir el ingreso posterior.

#### Scenario: Omitir despues del alta
- **WHEN** el usuario completa el registro y elige Omitir por ahora
- **THEN** su cuenta permanece creada, vuelve al flujo de login y no tiene perfil de riesgo

#### Scenario: Completar sin sesion previa
- **WHEN** el usuario comienza el cuestionario inmediatamente despues del registro
- **THEN** puede guardar el perfil solo para la cuenta recien creada, sin iniciar sesion automaticamente ni seleccionar otro propietario

#### Scenario: Alta rechazada
- **WHEN** el registro falla
- **THEN** no se ofrece guardar un perfil para una cuenta que no fue creada

### Requirement: Cuestionario completo y versionado
El cuestionario SHALL tener diez preguntas de opcion unica sobre objetivos, horizonte, liquidez, respaldo, ingresos, impacto de perdidas, experiencia, reaccion ante caidas y tolerancia. MUST comenzar sin opciones seleccionadas, conservar respuestas al retroceder y exigir respuestas validas para todas las preguntas antes del resultado.

#### Scenario: Navegacion sin respuesta
- **WHEN** se intenta avanzar sin responder la pregunta actual
- **THEN** el sistema solicita seleccionar una opcion y no avanza

#### Scenario: Correccion de una respuesta
- **WHEN** el usuario retrocede y modifica una respuesta
- **THEN** las otras respuestas se conservan y el resultado posterior considera la modificacion

#### Scenario: Entrada invalida
- **WHEN** se recibe una version desconocida, preguntas faltantes, duplicadas o extras u opciones inexistentes
- **THEN** la evaluacion se rechaza sin guardar un perfil parcial

### Requirement: Calculo reproducible de riesgo
El sistema SHALL calcular Bajo, Moderado o Alto mediante las reglas versionadas del diseño, separando capacidad y tolerancia, dando 80 % a la dimension menor y 20 % a la mayor. MUST aplicar el limite mas restrictivo por liquidez, horizonte, respaldo, gastos esenciales, experiencia y respuestas dispares. El modelo MUST NOT elegir ni modificar el resultado.

#### Scenario: Necesidad inmediata y tolerancia alta
- **WHEN** las respuestas indican gran tolerancia pero que el dinero debe estar disponible en cualquier momento
- **THEN** el perfil final es Bajo aunque el puntaje agregado alcance Alto

#### Scenario: Gastos comprometidos
- **WHEN** una perdida comprometeria gastos o deudas
- **THEN** el perfil final es Bajo

#### Scenario: Experiencia insuficiente
- **WHEN** todas las respuestas puntuan para Alto pero no hay experiencia previa
- **THEN** el perfil final es Moderado como maximo

#### Scenario: Condiciones compatibles con riesgo alto
- **WHEN** capacidad y tolerancia puntuan 100 y no se activa ningun limite
- **THEN** el resultado es Alto

#### Scenario: Mismas respuestas en otro idioma
- **WHEN** se responde con las mismas opciones estables en otro idioma
- **THEN** el nivel, puntajes y motivos son identicos

### Requirement: Resultado explicable y guardado explicito
El sistema SHALL mostrar nivel, descripcion y factores que determinaron el resultado antes de guardarlo. MUST permitir revisar respuestas y persistir solamente al confirmar Guardar perfil y continuar. El perfil MUST presentarse como orientativo, sin prometer rendimientos ni ausencia de perdidas.

#### Scenario: Limite aplicado
- **WHEN** un horizonte muy corto limita un resultado de puntaje alto
- **THEN** la explicacion informa que la necesidad proxima del dinero limita el riesgo a Bajo

#### Scenario: Cierre desde el resultado
- **WHEN** el usuario cierra el resultado sin guardar
- **THEN** no se crea ni reemplaza el perfil vigente

### Requirement: Perfil propio persistente y atomico
El sistema SHALL conservar una unica evaluacion vigente por usuario con respuestas, version, puntajes, limites y fecha. MUST reemplazar la evaluacion completa atomicamente, impedir acceso a perfiles ajenos y distinguir ausencia de perfil de riesgo Bajo.

#### Scenario: Alta posterior a la instalacion
- **WHEN** se registra una cuenta nueva y no se completa el cuestionario
- **THEN** la cuenta permanece sin perfil y no hereda el perfil de ejemplo

#### Scenario: Fallo durante el reemplazo
- **WHEN** falla el guardado de respuestas o el registro transaccional de la operacion
- **THEN** el perfil anterior permanece completo y el usuario puede reintentar

#### Scenario: Guardados concurrentes
- **WHEN** dos cuestionarios completos se guardan para el mismo usuario
- **THEN** existe una sola evaluacion vigente con las diez respuestas de un guardado completo, sin mezclar ambos

#### Scenario: Continuacion post-registro finalizada
- **WHEN** se intenta reutilizar una continuacion consumida, cancelada o vencida
- **THEN** no se permite guardar con ella y se indica completar el perfil desde Mi Configuracion tras ingresar

### Requirement: Perfil de ejemplo de riesgo alto en el script de base
El script de recreacion de la base SHALL cargar una evaluacion completa de riesgo Alto asociada exclusivamente al usuario de ejemplo `user`. MUST incluir version, fecha, diez respuestas validas, puntajes y motivos coherentes con el calculo real. Las otras cuentas del seed MUST permanecer sin perfil. El ejemplo MUST poder reemplazarse repitiendo el cuestionario.

#### Scenario: Instalacion desde el script
- **WHEN** se ejecuta el script de recreacion y carga de ejemplos
- **THEN** `user` tiene un unico perfil Alto de version 1, diez respuestas y capacidad, tolerancia y puntaje iguales a 100, sin limites aplicados

#### Scenario: Coherencia de las respuestas de ejemplo
- **WHEN** se recalculan las respuestas de ejemplo con las reglas version 1
- **THEN** se obtiene el mismo riesgo Alto y los mismos puntajes persistidos

#### Scenario: Nueva recreacion de prueba
- **WHEN** se vuelve a ejecutar el script sobre la base de pruebas mediante el harness
- **THEN** se recrean exactamente un perfil Alto para `user` y sus diez respuestas, sin duplicados

#### Scenario: Reemplazo del ejemplo
- **WHEN** `user` completa y guarda otra evaluacion desde Mi Configuracion
- **THEN** el nuevo resultado reemplaza el perfil de ejemplo como su unica evaluacion vigente

### Requirement: Consulta y repeticion desde Mi Configuracion
Mi Configuracion SHALL mostrar Sin perfil de riesgo y Completar cuestionario cuando no existe evaluacion, o nivel, fecha y Volver a hacer el cuestionario cuando existe. Repetir MUST mantener el perfil previo hasta confirmar el nuevo y MUST NOT alterar las preferencias de idioma.

#### Scenario: Primer cuestionario desde configuracion
- **WHEN** un usuario sin perfil completa y guarda el cuestionario
- **THEN** configuracion muestra el nivel y la fecha actualizados

#### Scenario: Repeticion cancelada
- **WHEN** un usuario con perfil Alto inicia otra evaluacion y la cancela
- **THEN** conserva Alto y su fecha anterior

#### Scenario: Repeticion guardada
- **WHEN** un usuario con perfil Alto guarda una evaluacion que resulta Bajo
- **THEN** se reemplaza por Bajo y se actualiza la fecha
