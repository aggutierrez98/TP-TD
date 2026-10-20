# Spec Delta

## Purpose

Permitir que cada usuario registre sus zonas habituales con un radio propio, las vea en un mapa y que el sistema calcule la cercania de un punto a esas zonas.

## ADDED Requirements

### Requirement: Menu Mi usuario
El shell SHALL mostrar el menu "Mi usuario" en lugar de "Sesion", con los items Mi configuracion, Mis zonas y Cerrar sesion. Mis zonas MUST habilitarse solo con su permiso, que tienen el grupo basico y admin.

#### Scenario: Usuario basico
- **WHEN** un usuario del grupo basico inicia sesion
- **THEN** ve el menu "Mi usuario" con Mis zonas habilitado

#### Scenario: Usuario sin permiso
- **WHEN** un usuario sin el permiso de zonas inicia sesion
- **THEN** Mis zonas aparece deshabilitado y Mi configuracion y Cerrar sesion conservan su comportamiento

### Requirement: Alta de zona
El usuario SHALL poder agregar una zona con nombre, punto (por busqueda de direccion o clic en el mapa) y radio en metros. El nombre MUST ser obligatorio y unico entre las zonas del mismo usuario, y el radio MUST estar dentro del rango permitido.

#### Scenario: Alta por busqueda de direccion
- **WHEN** el usuario busca una direccion, elige una coincidencia, escribe "Trabajo", fija 600 m y presiona Agregar
- **THEN** la zona aparece en la grilla y como circulo de 600 m en el mapa

#### Scenario: Alta por clic en el mapa
- **WHEN** el usuario hace clic en el mapa y completa nombre y radio
- **THEN** la zona se guarda con las coordenadas del clic

#### Scenario: Nombre repetido
- **WHEN** el usuario agrega una zona con el nombre de otra suya, sin distinguir mayusculas
- **THEN** el sistema rechaza el alta con un mensaje traducible y no guarda nada

#### Scenario: Sin punto elegido
- **WHEN** el usuario presiona Agregar sin haber elegido un punto
- **THEN** el sistema pide elegir un punto y no guarda nada

### Requirement: Baja de zona
El usuario SHALL poder quitar la zona seleccionada, previa confirmacion. Solo alta y baja estan disponibles; no hay edicion.

#### Scenario: Quitar seleccionada
- **WHEN** el usuario selecciona una zona, presiona Quitar seleccionada y confirma
- **THEN** la zona desaparece de la grilla y del mapa

### Requirement: Zonas propias del usuario
Toda consulta, alta o baja de zonas SHALL operar solo sobre las zonas del usuario de la sesion.

#### Scenario: Zonas de otro usuario
- **WHEN** dos usuarios tienen zonas cargadas y uno abre Mis zonas
- **THEN** ve solo las suyas y no puede quitar las del otro

### Requirement: Visualizacion en mapa
La pantalla SHALL dibujar cada zona como un circulo de su radio, con marcador y etiqueta con el nombre, y resaltar la zona seleccionada en la grilla.

#### Scenario: Seleccion en la grilla
- **WHEN** el usuario selecciona una zona en la grilla
- **THEN** el mapa se centra en ella

### Requirement: Distancia a las zonas
El sistema SHALL calcular en codigo, sin servicios externos, la distancia en metros entre un punto y cada zona del usuario, e informar si el punto cae dentro del radio de alguna y de cual esta mas cerca.

#### Scenario: Punto dentro de una zona
- **WHEN** un punto esta a 600 m del centro de "Trabajo", que tiene radio de 1000 m
- **THEN** el sistema informa que esta dentro de "Trabajo" a 600 m

#### Scenario: Punto fuera de todas
- **WHEN** un punto esta fuera del radio de todas las zonas
- **THEN** el sistema informa que no esta dentro de ninguna
