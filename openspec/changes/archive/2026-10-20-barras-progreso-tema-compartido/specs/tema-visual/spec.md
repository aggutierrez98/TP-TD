# Spec Delta

## ADDED Requirements

### Requirement: Estilo comun de barras de progreso
Toda barra de progreso de la aplicacion SHALL ser una `KryptonProgressBar` con el mismo estilo, definido en un unico punto de `UI/Tema/`: pista verde clara, relleno verde FINAZAPP, bordes suavemente redondeados y sin texto sobre la barra.

#### Scenario: Barra del perfil inversor
- **WHEN** se muestra el cuestionario del perfil inversor
- **THEN** la barra se dibuja con la pista verde clara y el relleno verde FINAZAPP, sin texto, y avanza segun la pregunta actual

#### Scenario: Barra del escaneo de tickets
- **WHEN** se escanea un ticket y la operacion esta en curso
- **THEN** se muestra una barra con el mismo aspecto que la del perfil inversor y una animacion de espera, sin porcentaje

#### Scenario: Barra nueva
- **WHEN** se agrega una barra de progreso a un formulario
- **THEN** obtiene el estilo comun aplicando el helper del tema, sin definir colores ni bordes en el formulario
