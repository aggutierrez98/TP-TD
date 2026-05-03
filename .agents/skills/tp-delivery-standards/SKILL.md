---
name: tp-delivery-standards
description: Apply the TP delivery constraints for this WinForms .NET Framework system, including the documented stack, 7-layer architecture, plan de entregas, documentation parity, and PlantUML-only UML policy. Use when implementing features, bugfixes, architecture changes, persistence, auth, audit, DV, i18n, installers, reports, or project documentation.
---

# TP Delivery Standards

Aplicar esta skill cuando el cambio pueda impactar codigo, arquitectura, diagramas, documentacion o entregables del TP.

## Required inputs

Leer siempre:

1. `.agents/01-stack-dotnet-winforms.md`
2. `.agents/02-architecture-7-layers.md`
3. [references/plan-entregas-resumen.md](references/plan-entregas-resumen.md)

## Core constraints

1. Respetar .NET Framework 4.7.2 y WinForms.
2. No introducir tecnologias incompatibles con el stack.
3. Respetar la arquitectura de 7 capas y la separacion UI/BLL/DAL/BE/SERVICIOS/ABSTRACCIONES.
4. Mantener coherencia entre implementacion y artefactos de analisis/diseno.
5. Compilar la solucion antes de cerrar la tarea.

## Delivery checklist

Antes de dar por terminado un cambio, validar:

- requerimiento tecnico impactado identificado
- codigo alineado con la arquitectura
- diagramas afectados detectados
- documentacion afectada detectada
- controles visuales con nomenclatura correcta
- compilacion exitosa

## UML policy

- Todo UML nuevo o modificado debe hacerse en PlantUML.
- Guardar siempre el `.puml` en el repo.
- Ordenar diagramas por requerimiento tecnico del TP base.
- No usar herramientas visuales como unica fuente.

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
