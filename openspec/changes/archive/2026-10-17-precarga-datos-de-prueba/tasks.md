# Tasks

## 1. Fuentes del dataset

- [x] 1.1 Crear `datos-de-prueba/` con un JSON por grupo (catálogos, comercios, productos y precios, promociones, reglas, datos del usuario de prueba) a partir de las Etapas 1 a 7
- [x] 1.2 Aplicar los ajustes de curación acordados (Gold, Congelados, promos 5/6, outlier del vino, tasa, promos descartadas)
- [x] 1.3 Validar longitudes máximas y unicidades del esquema contra los JSON

## 2. Geocodificación

- [x] 2.1 Geocodificar con Nominatim los comercios físicos y guardar latitud/longitud solo con coincidencia clara
- [x] 2.2 Revisar a mano los comercios sin coincidencia y confirmar que quedan en NULL

## 3. Generador y script

- [x] 3.1 Escribir el generador Python que emite `script_datos_prueba.sql` con resolución por nombre, `N'...'` e idempotencia
- [x] 3.2 Generar el script con encabezado que indique orden de ejecución y que no va sobre la base de tests

## 4. Verificación

- [x] 4.1 Ejecutar `script_bd.sql` y luego el script sobre una base descartable, y revisar los conteos por tabla
- [x] 4.2 Ejecutar el script por segunda vez y comprobar que los conteos no cambian
- [x] 4.3 Abrir la app con la base cargada: sin error de integridad, sesión como `user` con cuentas, categorías, movimientos y reglas visibles
- [x] 4.4 Verificar que hay promociones vencidas y que la mayoría está vigente al 2026-10-16
- [x] 4.5 Registrar en el README el uso del script de datos de prueba
