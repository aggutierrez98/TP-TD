---
name: dal-sql-access-patterns
description: Enforce the repository pattern for SQL Server access in the DAL layer. Use when creating or modifying `AccesoDatos`, entity DAL classes, queries, inserts, updates, deletes, or connection handling so `ConfigurationManager` is used for connection strings and every database operation runs through `AccesoDatos` under a SQL transaction.
---

# DAL SQL Access Patterns

Aplicar esta skill en cualquier cambio de la capa DAL.

## Required rules

1. Toda operacion contra SQL Server debe pasar por `DAL/AccesoDatos.cs`.
2. Ninguna clase DAL de entidad debe crear `SqlConnection` o `SqlTransaction` directamente.
3. La connection string debe obtenerse desde `ConfigurationManager.ConnectionStrings`.
4. El uso de transacciones es obligatorio para toda operacion de base de datos, incluyendo lecturas y escrituras.
5. Las clases DAL de entidad solo deben delegar a `AccesoDatos` y mapear resultados.

## Expected repository shape

- `AccesoDatos` concentra:
  - apertura y cierre de conexion
  - inicio, commit y rollback de transacciones
  - creacion de comandos y parametros
  - ejecucion de lecturas y escrituras
- cada DAL de entidad:
  - define SQL y parametros
  - llama a `AccesoDatos`
  - transforma `DataTable` o resultados escalares a entidades BE

## Validation

Al finalizar cambios DAL:

1. revisar que no haya `SqlConnection` fuera de `AccesoDatos`
2. revisar que no haya `BeginTransaction` fuera de `AccesoDatos`
3. compilar `TpIDS.sln`
