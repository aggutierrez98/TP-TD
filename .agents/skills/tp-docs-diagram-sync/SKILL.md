---
name: tp-docs-diagram-sync
description: Keep source code, PlantUML diagrams stored in the repository, and the formal Google Docs specification synchronized in both directions. Use when a code change affects functional or technical documentation, when diagrams must be updated, or when document changes must be reflected back into code without violating the stack and architecture constraints.
---

# TP Docs Diagram Sync

Aplicar esta skill cuando haya que sincronizar codigo, diagramas UML y documento formal del TP.

## Read first

1. `.agents/01-stack-dotnet-winforms.md`
2. `.agents/02-architecture-7-layers.md`
3. [references/docs-sync-sources.md](references/docs-sync-sources.md)
4. La skill `google-drive:google-docs` cuando haya que editar el documento formal.

## Source of truth policy

Usar esta prioridad:

1. codigo del repo para comportamiento real
2. `.puml` del repo para UML
3. Google Doc para comunicacion y entrega formal

No actualizar el Google Doc sin antes actualizar el PlantUML fuente cuando el cambio impacta diagramas.

## Code to docs workflow

1. Identificar requerimiento tecnico afectado.
2. Determinar el requisito del TP impactado antes de tocar diagramas (`T02`, `T03`, `T04`, `T05`, `T06a`, `T06b`, `T07`, etc.).
3. Ubicar o crear el `.puml` correspondiente en `diagramas/<requisito>/`.
4. Si el diagrama existente esta en una carpeta vieja, moverlo primero a la estructura nueva y luego editarlo ahi.
5. Actualizar narrativa tecnica y funcional que cambie por el codigo.
6. Recién despues copiar/exportar la imagen al Google Doc.
7. Compilar la solucion al final para asegurar que el estado documentado coincide con un build valido.

## Docs to code workflow

1. Leer el cambio pedido en el documento formal.
2. Determinar si es aclaracion documental o cambio real de comportamiento.
3. Si impacta comportamiento, actualizar codigo respetando stack y arquitectura.
4. Identificar requisito y archivo destino bajo `diagramas/<requisito>/`.
5. Actualizar PlantUML y luego el documento formal.
6. Compilar la solucion.

## Diagram rule

- Todo diagrama UML debe crearse en PlantUML.
- Guardar el `.puml` en el repo bajo `diagramas/<requisito>/`.
- Mantener una carpeta por requisito; el tipo de diagrama debe quedar expresado en el nombre del archivo.
- Usar nombres ordenables y descriptivos.
- No dejar diagramas solo como imagen embebida en documentos.
- Los diagramas deben quedar balanceados entre si y contra el codigo real.
- Si una clase, atributo, metodo o parametro aparece en un diagrama, debe existir con ese mismo nombre en el codigo vigente.
- Si un flujo de secuencia invoca un metodo, ese metodo debe existir con ese mismo nombre y parametros en el diagrama de clases correspondiente y en el codigo.
- Los DER deben reflejar exactamente la persistencia real definida en `script_bd.sql`, incluyendo nombres de tablas, columnas, PK, FK, nullabilidad y restricciones relevantes.
- Antes de cerrar una tarea documental, revisar consistencia cruzada entre clases, secuencias, DER y codigo.
