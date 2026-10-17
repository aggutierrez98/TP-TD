# Tasks

El progreso de este change vive solo en este archivo. **Propuesta pendiente de validacion:** no se toman las tareas 1 en adelante hasta que el usuario resuelva la 0.1. Requisito previo: `gestion-promociones` archivado. Los tests UI FlaUI se escriben despues del desarrollo (skill `strict-tdd`: no hay cambio de BLL, REPOSITORIO ni DAL).

## 0. Validacion previa

- [ ] 0.1 Validar con el usuario el alcance y las preguntas abiertas de `design.md`; no continuar sin su aprobacion explicita. Dejar las respuestas anotadas en `design.md` (pasar cada pregunta de "Open Questions" a "Decisions") y ajustar `proposal.md`, el spec y las tareas siguientes si cambia el alcance; verificar con `openspec validate listas-filtros-escalables --strict`.

## 1. Logica de lista filtrable

- [ ] 1.1 Implementar en `FormGestionPromociones` el conjunto de items marcados por lista y la reconstruccion de los items visibles segun el texto buscado (sin mayusculas ni acentos), re-marcando desde el conjunto; `ObtenerIdsMarcados` y la carga de una promocion pasan a usar el conjunto. Verificar que compila `TpIDS.sln` y que los tests UI de `FIN_PromocionesTests` existentes siguen pasando.
- [ ] 1.2 Unificar en un metodo la visibilidad de la ayuda gris de lista vacia y del mensaje de sin resultados, y el texto del contador de marcados; verificar con una captura con la lista vacia, con items y con una busqueda sin coincidencias.

## 2. Controles en la pantalla

- [ ] 2.1 Agregar la casilla de busqueda (`KryptonTextBox`, nomenclatura de `winforms-ui-control-naming`) a las listas elegidas en la decision 1 de `design.md`, el contador de marcados y el mensaje de sin resultados; respetar el minimo de apertura y el maximizado de la ventana. Verificar con una captura que el layout no se solapa.
- [ ] 2.2 Implementar la lectura del nombre completo segun la decision 3 de `design.md` (tooltip, scroll horizontal o ambos); verificar con un item de nombre largo insertado por SQL.
- [ ] 2.3 Limpiar las busquedas al cargar una promocion y en "Limpiar", segun las decisiones 7 y 8 de `design.md`; verificar manualmente en la app.

## 3. Traducciones

- [ ] 3.1 Agregar las keys nuevas (placeholder del buscador, contador, mensaje de sin resultados, tooltip si corresponde) a `translation-seeds/base.es.json` y `base.in.json`, ejecutar `translation-base-seed-sync` e insertarlas en `Palabra`/`Traduccion` de `TpCatedraIDS` y `TpCatedraIDS_Test`; verificar con una consulta la paridad es/en y que la pantalla se traduce al cambiar de idioma.

## 4. Tests UI

- [ ] 4.1 Agregar tests UI con catalogos grandes insertados por SQL (decenas de productos y comercios con nombres largos): buscar por parte del nombre y ver que la lista se reduce; marcar, filtrar y guardar la promocion y comprobar por SQL que el item oculto quedo persistido; busqueda sin coincidencias muestra el mensaje y no la ayuda de alta. Correr solo esa clase con `-UiFilter` y verificar el TRX.

## 5. Cierre e integracion

- [ ] 5.1 Compilar `TpIDS.sln` con MSBuild (skill `winforms-build-verification`) incluyendo la validacion de capas y del Composition Root; verificar exit code 0.
- [ ] 5.2 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` (skill `test-suite-verification`) y reportar los contadores reales del TRX; verificar exit code 0.
- [ ] 5.3 Avisar al usuario que la suite UI completa toma el escritorio unos 20 minutos (sin tocar mouse ni teclado), esperar su confirmacion y correr `tests/tools/run-tests.ps1 -IncludeIntegration -IncludeUi`; reportar los contadores reales del TRX y el exit code del script.
- [ ] 5.4 Si el cambio agrega alguna clase o estructura que los diagramas deban reflejar, anotarlo en `diagramas/TODO-actualizacion-diagramas.md`; si es solo comportamiento de pantalla, dejar registrado que no hay cambio de diagramas.
