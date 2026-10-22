# Spec Delta

## Purpose
Define como `script_bd.sql` carga el catalogo de traducciones desde `translation-seeds/`, de forma portable al laboratorio y sin artefactos intermedios.

## ADDED Requirements

### Requirement: Lectura directa del seed de traducciones
`script_bd.sql` SHALL cargar `translation-seeds\base.es.json` (idioma base) y `translation-seeds\base.in.json` (ingles) leyendolos directamente desde la ruta del repositorio indicada en `RepoRoot`. MUST NOT depender de copias del JSON en otra ubicacion, de scripts auxiliares ni de la carpeta `.agents/`.

#### Scenario: Repositorio sin carpeta .agents
- **WHEN** se ejecuta `script_bd.sql` en SQLCMD Mode con `RepoRoot` apuntando a una copia del repositorio que no contiene `.agents/`
- **THEN** la base se crea y `Palabra` y `Traduccion` quedan cargadas con los idiomas "Español" (default) y "English"

#### Scenario: Sin escritura fuera del repositorio
- **WHEN** se ejecuta `script_bd.sql`
- **THEN** no se crea ni modifica ningun archivo fuera de la base de datos (en particular, nada bajo `C:\Users\Public`)

### Requirement: Preservacion de Unicode
La carga SHALL conservar todos los caracteres de los JSON UTF-8, incluidos los que no pertenecen a la code page de la base (por ejemplo `→` y `−`), y SHALL tolerar un BOM UTF-8 al inicio del archivo.

#### Scenario: Caracteres fuera de Windows-1252
- **WHEN** el seed contiene `→` o `−` y la base usa la collation por defecto de la instancia
- **THEN** el valor guardado en `Traduccion.Valor` contiene ese caracter y no `?`

#### Scenario: JSON con BOM
- **WHEN** el archivo comienza con el BOM UTF-8
- **THEN** el JSON se interpreta sin errores de formato

### Requirement: Fallo explicito ante seed ilegible
Si un JSON no puede leerse, esta vacio o no contiene traducciones validas, el script SHALL abortar con un error que nombre el archivo afectado, sin continuar con una base a medio cargar.

#### Scenario: Archivo inexistente
- **WHEN** `RepoRoot` apunta a una carpeta sin `translation-seeds\base.es.json`
- **THEN** el script termina con el error de lectura de `base.es.json` y no inserta traducciones
