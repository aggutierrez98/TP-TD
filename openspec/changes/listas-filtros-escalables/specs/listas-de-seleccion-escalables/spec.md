# Spec Delta

## Purpose
Define como se comportan las listas de seleccion multiple de los filtros de promociones (capacidad `gestion-promociones`) cuando los nombres son largos o el catalogo tiene muchos items: lectura del nombre completo, busqueda por texto que conserva lo marcado, contador de marcados y mensajes de lista vacia o sin resultados.

## ADDED Requirements

### Requirement: Lectura del nombre completo de cada item
Cada lista de filtros SHALL permitir leer el nombre completo de un item aunque sea mas ancho que el recuadro de la lista. El usuario MUST poder ver el texto completo sin redimensionar la ventana ni editar el item.

#### Scenario: Nombre mas ancho que la lista
- **WHEN** una lista contiene un item cuyo nombre no entra en el ancho del recuadro
- **THEN** el usuario puede leer el nombre completo desde la propia lista

#### Scenario: Nombre que entra en la lista
- **WHEN** todos los nombres de una lista entran en el ancho del recuadro
- **THEN** la lista se muestra igual que antes, sin elementos de lectura adicionales visibles

### Requirement: Busqueda por texto en las listas que pueden crecer
Cada lista de filtros cuyo catalogo puede crecer SHALL ofrecer una casilla de busqueda que muestre solo los items cuyo nombre contenga el texto escrito, sin distinguir mayusculas ni acentos. Vaciar la casilla MUST volver a mostrar todos los items. Buscar en una lista MUST NOT afectar a las demas listas.

#### Scenario: Buscar por parte del nombre
- **WHEN** el usuario escribe "mast" en la busqueda de la lista de medios de pago que contiene "Tarjeta de credito Mastercard" y "Tarjeta de credito Visa"
- **THEN** la lista muestra solo "Tarjeta de credito Mastercard"

#### Scenario: Busqueda sin distinguir mayusculas ni acentos
- **WHEN** el usuario escribe "CREDITO" y un item se llama "Tarjeta de crédito Visa"
- **THEN** el item aparece entre los resultados

#### Scenario: Vaciar la busqueda
- **WHEN** el usuario borra el texto de la casilla de busqueda
- **THEN** la lista vuelve a mostrar todos los items del catalogo

#### Scenario: Catalogo con muchos items
- **WHEN** una lista tiene cientos de items y el usuario escribe parte del nombre de uno
- **THEN** la lista queda reducida a los items que coinciden y el usuario puede marcar uno sin recorrer el resto

### Requirement: Los items marcados se conservan al buscar
Marcar o desmarcar un item MUST conservar su estado aunque la busqueda lo oculte despues, y al guardar la promocion SHALL usarse todo lo marcado en cada lista, este visible o no. El usuario MUST poder ver que hay items marcados ocultos por el filtro.

#### Scenario: Marcar, filtrar y guardar
- **WHEN** el usuario marca un producto, luego busca un texto que lo oculta y guarda la promocion
- **THEN** la promocion queda con ese producto incluido

#### Scenario: Volver a un item marcado
- **WHEN** el usuario marca un item, filtra la lista, y luego vacia la busqueda
- **THEN** el item sigue marcado

#### Scenario: Cargar una promocion existente con un filtro activo
- **WHEN** el usuario selecciona una promocion en la grilla mientras una busqueda esta escrita en una lista
- **THEN** los items de esa promocion quedan marcados y la busqueda se limpia o se aplica sin perder ninguno

### Requirement: Contador de items marcados
Cada lista de filtros SHALL mostrar cuantos items tiene marcados, contando tambien los que el filtro de busqueda oculta.

#### Scenario: Contador sin marcados
- **WHEN** ninguna casilla de una lista esta marcada
- **THEN** el contador indica cero marcados y la lista sigue significando "sin restriccion"

#### Scenario: Contador con items ocultos por el filtro
- **WHEN** hay tres items marcados y la busqueda oculta dos de ellos
- **THEN** el contador indica tres marcados

### Requirement: Distinguir lista vacia de busqueda sin resultados
Una lista cuyo catalogo no tiene items SHALL mostrar la ayuda gris que indica donde se dan de alta, y una lista cuya busqueda no coincide con ningun item SHALL mostrar un mensaje de que no hay resultados para el texto buscado. Los dos mensajes MUST NOT mostrarse a la vez ni confundirse.

#### Scenario: Catalogo sin items
- **WHEN** el catalogo de una lista no tiene ningun item
- **THEN** la lista muestra la ayuda gris de alta y no ofrece busqueda util

#### Scenario: Busqueda sin coincidencias
- **WHEN** el usuario escribe un texto que ningun item contiene
- **THEN** la lista muestra el mensaje de sin resultados y no la ayuda de alta

#### Scenario: Volver de sin resultados
- **WHEN** el usuario corrige la busqueda y algun item coincide
- **THEN** el mensaje de sin resultados desaparece y se listan los items que coinciden
