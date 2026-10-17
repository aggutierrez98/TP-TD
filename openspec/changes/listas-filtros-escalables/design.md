# Design

> **Estado: propuesta pendiente de validacion.** Las decisiones de "Open Questions" no estan tomadas; las recomendaciones son del autor de la propuesta y no se implementan hasta que el usuario las valide (tarea 0.1 de `tasks.md`).

## Context

Ver `proposal.md` (Why). Estado observado en `FormGestionPromociones`:

- Nueve `CheckedListBox` nativos (`clb...`), cada uno dentro de un panel de marco (`pnlMarco...`) y con un rotulo (`lblFiltro...`). `CheckedListBox` es una de las excepciones nativas documentadas en `.agents/01-stack-dotnet-winforms.md`, y `TemaFinanzapp` le aplica la paleta.
- Propiedades relevantes: `IntegralHeight = false`, `CheckOnClick = true`, sin `MultiColumn` ni `HorizontalScrollbar`. El scroll vertical aparece solo cuando los items no entran (verificado con un control suelto: 5 items sin barra, 15 con barra, alto de item 15 px).
- Cada lista guarda objetos `OpcionFiltro` (id y texto) como items. El formulario lee lo marcado con `CheckedItems` y lo restaura con `SetItemChecked` sobre `Items`. Es decir, hoy **lo marcado vive en el propio control**: si se quitaran items del control para filtrar, se perderia su estado.
- Cada lista tiene una ayuda gris de lista vacia (`lblVacio...`), hija de la lista, cuya visibilidad decide `ActualizarAyudasListasVacias()`.
- Segmentos tiene un caso particular: acepta valores que ya no estan en el catalogo para no perderlos al guardar.

## Goals / Non-Goals

**Goals:**
- Poder leer cualquier nombre completo y encontrar un item en un catalogo grande sin recorrerlo a mano.
- No perder lo marcado al filtrar, y ver cuantos items hay marcados.
- Mantener el comportamiento actual con catalogos chicos (sin ruido visual).

**Non-Goals:**
- Cambiar el modelo de filtros, la regla de alcance o la BLL.
- Paginar o filtrar desde la base: el volumen del TP se resuelve en memoria.
- Reemplazar el `CheckedListBox` por un control de terceros.
- Aplicar el mismo patron a otras pantallas (se evalua despues, si funciona aca).

## Decisions

Las decisiones firmes son las que no dependen de las preguntas abiertas:

**1. Lo marcado se guarda aparte del control.** Cada lista lleva un conjunto de ids (o valores, en segmentos) marcados, que es la fuente de verdad. El filtro reconstruye los `Items` visibles a partir del catalogo completo y re-marca desde ese conjunto. `ObtenerIdsMarcados` pasa a leer el conjunto, no `CheckedItems`. Alternativa descartada: ocultar items sin quitarlos, porque `CheckedListBox` no permite ocultar items individuales.

**2. El filtro compara sin mayusculas ni acentos**, igual que la busqueda por texto de promociones (`PromocionBLL.Buscar`), para que el usuario encuentre "credito" aunque el item diga "crédito".

**3. Sin cambios fuera de la UI.** El catalogo completo ya se carga en memoria al abrir la pantalla; filtrar es una operacion sobre esa lista.

## Open Questions

Ninguna esta decidida. En cada una va la recomendacion del autor.

1. **¿Buscador en las nueve listas o solo en las que pueden crecer?** Recomendacion: en productos incluidos y excluidos, categorias incluidas y excluidas, comercios y medios de pago; no en entidades, rubros ni segmentos, que son catalogos cortos por naturaleza. Menos controles en una pantalla que ya es larga. Si el usuario prefiere uniformidad, las nueve es una opcion valida y mas simple de explicar.
2. **¿La casilla de busqueda se muestra siempre o solo pasado un umbral de items?** Recomendacion: siempre visible en las listas elegidas en (1). Un umbral hace que la pantalla cambie de forma segun los datos y complica los tests; el costo de mostrarla de mas es una fila de 24 px.
3. **¿Scroll horizontal, tooltip con el nombre completo, o ambos?** Recomendacion: ambos. El tooltip resuelve la lectura puntual sin mover nada; el scroll horizontal (`HorizontalScrollbar` con `HorizontalExtent` calculado del nombre mas largo) permite ver el contexto. Si hay que elegir uno, el tooltip, por ser menos intrusivo.
4. **¿Casilla de busqueda sobre `CheckedListBox` (con `KryptonTextBox` arriba), o control con busqueda integrada?** Recomendacion: seguir con `CheckedListBox` y un `KryptonTextBox` arriba de cada lista. Es lo mas simple, respeta la excepcion nativa ya documentada y no agrega dependencias. Un control integrado obligaria a escribir o importar un control propio y a retocar el tema.
5. **¿Hace falta un dialogo selector con busqueda para catalogos muy grandes?** Recomendacion: no ahora. Con buscador y contador alcanza para cientos de items; un dialogo (lista grande con buscador y botones Aceptar/Cancelar, abierto desde un boton "Elegir...") recien se justifica con miles, y obliga a cambiar el layout de la pantalla. Dejarlo como siguiente paso si el catalogo real lo pide.
6. **¿Donde y como se muestra el contador?** Recomendacion: en el rotulo de la lista ("Productos incluidos (3)"), sin control nuevo; se oculta el numero cuando es cero para no ensuciar la vista. Alternativa: un label aparte junto a la casilla de busqueda.
7. **¿Que pasa con un filtro escrito al seleccionar otra promocion en la grilla?** Recomendacion: limpiar las busquedas al cargar una promocion y marcar sus items; es lo que el usuario espera al "cambiar de contexto".
8. **¿Que hace "Limpiar" con las busquedas?** Recomendacion: vacia las casillas de busqueda, ademas de desmarcar todo.

## Risks / Trade-offs

- [El conjunto de marcados puede desincronizarse del control] → una sola funcion reconstruye los items y los re-marca desde el conjunto; tests UI con filtro, marcado y guardado.
- [Un `CheckedListBox` con cientos de items y `Items.Clear()` en cada tecla puede parpadear] → suspender el redibujado (`BeginUpdate`/`EndUpdate`) al reconstruir; si no alcanza, un retardo corto antes de filtrar.
- [Mas controles en una pantalla ya larga] → el panel con scroll existente absorbe el alto; solo se agregan buscadores donde la pregunta 1 lo justifique.
- [El cartel de "sin resultados" y la ayuda gris comparten lugar] → un unico metodo decide cual se muestra, extendiendo `ActualizarAyudasListasVacias`.
- [Orden de archivado] → este change asume `gestion-promociones` ya archivado; si se tomara antes, el delta de `listas-de-seleccion-escalables` sigue siendo valido porque es una capacidad nueva.
