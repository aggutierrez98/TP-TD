# Proposal

## Why

FINAZAPP ya recomienda inversiones segun el saldo disponible, pero desconoce los objetivos, la tolerancia a perdidas y la necesidad de liquidez del usuario. Configurar el perfil inversor permite adaptar esas recomendaciones sin confundir capacidad de ahorro con disposicion a asumir riesgo.

## What Changes

- Cuestionario opcional de 10 preguntas inmediatamente despues del registro, con anterior/siguiente, resultado explicable y guardado explicito.
- Perfil de riesgo bajo, moderado o alto calculado por codigo, combinando capacidad y tolerancia, con limites por liquidez, gastos esenciales y experiencia.
- Persistencia por usuario de respuestas, version del cuestionario, puntajes, limites aplicados, resultado y fecha. Los registros nuevos comienzan sin perfil; el usuario de ejemplo `user` queda precargado con una evaluacion completa de riesgo Alto desde `script_bd.sql`.
- Mi Configuracion muestra el perfil y permite repetir el cuestionario; cancelar conserva el perfil anterior.
- El asistente permite educacion financiera sin perfil, pero ante recomendaciones personales devuelve solamente una invitacion fija a completar el cuestionario. El control precede a la extraccion de porcentaje y a la consulta de mercado.
- Con perfil, el codigo cruza riesgo, horizonte y liquidez con la situacion economica actual; el modelo recibe un contexto cerrado y no elige el perfil ni amplia los instrumentos permitidos.
- Traducciones, pruebas y tareas de sincronizacion documental para el recorrido aprobado visualmente.

## Capabilities

### New Capabilities

- `perfil-inversor`: cuestionario versionado, calculo explicable, propiedad de los datos, onboarding opcional y gestion personal del perfil.

### Modified Capabilities

- `recomendacion-inversiones`: requisito previo de perfil para recomendaciones personales, separacion de educacion financiera, seleccion por interseccion de capacidad economica y riesgo, y contexto/validacion del asistente.

Esta ultima capacidad esta implementada en el cambio completo `recomendacion-inversiones-asistente`, aun sin archivar. Su delta es la base de los requisitos MODIFIED de este cambio: el predecesor MUST archivarse antes de sincronizar o archivar estos deltas. No se alteran sus artefactos ni se afirma que la capacidad ya exista en las specs principales.

## Impact

- UI: `FormRegistro`, nuevo `FormPerfilInversor`, `FormConfiguracionUsuario`; mantener el flujo de login existente.
- BE/BLL/REPOSITORIO/DAL: agregado personal `PerfilInversor`, reglas y persistencia, `UsuarioBLL`, unidad de trabajo y `CompositionRoot`.
- Asistente: `AsistenteFinancieroBLL`, `RecomendacionInversionesBLL`, `ResumenInversiones`, clasificador, agentes y validacion de salida.
- SQL: tablas propias y seed de una evaluacion de riesgo Alto para `user` en el mismo `script_bd.sql`, respetando su recreacion completa de la base. No se incorpora un mecanismo de migracion incremental.
- Catalogos de traduccion y suites unitarias, integracion y UI. Sin nuevas dependencias productivas.
- G07/G08, CU y documentacion formal quedan representados por tareas de coordinacion del propio cambio, para ejecutarse contra la implementacion real.
- Fuera de alcance: operaciones de inversion, seleccion manual de un riesgo superior, vencimiento automatico del perfil e historial navegable de evaluaciones.
