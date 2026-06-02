# Sources of Truth

## Formal document

- Google Doc del TP final: `https://docs.google.com/document/d/1fiUOTtxZipf-rsXXskIBwyjFz1ng-kLH/edit`

## Repo-first policy

- El comportamiento real del sistema vive en el codigo.
- El codigo fuente de UML vive en PlantUML dentro del repo.
- El Google Doc refleja el estado aprobado del repo, no lo reemplaza.

## Diagram repository location

Guardar diagramas en:

- `diagramas/<requisito>/`

Ejemplos:

- `diagramas/T04-perfiles-usuario/`
- `diagramas/T05-multidioma/`
- `diagramas/T06a-bitacora/`

## Suggested file naming

Usar nombres ordenables por requerimiento tecnico dentro de la carpeta del requisito. El tipo de diagrama debe estar en el nombre del archivo:

- `diagramas/T02-login-logout/T02-login-logout-secuencia.puml`
- `diagramas/T04-perfiles-usuario/T04-perfiles-usuario-composite-clases.puml`
- `diagramas/T05-multidioma/T05-multidioma-observer-clases.puml`
- `diagramas/T07-digitos-verificadores/T07-digitos-verificadores-secuencia.puml`

## Sync rule

Primero actualizar `.puml` en repo, despues exportar/copiar imagen al Google Doc.
