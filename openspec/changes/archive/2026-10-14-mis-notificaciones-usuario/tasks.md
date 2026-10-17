# Tasks

## 1. Baseline y consulta personal

- [x] 1.1 Cargar skills obligatorias de implementación y revisar cambios locales compartidos en FormPrincipal/CompositionRoot; verificar baseline de build y tests según strict-tdd y registrar fallos preexistentes antes del ciclo. Baseline recuperada tras resolución del cambio JSON: runner -IncludeIntegration y build TpIDS.sln exit 0 el 2026-10-14.
- [x] 1.2 Añadir contratos BE de filtros y paginado y extender contrato de repositorio; escribir y observar tests de integración en rojo para consulta de tres tipos, aislamiento por destinatario, 24 registros en páginas 10/10/4, orden con fechas iguales, límites de fecha, total y página vacía; implementar consulta SQL transaccional y comprobar esos tests en verde sin cambiar semántica de ObtenerPorUsuario existente.
- [x] 1.3 Inyectar ISesion en NotificacionBLL y CompositionRoot; mediante rojo-verde implementar consulta personal y contador, validando ausencia de sesión, rango invertido y página inválida; verificar tests nuevos y regresión del BLL administrativo.

## 2. Detalle y lectura

- [x] 2.1 Mediante tests de integración rojo-verde implementar detalle y marcado de los tres tipos por Id/UsuarioId, modificando solo Leida; verificar rechazo de registros ajenos, rollback, lectura repetida y preservación de campos de recordatorios.
- [x] 2.2 Mediante tests BLL rojo-verde implementar detalle con lectura y marcado personal con auditoría transaccional; verificar retorno tras Save, idempotencia, notificación inexistente/ajena y fallo de persistencia sin falso éxito.

## 3. Bandeja y detalle WinForms

- [x] 3.1 Crear FormMisNotificaciones MDI y FormDetalleNotificacion con Krypton, solo Desde/Hasta y Buscar, grilla de solo lectura, botones de detalle/lectura y paginado de 10; verificar filtros aplicados, búsqueda reiniciada, navegación, vacío y detalle multilínea frente al diseño aprobado.
- [x] 3.2 Conectar lectura automática al abrir detalle y acción explícita desde fila; verificar estado actualizado solo tras guardado, doble clic válido y manejo de error sin marcar falsamente; añadir tests UI de estos recorridos después del desarrollo, respetando autorización para ejecutarlos.
- [x] 3.3 Incorporar keys de bandeja, tipos, detalle, errores y paginado y sincronizar translation-seeds/base.es.json; verificar catálogo y presentación al cambiar idioma. Catálogo español/inglés sincronizado; 62 traducciones cargadas transaccionalmente en TpCatedraIDS a pedido del usuario para actualizar su base existente, sin recrearla. Las sesiones abiertas deben recargar el idioma o reiniciar la app.

## 4. Menú, badge y actualización periódica

- [x] 4.1 Agregar entrada superior Mis notificaciones junto a Sesión y badge de no leídas con renderer nativo del tema, acceso por sesión activa y única instancia MDI; verificar usuario sin permiso administrativo, cantidad cero, varias cifras, foco y cambio de idioma/DPI. Verificación UI con el DPI actual del escritorio; otras escalas de Windows no se ejecutaron.
- [x] 4.2 Implementar coordinador de actualización asíncrona del contador, inicial/cada 30 segundos/tras lectura, conexión propia y una consulta en vuelo; añadir pruebas deterministas de respuesta antigua, lectura durante consulta, cierre y recuperación de fallo; verificar ausencia de bloqueos, consultas superpuestas y actualizaciones de controles cerrados.
- [x] 4.3 Integrar evento de lectura de bandeja con refresco inmediato del shell y disponer timer al cierre/logout; añadir cobertura UI posterior para contador y acceso personal, y verificar que los filtros no alteran el contador global.
- [x] 4.4 Sincronizar keys de menú y descripción accesible con catálogo base; verificar traducción del título y conservación del badge después del cambio de idioma.

- [x] 4.5 Corregir superposición del badge mediante ancho preferido y rectángulo de texto separados; verificar regresión de render en español/inglés y contadores de una, dos y cuatro cifras, suite unitaria/integración y build productivo. Rojo observado en los tres casos por invasión del área del badge; verde: runner Release -IncludeIntegration exit 0, TRX 493/493 unitarios y 170/170 integración; MSBuild TpIDS.sln Release exit 0, guards OK. Ajuste visual posterior solicitado: área de texto con ancho medido exacto, separación de 3 px antes del badge y 7 px libres a la derecha; tests de render, runner y build repetidos en verde. Pruebas de render sin tomar el escritorio; suite UI no repetida para esta corrección visual.

## 6. Verificación integrada

- [x] 6.1 Compilar TpIDS.sln con MSBuild y guards de arquitectura/CompositionRoot, y ejecutar tests/tools/run-tests.ps1 -IncludeIntegration; verificar exit code 0 y reportar contadores TRX reales, indicando cualquier limitación de SQL Server. Runner Release -IncludeIntegration -UiFilter FIN_MisNotificaciones exit 0: unitarios 490/490, integración 170/170. MSBuild TpIDS.sln /t:Build /p:Configuration=Release /m exit 0, guards OK.
- [x] 6.2 Por pedido explícito del usuario, ejecutar únicamente los tests UI del módulo con -UiFilter FIN_MisNotificaciones, advirtiendo que no verifica regresiones cruzadas; verificar resultados reales de las pantallas, traducciones y contador. TRX: 6/6 UI pasaron, exit 0; suite UI completa no ejecutada. Primer intento detectó layout de controles anclados fuera del cliente y lectura del contador mediante propiedad UIA equivocada; corregidos y comprobados en la corrida final.
- [x] 6.4 Ejecutar openspec validate mis-notificaciones-usuario --strict y contrastar escenarios con evidencias de verificación; comprobar que todas las tareas ejecutadas están marcadas y las diferidas tienen seguimiento formal antes de archivar. Implementación funcional y ajuste visual aceptados por el usuario. Por confirmación explícita, se retira de este cambio la verificación transversal de portabilidad: continúa en migrar-json-system-text-sin-nuget/tasks.md:5.2; el build local no acredita compilación limpia integral sin NuGet.
