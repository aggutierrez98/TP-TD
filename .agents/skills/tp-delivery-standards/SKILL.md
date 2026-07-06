---
name: tp-delivery-standards
description: Apply the TP delivery constraints for this WinForms .NET Framework system, including the documented stack, 6-layer architecture, plan de entregas, documentation parity, and PlantUML-only UML policy. Use when implementing features, bugfixes, architecture changes, persistence, auth, audit, DV, i18n, installers, reports, or project documentation.
---

# TP Delivery Standards

Aplicar esta skill cuando el cambio pueda impactar codigo, arquitectura, diagramas, documentacion o entregables del TP.

## Required inputs

Leer siempre:

1. `.agents/01-stack-dotnet-winforms.md`
2. `.agents/02-architecture-7-layers.md`
3. [references/plan-entregas-resumen.md](references/plan-entregas-resumen.md)

Si el cambio toca referencias entre proyectos, movimiento de clases entre capas, nuevas entidades, servicios, repositorios o contratos, aplicar tambien [`../architecture-dependency-guard/SKILL.md`](../architecture-dependency-guard/SKILL.md).

Si la tarea consiste en crear, editar o validar diagramas UML o especificaciones de casos de uso, usar la skill central [`../tp-uml-y-casos-de-uso/SKILL.md`](../tp-uml-y-casos-de-uso/SKILL.md), que orquesta la subskill embebida [embedded-skills/plantuml/SKILL.md](embedded-skills/plantuml/SKILL.md) y la especificacion de casos de uso, y maneja la sincronizacion con el documento formal.

## Core constraints

1. Respetar .NET Framework 4.7.2 y WinForms.
2. No introducir tecnologias incompatibles con el stack.
3. Respetar la arquitectura de 6 capas y la matriz obligatoria de dependencias UI/BLL/DAL/BE/SERVICIOS/ABSTRACCIONES.
4. Mantener coherencia entre implementacion y artefactos de analisis/diseno.
5. Compilar la solucion antes de cerrar la tarea.

## Delivery checklist

Antes de dar por terminado un cambio, validar:

- requerimiento tecnico impactado identificado
- codigo alineado con la arquitectura
- diagramas afectados anotados (la actualizacion del diagrama es a demanda, no obligatoria por cambio)
- documentacion afectada detectada
- controles visuales con nomenclatura correcta
- compilacion exitosa

> La diagramacion no se hace despues de cada desarrollo. Se realiza solo a demanda, cuando el desarrollo esta completo y preguntando primero al usuario (ver `tp-uml-y-casos-de-uso`).

## UML policy

- Todo UML nuevo o modificado debe hacerse en PlantUML.
- Guardar siempre el `.puml` en el repo.
- Ordenar diagramas por requerimiento tecnico del TP base.
- No usar herramientas visuales como unica fuente.
- La subskill embebida `embedded-skills/plantuml` se carga solo cuando la tarea realmente implique trabajar con PlantUML; no leerla para cambios de codigo que no requieran diagramacion.

## Special attention areas from the plan

- login/logout, Singleton y sesiones
- encriptado y hashing
- bitacora y auditoria
- digitos verificadores e integridad
- perfiles/permisos con Composite
- idiomas con Observer
- instalador
- exportacion PDF
- serializacion

Si el cambio toca alguna de esas areas, revisar si tambien impacta documentacion formal o diagramas.
