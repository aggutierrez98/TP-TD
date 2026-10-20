# Design

## Context

Ver `proposal.md` (Why). Hallazgos del codigo al planificar:

- `UI/FormRegistro.cs` llama a `UsuarioBLL.Registrar`, que devuelve `bool`, crea el usuario en una transaccion y no inicia sesion. Despues muestra exito y cierra; `FormLogin` sigue siendo el punto de autenticacion.
- `FormConfiguracionUsuario` ya obtiene el usuario de `ISesion` y permite cambiar idioma.
- `AsistenteFinancieroBLL` clasifica, extrae `recomendar_inversion(porcentaje?)`, calcula el resumen y redacta. No lee un perfil. La rama sin porcentaje precede al calculo economico.
- `GeminiClasificadorConsultas` ya separa `INVERSIONES` de `FINANZAS_GENERALES`; hay que precisar los ejemplos y las consultas mixtas, no crear otro clasificador.
- `RecomendacionInversionesBLL` calcula cuatro niveles economicos con tres meses calendario completos. Hoy solo el nivel Alto habilita acciones/CEDEARs/cripto. `AgenteInversiones` incluso ordena no pedir perfil; esa instruccion debe reemplazarse.
- `ValidadorRespuestaAsistente` detecta referencias conocidas excluidas, pero no toda parafrasis ni instrumentos desconocidos. No es una garantia semantica general.
- La base normativa de inversiones esta en el delta del cambio completo `recomendacion-inversiones-asistente`, aun no en `openspec/specs/`. Este cambio es sucesor: conservar nombres de requisitos y archivar el predecesor antes de sincronizar/archivar sus MODIFIED.

## Goals / Non-Goals

**Goals:** resultado reproducible y explicable; guardar solo evaluaciones completas; autoridad en BLL; mismo cuestionario desde registro y configuracion; perfil vigente leido en cada recomendacion.

**Non-Goals:** perfil elegido por IA, parametros financieros editables por el usuario, broker/ejecucion, promesas de rentabilidad, expiracion automatica, recoleccion de montos patrimoniales exactos e historial navegable de perfiles.

## Decisions

### 1. Base de investigacion y autoria de las reglas

Fuentes consultadas durante el brainstorming del 2026-10-19:

| Fuente | Aporte utilizado |
|---|---|
| [Bankinter: test de perfil](https://www.bankinter.com/blog/finanzas-personales/test-perfil-inversor) | Diez preguntas, objetivos, horizonte, escenarios de caida del 15 %, volatilidad y preferencias. Es material educativo; no se presupone que sea su motor de idoneidad comercial. |
| [Indexa: calculo del perfil](https://support.indexacapital.com/es/esp/calculo-perfil-riesgo) | Separar capacidad y tolerancia, ponderar 80 % la menor y 20 % la mayor, tratar contradicciones y permitir actualizar respuestas. |
| [CMF: perfil del inversionista](https://www.cmfchile.cl/educa/621/w3-article-1252.html) | Considerar liquidez, ingresos, obligaciones, respaldo y conocimientos ademas de tolerancia. |

El usuario confirmo: educacion sin perfil, bloqueo de recomendaciones personales sin perfil, limite conservador ante dinero necesario para gastos proximos y recorrido visual. La cantidad/preguntas propuestas y el recorrido fueron presentados en el brainstorming. Los puntajes exactos, umbrales, limites y detalles tecnicos siguientes son decisiones de diseño propias para FINAZAPP, sujetas a revision del plan; no son una formula oficial bancaria ni una evaluacion profesional validada.

Alternativas descartadas: clasificar solo por suma total (compensa restricciones esenciales), clasificar con IA (no reproducible) o copiar sin adaptacion un test extranjero (no coincide con el alcance y los instrumentos actuales).

### 2. Cuestionario v1: texto, opciones e identificadores

Una opcion por pregunta, sin valor preseleccionado. Identificadores Q1..Q10 y opciones A..D, independientes del idioma; las opciones se listan en orden. Para los bloques de puntuacion, tres opciones valen 0/50/100 y cuatro opciones 0/33/67/100. Q7 no entra en la media: limita por experiencia.

| ID / bloque | Pregunta | Opciones (codigo: texto; puntos cuando corresponda) |
|---|---|---|
| Q1 / tolerancia | ¿Que buscas principalmente al invertir? | A: Preservar mis ahorros y priorizar evitar perdidas (0). B: Equilibrar crecimiento y riesgo (50). C: Buscar mayor crecimiento, aceptando perdidas importantes (100). |
| Q2 / capacidad | ¿Cuando pensas necesitar el dinero que invertirias? | A: En menos de 3 meses (0). B: Entre 3 y 12 meses (33). C: Entre 1 y 3 años (67). D: En mas de 3 años (100). |
| Q3 / capacidad | ¿Cuanta disponibilidad necesitas de ese dinero? | A: Necesito poder retirarlo en cualquier momento (0). B: Necesitaria retirar una parte durante el proximo año (50). C: Puedo mantenerlo invertido sin necesitarlo para mis gastos (100). |
| Q4 / capacidad | Si dejaras de recibir ingresos, ¿por cuanto tiempo podrias cubrir tus gastos con ahorros separados de esta inversion? | A: Menos de 1 mes (0). B: Entre 1 y 3 meses (33). C: Entre 3 y 6 meses (67). D: Mas de 6 meses (100). |
| Q5 / capacidad | ¿Como describis tus ingresos y tu capacidad de ahorro? | A: No siempre logro cubrir mis gastos (0). B: Cubro mis gastos, pero ahorro poco o mis ingresos varian mucho (50). C: Tengo ingresos estables y ahorro regularmente (100). |
| Q6 / capacidad | Si perdieras una parte importante de esta inversion, ¿como te afectaria? | A: Comprometeria mis gastos o el pago de mis deudas (0). B: Tendria que postergar un objetivo importante (50). C: No comprometeria mis gastos ni obligaciones (100). |
| Q7 / experiencia | ¿Que experiencia tenes invirtiendo? | A: Ninguna. B: Plazos fijos, cuentas remuneradas o fondos de bajo riesgo. C: Bonos o fondos con variaciones de precio. D: Acciones, CEDEARs u otros activos de alta volatilidad. |
| Q8 / tolerancia | Si tu inversion bajara un 15 % en un mes, ¿que harias? | A: Venderia todo (0). B: Venderia una parte (33). C: Mantendria, despues de revisar la situacion (67). D: Evaluaria invertir mas, entendiendo que podria seguir bajando (100). |
| Q9 / tolerancia | ¿Que perdida temporal podrias tolerar sin necesitar retirar el dinero? | A: No aceptaria perdidas (0). B: Hasta un 5 % (33). C: Hasta un 15 % (67). D: Mas del 15 % (100). |
| Q10 / tolerancia | ¿Con cual situacion te sentis mas comodo? | A: Menor variacion y menor rendimiento esperado (0). B: Variaciones moderadas para buscar mayor crecimiento (50). C: Variaciones fuertes y posibles perdidas importantes para buscar crecimiento a largo plazo (100). |

Los textos finales se escriben con ortografia y acentos en los catalogos traducibles. Los intervalos de Q2/Q4 se explicitan en ayudas: exactamente 3 meses pertenece a B, exactamente 12 meses a C, exactamente 3 años a C; en Q4, exactamente 1 mes pertenece a B, exactamente 3 a C, exactamente 6 a C. No pedir al modelo que redacte preguntas u opciones.

### 3. Algoritmo v1 y explicacion

1. Validar version 1 y exactamente una opcion existente para cada una de las diez preguntas. Rechazar faltantes, duplicadas, extras o valores fuera de catalogo. La UI no envia un nivel ni puntaje aceptable como autoridad.
2. `C = (Q2 + Q3 + Q4 + Q5 + Q6) / 5` y `T = (Q1 + Q8 + Q9 + Q10) / 4`, con `decimal`.
3. `P = 0.80 * min(C,T) + 0.20 * max(C,T)`, sin redondear para clasificar.
4. Perfil base: `P < 40` Bajo; `40 <= P < 70` Moderado; `P >= 70` Alto.
5. Aplicar todos los limites; gana el mas restrictivo:

| Condicion | Maximo | Motivo estable |
|---|---|---|
| Q2=A | Bajo | HorizonteMuyCorto |
| Q3=A | Bajo | LiquidezInmediata |
| Q4=A | Bajo | RespaldoInsuficiente |
| Q5=A | Bajo |GastosNoCubiertos |
| Q6=A | Bajo | GastosComprometidos |
| Q9=A | Bajo | NoAceptaPerdidas |
| Q2=B o C | Moderado | HorizonteLimitado |
| Q3=B | Moderado | LiquidezProxima |
| Q4=B | Moderado | RespaldoLimitado |
| Q5=B o Q6=B | Moderado | CapacidadLimitada |
| Q7=A o B | Moderado | ExperienciaLimitada |
| `max(Q1,Q8,Q9,Q10) - min(Q1,Q8,Q9,Q10) >= 67` | Moderado | RespuestasDispares |

`PerfilFinal = min(PerfilBase, limites aplicables)`. La diferencia entre respuestas no bloquea guardar: limita y explica; el usuario puede revisarlas. Alto exige asi horizonte mayor a tres años, experiencia en instrumentos con variacion y ausencia de las restricciones anteriores. No hacer que invertir mas dinero suba el riesgo.

Resultado visible: nivel, descripcion, C/T/P redondeados solo si se presentan y explicacion con claves traducibles de los limites activos. Cuando no hay limites, explicar capacidad y tolerancia; cuando hay varios, ordenar primero los de Bajo y despues los de Moderado, por orden de la tabla. Conservar todos los codigos en persistencia.

Vectores de aceptacion (opciones Q1..Q10):
- `A,A,A,A,A,A,A,A,A,A`: C=0, T=0, P=0, Bajo.
- `B,C,C,C,C,C,C,C,C,B`: C=86.8, T=58.5, P=64.16, Moderado.
- `C,D,C,D,C,C,D,D,D,C`: C=100, T=100, P=100, Alto.
- Vector Alto con Q2=A: C=80, T=100, P=84, pero Bajo por HorizonteMuyCorto.
- Vector Alto con Q7=A: P=100, pero Moderado por ExperienciaLimitada.
- Vector Alto con Q9=A: T=75, P=80, pero Bajo por NoAceptaPerdidas.
- Vector Alto con Q1=A: T=75, P=80, pero Moderado por RespuestasDispares.

### 4. Modelo persistente y transaccion

Proponer en BE `PerfilInversor`, `RespuestaPerfilInversor` y `NivelRiesgoInversor` (Bajo/Moderado/Alto). El calculador puro `CalculadorPerfilInversor` vive en BLL; `PerfilInversorBLL` coordina lectura, validacion, calculo y guardado. Repositorio propio y DAL sobre el patron existente, expuestos por `IUnidadDeTrabajo`. Construccion de BLL y dependencias solo en `CompositionRoot`.

Esquema incorporado al script de recreacion `script_bd.sql`:
- `PerfilInversor`: Id, UsuarioId (FK a Usuarios, UNIQUE), VersionCuestionario, FechaEvaluacionUtc, Capacidad/Tolerancia/Puntaje (`decimal(7,4)`), NivelRiesgo, MotivosLimite (JSON de codigos estables, `nvarchar(max)`). CHECK de version positiva, rango de puntajes y enum; JSON se valida en BLL, sin exigir funciones JSON al SQL del laboratorio.
- `PerfilInversorRespuesta`: PerfilInversorId (FK con cascade), PreguntaId, OpcionId, PK (PerfilInversorId, PreguntaId); CHECK Q1..Q10 y opcion A..D. BLL valida las opciones concretas de cada pregunta y la completitud.

Una evaluacion vigente por usuario. Guardar crea/actualiza cabecera y reemplaza diez respuestas en una sola transaccion junto con Bitacora. Serializar operaciones sobre el mismo UsuarioId (bloqueo de fila del usuario dentro de la transaccion) para evitar altas duplicadas; la restriccion UNIQUE es respaldo. Concurrencia de dos cuestionarios completos: gana el ultimo guardado confirmado, sin mezclar respuestas. Error SQL/Bitacora revierte todo; el formulario conserva el borrador para reintentar.

La ausencia de fila significa Sin perfil de riesgo; nunca se persiste Bajo como sustituto de desconocido. No tocar el DVH/DVV de Usuarios por guardar el perfil: tablas nuevas separadas, sin ampliar el alcance de integridad existente. No registrar respuestas ni credenciales en Bitacora. Se conserva version y fecha, sin historial completo de versiones del usuario.

Decision explicita del usuario posterior a la aprobacion inicial: `script_bd.sql` crea y precarga una evaluacion de ejemplo de riesgo Alto exclusivamente para la cuenta `user`, que ya existe en el seed. Resolver el propietario por Username (o reutilizar `@UsuarioBasicoId` dentro de su batch), nunca por un Id supuesto. Guardar version 1, fecha UTC de instalacion, C=100, T=100, P=100, nivel Alto, motivos vacios y las diez respuestas del vector `C,D,C,D,C,C,D,D,D,C`. El resultado persistido debe coincidir con el recalculo del calculador real. Las demas cuentas del seed no reciben perfil y cada alta posterior sigue comenzando sin perfil; `user` puede repetir el cuestionario y reemplazar el ejemplo por el mismo flujo personal.

El script conserva su contrato actual de eliminar y recrear la base y cargar ejemplos; no introducir migraciones incrementales ni otra via de instalacion en este cambio. Toda prueba que lo ejecute se dirige a `TpCatedraIDS_Test` mediante el harness existente. Dos instalaciones de prueba deben producir de nuevo exactamente un perfil Alto de `user` con diez respuestas, no conservar el estado de la instalacion anterior.

### 5. Registro sin sesion: continuacion limitada

No se puede guardar por usuario de sesion inmediatamente tras `Registrar`: aun no existe sesion. Se descartan iniciar sesion automaticamente (cambiaria el login) y exponer `Guardar(usuarioId, ...)` a la UI (permitiria elegir propietario).

Decision tecnica: agregar una variante `UsuarioBLL.RegistrarParaOnboarding` que reutilice la misma alta transaccional y devuelva una continuacion opaca creada por BLL despues del commit. Mantener `Registrar` para consumidores existentes. La continuacion es un objeto sellado con constructor interno, Id de usuario no editable ni expuesto como parametro de guardado; vive solo en memoria y referencia exclusivamente al usuario recien creado. La UI puede transportarlo al formulario, no fabricarlo ni sustituir su propietario.

`PerfilInversorBLL.GuardarPostRegistro(continuacion, respuestas)` valida la continuacion (vigencia de 30 minutos con reloj inyectable, no cancelada/consumida) y toma de ella el propietario; serializa su consumo para impedir reutilizacion concurrente. Solo se consume despues de un commit exitoso; error permite reintento. Omitir/cerrar cancela la continuacion y descarta el borrador. No es sesion ni habilita otras operaciones. No se guarda en disco ni se entrega contraseña al nuevo formulario. Expiracion informa que puede completar el perfil desde Mi Configuracion tras ingresar; la cuenta permanece creada.

Desde Mi Configuracion, las operaciones publicas toman el propietario exclusivamente de `ISesion`, capturado al abrir/guardar. El asistente usa el Id capturado al enviar el mensaje mediante contrato interno BLL, no un Id provisto por pregunta/herramienta.

### 6. Pantallas y comportamiento aprobado

El boceto fue aprobado como recorrido y referencia visual; esta seccion es su especificacion duradera, sin depender de un servidor o HTML temporal.

- Tras exito del registro: dialogo con introduccion, tiempo estimado de 3 a 5 minutos y Comenzar/Omitir por ahora. Omitir no revierte el alta; al terminar el recorrido se vuelve al login existente.
- `FormPerfilInversor`: reutilizable para onboarding y configuracion, una pregunta por paso, indicador Pregunta N de 10, barra de avance, opciones, Anterior y Siguiente. No avanzar sin opcion. Volver conserva selecciones del borrador. Sin guardados parciales ni borradores persistentes.
- Tras Q10: calcular y mostrar resultado real (no el Moderado fijo del boceto), descripcion y motivos. Revisar respuestas vuelve al borrador; Guardar perfil y continuar confirma. Cerrar antes de guardar equivale a cancelar.
- Desde configuracion: mostrar Sin perfil de riesgo + Completar cuestionario, o nivel, fecha en horario local + Volver a hacer el cuestionario. Repetir comienza limpio, conserva vigente el anterior hasta guardar; cancelar o fallar no lo borra. Actualizar configuracion al volver con guardado exitoso, sin afectar el idioma seleccionado.
- Krypton, `FormTraducibleBase`, nomenclatura y layout redimensionable del proyecto. Preguntas largas con ajuste de linea, opciones accesibles por teclado y sin significado dependiente solo del color. Operaciones SQL sin congelar UI; aplicar patron asincrono existente con botones de guardado deshabilitados durante la operacion y errores traducidos.
- Familias de keys: `FormInvestorProfile.*`, `FormUserSettings.*`, `Error.InvestorProfile.*`, `Assistant.Investment.ProfileRequired`. Sin nuevos textos incrustados traducibles. Actualizar catalogos base y comprobar el generador existente.

Refinamiento visual aprobado por el usuario el 2026-10-20 tras revisar sus capturas: rehacer introduccion, preguntas y resultado con contenido compacto y jerarquia tipografica real (titulo destacado, pregunta legible, cuerpo Segoe UI), barra de progreso visible, opciones agrupadas en tarjetas y botones consistentes con accion principal verde y secundarias discretas. Mantener el tema Krypton, textos traducibles y controles accesibles. El contenido largo debe ajustarse al ancho y permitir scroll sin solaparse con el pie; redimensionar no escala fuentes. Verificar con capturas reales de introduccion/pregunta/resultado ademas de las comprobaciones funcionales.

### 7. Asistente: clasificacion y ausencia de perfil

Conservar limite de uso, prefiltro y clasificador fail-closed existentes. Precisar el clasificador y el agente general:
- «¿Que es un plazo fijo?» y «¿Que riesgos tienen las acciones?» => FINANZAS_GENERALES, explicacion educativa sin pedir perfil ni porcentaje.
- «¿En que me conviene invertir?», «¿Compro acciones con mis ahorros?» y pedidos mixtos con recomendacion personal => INVERSIONES. Una etiqueta educativa no debe utilizarse para recomendar personalmente.
- Ante fallo de clasificacion, mantener el rechazo existente; no inferir intencion a partir de una lista improvisada de palabras.

En la rama INVERSIONES, leer el perfil antes de function calling. Sin perfil, devolver SOLO el texto traducido: «Para darte recomendaciones personales de inversion, primero necesitas completar tu perfil inversor. Podes hacerlo desde Mi Configuracion → Completar cuestionario». No anexar aviso legal, saldo, porcentaje ni recomendaciones. Persistir el intercambio fijo como respuesta de orientacion del flujo (no rechazo de seguridad), igual que la peticion de porcentaje; el limite de uso sigue consumiendose. No llamar a herramientas, calculador economico, proveedores de mercado ni redactor; la llamada al clasificador si puede haberse realizado.

Error al consultar perfil no equivale a ausencia: informar error tecnico y no continuar con recomendaciones. No aceptar perfiles declarados en el chat. Leer el perfil actual por mensaje, no cachearlo junto al historial.

Con perfil, conservar la regla de porcentaje explicito y el calculo de monto. `ResumenInversiones` suma perfil/fecha/version, limites relevantes de horizonte/liquidez y riesgo efectivo. No enviar las diez respuestas al modelo: solo el resumen derivado necesario. El historial no puede ampliar las referencias actuales; para recomendaciones de inversion no reenviar intercambios anteriores de recomendaciones, evitando arrastrar instrumentos permitidos por un perfil anterior. La UI puede seguir mostrando el historial completo.

### 8. Riesgo efectivo y familias de instrumentos

No reemplazar los cuatro niveles economicos por el perfil. Los primeros determinan viabilidad, monto y necesidad de reserva; el perfil limita exposicion. Calculo fijo:
- Negativo: monto cero y orientacion para estabilizar gastos; no instrumentos.
- Ajustado: maximo riesgo efectivo Bajo y prioridad al fondo de emergencia.
- Comodo: maximo riesgo efectivo Moderado.
- Alto economico: maximo riesgo efectivo Alto.
- `RiesgoEfectivo = min(PerfilGuardado, MaximoEconomico)` para niveles positivos.

Universo acumulativo orientativo del TP, consultando solo familias elegibles:

| Riesgo efectivo | Familias candidatas |
|---|---|
| Bajo | FCI mercado de dinero y cuentas remuneradas; plazo fijo solo si no se exige liquidez inmediata, horizonte no es Q2=A y nivel economico no es Ajustado. |
| Moderado | Familias de Bajo y FCI de renta fija, manteniendo las restricciones de liquidez. |
| Alto | Familias anteriores y FCI de renta variable, CEDEARs, acciones, BTC/ETH/USDT de las fuentes ya existentes. |

Priorizar reserva en Ajustado incluso si el usuario tiene perfil Alto. No tratar un FCI de renta fija como garantizado ni una stablecoin como bajo riesgo. Las familias son una politica simplificada de este TP, no una certificacion individual: no inventar plazos de rescate, vencimientos o garantias que el proveedor no informe. Si falta un dato necesario para comprobar una restriccion concreta del usuario, excluir ese candidato y explicar la falta de alternativas verificables; nunca relajarlas para llenar el resumen. Para Q3=A, usar solo candidatos con disponibilidad compatible verificada por metadata o catalogo explicito versionado; si no hay tales datos, devolver orientacion sin productos concretos.

Politica operativa de compatibilidad (decision propia precisada en la revision del diseño): no calcular una distribucion de cartera ni inventar que porcentaje necesitara retirar. Aplicar las restricciones al monto completo y a cada candidato. La inmovilizacion contractual maxima admitida por Q2 es cero para A, tres meses para B, doce meses para C y treinta y seis meses para D, tomando el extremo inferior conservador del intervalo. Q3=A exige disponibilidad inmediata verificada, sin plazo de rescate, preaviso o permanencia; Q3=B exige acceso en como maximo un dia habil, sin preaviso ni permanencia adicionales; Q3=C no agrega un tope al de Q2. Gana siempre la condicion mas restrictiva. Para Q2=A, un rescate diferido tampoco es compatible. Un plazo fijo con vencimiento conocido solo es candidato cuando cumple ambos topes; por eso Q3=B lo excluye aunque el horizonte sea largo.

Para productos sin vencimiento fijo, los topes anteriores se comparan con el plazo de rescate/liquidacion que informa su condicion verificable, no con una fecha de venta ni una recuperacion de precio supuesta. Liquidez no significa preservacion del capital: acciones/CEDEARs/cripto siguen restringidos a riesgo efectivo Alto y horizonte Q2=D, aunque se puedan vender antes. La metadata debe distinguir meses calendario, dias habiles y disponibilidad inmediata; una ausencia o ambigüedad en esas condiciones excluye al candidato cuando son necesarias. No se requiere ampliar las fuentes: con los datos actuales puede resultar un resumen sin productos y esa salida esta prevista.

Mantener cache/timeout de proveedores y criterios de seleccion dentro de cada familia del cambio predecesor. La caida de una fuente no permite sustituir por una familia de mayor riesgo. Conservar universo de referencias excluidas conocido para el validador y agregar familias/nombres/tickers identificables prohibidos por la politica, sin consultar precios de familias prohibidas. Validar que las referencias de recomendaciones esten en el resumen actual; el prompt explica por que otras familias quedaron excluidas, pero no es el mecanismo que decide elegibilidad. Reconocer la limitacion del validador lexical actual en Risks.

### 9. Orden de trabajo, pruebas y documentacion

TDD estricto para reglas, onboarding, persistencia y cambios en el asistente. Pruebas del calculador: vectores anteriores, todos los limites, precedencia, 40/70 exactos en la funcion de clasificacion y equivalencia independientemente del idioma. Pruebas de integracion: propiedad, alta/update, rollback, unicidad, recreacion del esquema y seed Alto de `user` con respuestas/puntajes consistentes. Pruebas del asistente con dobles: sin perfil no llama tools/redactor/mercado; educacion si responde; porcentaje despues del perfil; perfil actualizado y referencias antiguas excluidas; cruce de los tres riesgos con los cuatro niveles economicos.

Los tests de UI se escriben despues del comportamiento: onboarding omitido/completado, navegacion, guardado/error, repeticion cancelada y configuracion. Ejecutar build y suite requerida segun skills; la suite UI requiere confirmacion del usuario por uso del escritorio.

No anticipar clases/DER en G07/G08. Las tareas documentales de este cambio coordinan su incorporacion a `actualizar-diagramas-tp` antes del cierre si ese cambio sigue abierto, o realizan los artefactos aqui con la skill correspondiente y alcance acordado. El progreso permanece en tasks.md.

## Risks / Trade-offs

- [Cuestionario simplificado y autodeclarado] → Descripcion orientativa, reglas explicitas y posibilidad de repetirlo; no prometer equivalencia con idoneidad regulatoria.
- [Familia no determina todo el riesgo ni la liquidez de un producto] → Restricciones por candidato, metadata/catálogo verificable y exclusion cuando no alcanza la evidencia; sin garantias inventadas.
- [No se pregunta cuanto de la inversion se necesitara retirar] → Aplicar el limite de liquidez a todo el monto; se acepta perder alternativas antes que inventar una distribucion personalizada.
- [Clasificador y redactor pueden equivocarse] → Ejemplos de intencion, consultas mixtas personales como INVERSIONES, rechazo ante fallo, contexto cerrado y pruebas negativas. La validacion lexical no detecta toda recomendacion parafraseada o desconocida; no afirmar que ofrece una garantia semantica absoluta.
- [Continuacion post-registro sin sesion] → Objeto opaco creado solo por BLL, propietario fijo, vencimiento/consumo/cancelacion y pruebas de reutilizacion. No aceptar un Id arbitrario.
- [Cambiar perfil mientras hay respuesta en curso] → Cada envio usa un snapshot leido antes de calcular; el siguiente envio ya toma el nuevo perfil. No cambiar retroactivamente mensajes ni cachear la evaluacion en el historial.
- [Cambio predecesor aun abierto] → Deltas apilados con nombres exactos y orden de archivo explicito; validar correspondencia antes de sincronizar.

## Migration Plan

1. Incorporar tablas y seed al `script_bd.sql` de recreacion existente. La cuenta `user` queda con el perfil Alto de ejemplo y sus diez respuestas; las otras cuentas quedan sin perfil. No hay migracion incremental.
2. Desplegar BLL/UI/asistente junto con la base generada por el script actualizado: el nuevo contrato exige perfil antes de recomendaciones y el usuario `user` ya puede utilizarlo con el ejemplo cargado.
3. Verificar dos recreaciones de la base de pruebas con el harness, el recalculo del seed de `user`, registro/omision y el reemplazo personal del ejemplo. La ejecucion del script productivo es una recreacion completa, no un paso de actualizacion que preserve datos.
4. Reversion: usar respaldo para recuperar una instalacion previa si fuera necesario; cambiar solo binarios no revierte una recreacion de datos. La version previa del asistente tampoco aplica el nuevo requisito de perfil.
5. Antes de archivar este cambio: archivar `recomendacion-inversiones-asistente`, verificar que su capacidad principal coincide con la base de los MODIFIED y ejecutar la validacion estricta de ambos artefactos afectados. No archivar automaticamente durante esta planificacion.
