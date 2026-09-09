# Sources of Truth

## Formal document

- Google Doc del TP final: `https://docs.google.com/document/d/1vwaQ_KgrLtoiwhx-CT-iFmGwZ4FuOFq5wVy3qxs8ecY/edit` ("Trabajo Final TD Gutierrez")

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
- `diagramas/T06-bitacora-control-cambios/`

## Suggested file naming

Usar nombres ordenables por requerimiento tecnico dentro de la carpeta del requisito. El tipo de diagrama debe estar en el nombre del archivo:

- `diagramas/T02-login-logout/T02-login-logout-secuencia.puml`
- `diagramas/T04-perfiles-usuario/T04-perfiles-usuario-composite-clases.puml`
- `diagramas/T05-multidioma/T05-multidioma-observer-clases.puml`
- `diagramas/T07-digitos-verificadores/T07-digitos-verificadores-secuencia.puml`

## Sync rule

Primero actualizar `.puml` en repo, despues exportar/copiar imagen al Google Doc.

## Balance rule

- Los diagramas de clases, secuencia y DER deben ser consistentes entre si y con el codigo vigente.
- Los nombres de clases, atributos, metodos y parametros deben coincidir exactamente con el codigo real.
- Los DER deben construirse inspeccionando la base real en SQL Server (`TpCatedraIDS`, ver
  `der-desde-mssql.md`) y reflejar exactamente su persistencia; cruzar contra `script_bd.sql` y,
  ante diferencias, la base real manda.
