# Design

## Context

El dataset se reunió en siete etapas fuera del repositorio y hoy solo existe en la conversación de trabajo. Debe quedar versionado. `script_bd.sql` ya siembra el usuario `user` (grupo básico), las monedas ARS/USD, la tasa USD→ARS y ocho categorías globales; no se toca. Ver proposal.md para la motivación.

## Goals / Non-Goals

**Goals:**
- Un script SQL único, idempotente y legible, más las fuentes para regenerarlo.
- Que la app arranque sin error de integridad (DVH/DVV) tras cargarlo.

**Non-Goals:**
- Modelar MODO, planes comerciales (Plan Épico, Black+) o compra mínima: la descripción de la promoción lo aclara.
- Cargar la base de tests o integrarlo a `script_bd.sql`.
- Mantener los precios actualizados.

## Decisions

- **Script aparte (`script_datos_prueba.sql`)**, no un bloque en `script_bd.sql`. Así el esquema queda limpio y la carga es opcional. Alternativa descartada: anexarlo a `script_bd.sql`, que lo volvería obligatorio y rompería los tests que parten de catálogos vacíos.
- **Fuentes en `datos-de-prueba/*.json` más un generador Python** (como `.agents/scripts/migrar-designer-krypton.py`). El SQL se genera y se commitea. Alternativa descartada: escribir el SQL a mano, inviable para ~750 filas con relaciones.
- **Idempotencia por clave natural** (`IF NOT EXISTS` o `MERGE` por nombre) y resolución de FKs con subconsultas por nombre. Los literales de texto llevan prefijo `N'...'` por los acentos.
- **`user` ya existe**: el script busca su `Id` por `Username` y no inserta en `Usuarios`, evitando el problema de DVH/DVV por SQL directo.
- **Ajustes de curación aplicados en el generador:** "Oro" pasa a "Gold"; tres productos de pescado pasan a Congelados; las promos de Coto con producto incluido no llevan categoría incluida; se excluye el precio outlier del vino en Jumbo; no se carga tasa USD→ARS; se descartan promos que dependen de relaciones inexistentes.
- **Promociones**: se cargan con la vigencia real, salvo tres de las más cortas (Día de la Madre de Banco Provincia, Coto del 16 de octubre y Farmacity con American Express), cuyas fechas se adelantaron a viernes de octubre pasados para que la demo muestre promociones vencidas.
- **Reglas de categorización**: 28 reglas (Dividendos unificado en una, "cap interes" sin tilde agregado, "coto sucursal" en vez de "coto"), asignadas a `user`. Los comercios asociados se completan por prefijo de nombre.
- **Coordenadas**: geocodificación con OpenStreetMap Nominatim (~1 petición por segundo, `User-Agent` propio) en el generador; el resultado se guarda en el JSON y el script solo lleva números. Sin coincidencia clara queda NULL.
- **Cuentas del usuario**: los vínculos cuenta–medio se cruzan contra las relaciones entidad–medio cargadas; los inexistentes se omiten.

## Risks / Trade-offs

- [Datos desactualizados: precios, promos y direcciones cambian] → son datos de demo con fecha de consulta 2026-10; se documenta en el JSON.
- [Direcciones de ChatGPT sin verificar] → algunas figuran como `desactualizado`; se cargan igual y se aceptan como ejemplo.
- [Geocodificación incorrecta] → solo se acepta coincidencia clara; si no, NULL.
- [Cargar el script sobre la base de tests rompe tests de integración] → se documenta en el encabezado del script y no se ejecuta desde el harness.
- [Cruce de filtros: entidad × medio × segmento es producto cartesiano y puede cubrir combinaciones inexistentes] → aceptado para la demo.

## Migration Plan

Ejecutar `script_bd.sql` y luego `script_datos_prueba.sql`. Rollback: recrear la base (es de desarrollo).
