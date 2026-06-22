# DER desde la base de datos real (MSSQL Server)

Para crear o actualizar cualquier DER, **inspeccionar primero la estructura real de la base en
SQL Server** y replicarla fielmente. La base es la fuente de verdad de la persistencia; el DER
debe reflejarla exactamente y mantenerse balanceado contra `script_bd.sql` y el codigo.

## Conexion del proyecto

Tomada de `UI/App.config` (Windows / Integrated Security):

- Base de datos: `TpCatedraIDS`
- Instancias usadas en el repo: `.\SQLEXPRESS` (connection string `TpIDS`) y `.` (`TpIDS-UAI`).

```powershell
# Verificar conectividad
sqlcmd -S .\SQLEXPRESS -d TpCatedraIDS -E -Q "SELECT DB_NAME();"
```

Si la instancia local difiere, ajustar `-S`. Usar `-E` (autenticacion integrada de Windows).

## Que extraer

Para el DER hay que obtener exactamente de la base: tablas, columnas con tipo y longitud,
nullabilidad, claves primarias, claves foraneas (con tabla/columna referenciada) y unicidad.

### Tablas y columnas (tipo, longitud, nullabilidad)

```powershell
sqlcmd -S .\SQLEXPRESS -d TpCatedraIDS -E -Q "
SET NOCOUNT ON;
SELECT t.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE,
       c.CHARACTER_MAXIMUM_LENGTH AS LEN, c.IS_NULLABLE, c.ORDINAL_POSITION
FROM INFORMATION_SCHEMA.TABLES t
JOIN INFORMATION_SCHEMA.COLUMNS c ON c.TABLE_NAME = t.TABLE_NAME
WHERE t.TABLE_TYPE = 'BASE TABLE'
ORDER BY t.TABLE_NAME, c.ORDINAL_POSITION;"
```

### Claves primarias

```powershell
sqlcmd -S .\SQLEXPRESS -d TpCatedraIDS -E -Q "
SET NOCOUNT ON;
SELECT tc.TABLE_NAME, kcu.COLUMN_NAME, tc.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
  ON kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
ORDER BY tc.TABLE_NAME, kcu.ORDINAL_POSITION;"
```

### Claves foraneas (tabla/columna origen -> destino)

```powershell
sqlcmd -S .\SQLEXPRESS -d TpCatedraIDS -E -Q "
SET NOCOUNT ON;
SELECT fk.name AS FK, tp.name AS TABLA, cp.name AS COLUMNA,
       tr.name AS TABLA_REF, cr.name AS COLUMNA_REF
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
JOIN sys.tables tp  ON tp.object_id = fkc.parent_object_id
JOIN sys.columns cp ON cp.object_id = fkc.parent_object_id AND cp.column_id = fkc.parent_column_id
JOIN sys.tables tr  ON tr.object_id = fkc.referenced_object_id
JOIN sys.columns cr ON cr.object_id = fkc.referenced_object_id AND cr.column_id = fkc.referenced_column_id
ORDER BY tp.name, fk.name;"
```

## Flujo de trabajo del DER

1. Confirmar conectividad y que la base sea `TpCatedraIDS`.
2. Ejecutar las consultas de arriba para obtener tablas, columnas, PK y FK reales.
3. Construir el `.puml` del DER (subskill PlantUML, ver `er_diagrams.md`) replicando exactamente
   nombres de tablas, columnas, tipos, nullabilidad, PK y FK observadas.
4. Cruzar el resultado con `script_bd.sql`: si difieren, la base real manda para el DER, pero
   dejar constancia de la diferencia (puede indicar que el script quedo desactualizado).
5. Para G07 (DER integrado), repetir sobre todas las tablas y unificar en un solo diagrama.

## Si no hay acceso a la base

Si la instancia no esta disponible, avisar al usuario y, como ultimo recurso, derivar el DER de
`script_bd.sql`, dejando explicito que no se pudo validar contra la base real.
