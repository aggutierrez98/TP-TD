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
2. Ubicar o crear el `.puml` correspondiente en `.agents/diagramas/plantuml/`.
3. Actualizar narrativa tecnica y funcional que cambie por el codigo.
4. Recién despues copiar/exportar la imagen al Google Doc.
5. Compilar la solucion al final para asegurar que el estado documentado coincide con un build valido.

## Docs to code workflow

1. Leer el cambio pedido en el documento formal.
2. Determinar si es aclaracion documental o cambio real de comportamiento.
3. Si impacta comportamiento, actualizar codigo respetando stack y arquitectura.
4. Actualizar PlantUML y luego el documento formal.
5. Compilar la solucion.

## Diagram rule

- Todo diagrama UML debe crearse en PlantUML.
- Guardar el `.puml` en el repo con nombres ordenables por requerimiento tecnico.
- No dejar diagramas solo como imagen embebida en documentos.
