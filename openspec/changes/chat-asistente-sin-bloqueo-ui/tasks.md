# Tasks

## 1. Regresion y correccion del envio en BLL

- [ ] 1.1 Obtener baseline verde segun strict-tdd y reproducir el camino bloqueante con historial lento controlado; verificar que se distingue SQL previo al await de la espera HTTP y registrar en design.md cualquier hallazgo que cambie el diagnostico.
- [ ] 1.2 Agregar tests con señales acotadas para lectura lenta y guardado lento con servicio completado, verificando el retorno no bloqueante al llamador; ejecutar y observar los fallos antes de modificar la BLL, liberando siempre las señales.
- [ ] 1.3 Implementar la frontera de trabajo asincrono en AsistenteFinancieroBLL capturando pregunta/usuario antes de Task.Run y manteniendo HTTP con await y unidad de trabajo posterior a la respuesta; verificar que pasan las regresiones de 1.2 y los tests existentes de historial, persistencia, auditoria y errores.
- [ ] 1.4 Verificar con tests el servicio pendiente sin unidad de trabajo abierta y la propagacion de fallos de lectura/guardado; comprobar que no se usan conexiones compartidas en las dependencias del envio y actualizar design.md si requiere alguna decision adicional.

## 2. Continuaciones seguras en WinForms

- [ ] 2.1 Ajustar el handler para impedir reentrada y omitir controles/dialogos tras cerrar el formulario, manteniendo las actualizaciones en el hilo UI y restauracion en finally; verificar exito, error con pregunta conservada y cierre durante espera mediante comprobacion visual autorizada o prueba automatizada del formulario con dependencias controladas.
- [ ] 2.2 Verificar con el ejecutable recompilado que la ventana repinta, se puede mover y permite navegar el shell mientras responde; comprobar boton/Enter sin duplicados y documentar en design.md la evidencia o una limitacion real de entorno. Si se requiere automatizacion UI, pedir la confirmacion previa prevista por test-suite-verification antes de tomar el escritorio.

## 3. Verificacion integrada

- [ ] 3.1 Compilar TpIDS.sln con MSBuild segun winforms-build-verification y verificar exit code 0, incluyendo guards de arquitectura y CompositionRoot.
- [ ] 3.2 Ejecutar tests/tools/run-tests.ps1 -IncludeIntegration segun test-suite-verification y verificar exit code 0 y contadores reales de TRX; si SQL no esta disponible, ejecutar unitarios y dejar explicitada la limitacion sin declarar verificada la integracion.
- [ ] 3.3 Ejecutar openspec validate chat-asistente-sin-bloqueo-ui --strict, comprobar coherencia de artefactos y revisar el diff limitado al cambio antes de reportar el resultado.
