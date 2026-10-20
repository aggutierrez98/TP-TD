# Spec Delta

## ADDED Requirements

### Requirement: Notificaciones de demostracion
El script de datos de prueba SHALL cargar para `user` al menos una recomendacion de compras, una de inversiones sin monto y un recordatorio de movimientos, completos y sin leer en la primera carga. SHALL identificarlos como ejemplos, resolver el usuario por su nombre y permitir consultarlos sin servicios externos. Reejecutarlo MUST NOT duplicarlos ni restablecer su estado de lectura.

#### Scenario: Primera apertura en laboratorio
- **WHEN** se ejecutan el esquema y los datos de prueba y se inicia sesion como `user`
- **THEN** Mis notificaciones muestra los tres ejemplos y permite abrir sus mensajes completos sin ejecutar tareas ni llamadas a APIs

#### Scenario: Carga repetida
- **WHEN** se vuelve a ejecutar el script incluso en otra fecha despues de leer un ejemplo
- **THEN** conserva exactamente un ejemplar de cada ejemplo y su lectura, sin modificar Usuarios

#### Scenario: Contexto de los ejemplos
- **WHEN** se abre una recomendacion de ejemplo
- **THEN** se identifica como demostracion y usa datos compatibles con los catalogos y el perfil sembrados sin prometer precios o rendimientos vigentes
