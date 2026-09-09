# Plan de Entregas Relevante

## Restricciones generales

- Mantener al menos 4 capas; este repo exige una arquitectura por capas con `REPOSITORIO` entre `BLL` y `DAL`, y esa restriccion prevalece.
- Balancear implementacion con diagramas de clases, DER/modelo de datos, casos de uso y diagramas de secuencia.
- Mantener calidad OO: cohesion, acoplamiento controlado, reuso, separacion tecnica/negocio.
- Mantener UI WinForms usable y consistente.
- Mantener manejo correcto de excepciones y registro en bitacora.
- Mantener base de datos normalizada y con integridad referencial.

## IS 2023 destacados

- `T01` Arquitectura base, persistencia, diagrama de componentes, pruebas de arquitectura.
- `T02` Login/logout y usuarios, uso de Singleton.
- `T03` Encriptado/hash para datos sensibles.
- `T04` Perfiles de usuario, uso de Composite y recursion.
- `T05` Multiples idiomas, uso de Observer y cambio dinamico.
- `T06` Bitacora y control de cambios.
- `T07` Digitos verificadores e integridad (item oficial).
- `T08` Backup y restauracion de la base de datos (extension no oficial del equipo, no es un item formal de la Carpeta Modelo).
- `G07/G08` Diagrama de clases y modelo de datos integrados por capa.

## TD 2023 destacados

- `N01/N02` Especificacion funcional y especificacion de Casos de Uso: son dos **tipos de
  artefacto** oficiales, repetidos por cada proceso de negocio del proyecto (no dos procesos
  distintos). Este repo indexa sus procesos de negocio reales con numeracion propia
  `NEG01`, `NEG02`, ... (ej. `NEG01-movimientos`, `NEG02-categorias`); cada uno tiene su propio
  N01 (analisis y diseno) y N02 (implementacion y pruebas).
- `A01` Instalador completo.
- `A02` Reporte/exportacion PDF.
- `A02` Serializacion.
- `D01/D02/D03` Manual de instalacion, ayuda en linea y material de apoyo.
- `G07/G08` Integracion final de diagramas de clases y modelo de datos.

## Politica UML

- Todo diagrama UML debe modelarse en PlantUML.
- Guardar el codigo fuente de diagramas dentro del repo.
- Ordenar diagramas por requerimiento tecnico del TP base antes de llevar imagenes al documento final.
