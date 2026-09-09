# G00. Descripcion Global del Producto

**Sistema:** FINAZAPP — "Tus finanzas en un solo lugar"

> Nota de caratula institucional: este documento es la fuente (`.md`) que se traslada al
> cuerpo de la carpeta de proyecto. Al armar el bibliorato final, completar la caratula segun
> la Carpeta Modelo TC/TD (Universidad, Facultad, Materia, Docente, Alumno, Legajo,
> Localizacion, Comision, Turno, Año, Nombre del Sistema, Version) con los datos de cursada
> vigentes; esos datos administrativos no se versionan en este repositorio.

## Historial de Revision

| Fecha | Version | Autor | Descripcion |
| --- | --- | --- | --- |
| 2026-08-26 | 1.0 | Agustin Gutierrez | Version inicial de G01 a G05, describiendo el alcance completo de FINAZAPP tal como se presentara terminado (no solo lo ya implementado en codigo). |
| 2026-08-29 | 1.1 | Agustin Gutierrez | Se agrega G03 Planificacion del proyecto (faltaba, exigida por la Carpeta Modelo 2023 vigente) y se renumeran las secciones siguientes: G03 Definiciones -> G04, G04 Roles -> G05, G05 Otros Requisitos -> G06. La numeracion queda G01-G06 en este documento, G07/G08 (clases/DER parciales) fuera de el. |
| 2026-08-29 | 1.2 | Agustin Gutierrez | Se corrige ambiguedad en la tabla de G03: N01 (analisis y diseno) y N02 (implementacion y pruebas) son artefactos que aplican a **cada** proceso de negocio del proyecto (NEG01-movimientos, NEG02-categorias), no a un proceso generico unico como sugeria la redaccion heredada del plan de entregas oficial de la catedra. |
| 2026-09-02 | 1.3 | Agustin Gutierrez | Se agrega en G06 la subseccion "Estandares normativos y marco regulatorio" (Ley 25.326/AAIP, Disposicion AAIP 2/2023, Ley 26.388, BCRA "A" 7724, Ley 26.831/CNV, Ley 21.526, Ley 24.240, Ley 11.723), redactada como decisiones de diseno de FINAZAPP justificadas por la normativa aplicable, en base al marco regulatorio aportado por el usuario. |

## Alcance de este documento

Este documento describe **FINAZAPP como producto completo**, es decir, el sistema tal como
se presentara al finalizar todo el proceso de desarrollo (segun el criterio de iteraciones e
incrementos del Proceso Unificado descripto en la Carpeta Modelo, seccion C). El desarrollo
actual del TP avanza de forma incremental: hoy estan implementadas las gestiones tecnicas
transversales (T01-T07: arquitectura base, login/logout, encriptado, perfiles, multiidioma,
bitacora/control de cambios, digitos verificadores); las gestiones de negocio de finanzas
personales (N0X) y las caracteristicas adicionales (A0X) y documentacion (D0X) se incorporaran
en etapas posteriores ya acordadas. G01 a G06 se redactan igual con la vision completa del
producto, porque asi lo exige la naturaleza de estos items (describir el negocio, la
planificacion y el alcance total, no solo el estado de avance). G07 y G08 quedan **fuera** de
este documento: la propia Carpeta Modelo los define como "diagrama de clases / modelo de datos
**parcial de todos los modulos implementados**", por lo que se seguiran actualizando en
paralelo al avance real del codigo, no de forma anticipada.

---

## G01. Proposito

FINAZAPP nace para resolver un problema cotidiano y medible: las personas manejan su
economia personal repartida entre bancos, billeteras virtuales, tickets en papel y memoria,
sin una vision unificada de sus ingresos, gastos y oportunidades de ahorro.

Segun datos citados en la presentacion del proyecto:
- El 55% de las personas en Argentina afirma tener un presupuesto, pero solo el 19% lleva un
  registro exacto de sus ingresos y egresos (Instituto de Estudios Financieros, IEF).
- El 72% de las personas trabajadoras en Argentina tiene algun tipo de deuda activa
  (plataforma de empleo Bumeran).

A esto se suma que cada banco y cada tarjeta ofrece sus propios beneficios y promociones de
forma dispersa, por lo que gran parte de los descuentos disponibles nunca llegan a
aprovecharse.

El proposito de FINAZAPP es **centralizar, automatizar y optimizar la economia cotidiana de
usuarios individuales**, combinando:

- **Centralizacion omnicanal**: un unico lugar para ver todos los movimientos financieros,
  sin importar su origen.
- **Motor contextual de ahorro**: cruzar las necesidades de compra del usuario con las
  promociones vigentes y su ubicacion, para que el ahorro sea automatico y no dependa de que
  el usuario este atento a cada oferta.
- **Asistencia financiera con Inteligencia Artificial**: acompañar al usuario con
  sugerencias de presupuesto, ahorro y orientacion de inversion en lenguaje simple, sin que
  necesite conocimientos financieros previos.

Se busca mejorar la toma de decisiones financieras del usuario final, reduciendo el "gasto a
ciegas" y aumentando la tasa de ahorro efectiva, sin que la complejidad tecnica o financiera
sea una barrera de uso.

## G02. Descripcion funcional del producto y Alcance

### Gestiones del sistema

La descripcion funcional a nivel global se organiza en dos grandes familias de gestiones:
las gestiones **de negocio** (el dominio de finanzas personales, razon de ser de FINAZAPP) y
las gestiones **tecnicas / transversales** (soporte, seguridad y calidad de servicio que
todo el sistema necesita para funcionar, independientemente del dominio de negocio).

#### Gestiones de negocio

| Gestion | Descripcion |
| --- | --- |
| Gestion de Ingesta y Consolidacion de Movimientos | Registro manual de ingresos/egresos, digitalizacion de tickets por OCR y sincronizacion con cuentas bancarias/billeteras virtuales, con clasificacion editable en categorias (gastos fijos, variables, hormiga, etc.). |
| Gestion de Ahorro y Descuentos Inteligentes | Listas de compras inteligentes, motor de promociones (bancos/tarjetas/comercios) actualizado periodicamente, y notificaciones geolocalizadas de descuentos aplicables segun habitos y medios de pago del usuario. |
| Gestion de Inteligencia de Negocios | Agentes de IA que analizan balances y perfiles para generar diagnosticos y sugerencias de ahorro/presupuesto/inversion, chatbot financiero conversacional, y dashboard analitico con KPIs (balance mensual, promedio historico, tasa de ahorro, distribucion por categoria). |

#### Gestiones tecnicas / transversales

| Gestion | Estado actual | Codigo TP |
| --- | --- | --- |
| Arquitectura base y persistencia | Implementado | T01 |
| Login / Logout y gestion de usuarios | Implementado | T02 |
| Encriptado y hash de datos sensibles | Implementado | T03 |
| Perfiles y permisos de usuario (Composite) | Implementado | T04 |
| Multiples idiomas (Observer) | Implementado | T05 |
| Bitacora y control de cambios | Implementado | T06 |
| Digitos verificadores e integridad | Implementado | T07 |
| Backup y restauracion de la base de datos | Implementado | T08 |

Estas gestiones tecnicas son las que hoy sostienen al sistema (auditoria, seguridad,
localizacion, integridad de datos) y se mantendran sin cambios de fondo cuando se incorpore
el dominio de negocio de finanzas personales sobre ellas.

### Alcance

El alcance determina nitidamente hasta donde llega la solucion ofrecida por FINAZAPP como
producto terminado.

#### Dentro del alcance

**A. Modulo de Usuarios, Seguridad y Roles**
- Rol Usuario Final: gestion de su perfil, parametrizacion de cuentas financieras, registro
  de gastos/ingresos, consulta de dashboards y uso del asistente de IA.
- Rol Administrador / Operador: gestion de accesos y estados de cuentas, ejecucion de
  backups, monitorizacion y remediacion manual de conectores de sincronizacion o scrapers
  caidos.
- Seguridad de datos: almacenamiento cifrado en reposo y en transito (AES-256 / TLS) de
  credenciales y tokens de acceso a plataformas bancarias.

**B. Modulo de Registro y Conciliacion de Movimientos**
- Registro manual clasificado por categorias y etiquetas.
- Digitalizacion y procesamiento de tickets/recibos fisicos mediante captura de imagen y
  extraccion OCR de montos, fechas y conceptos.
- Sincronizacion automatica de movimientos mediante APIs bancarias y/o modulos de
  integracion automatizada.
- Catalogacion editable de ingresos y egresos.

**C. Modulo de Descuentos, Compras Inteligentes y Geolocalizacion**
- Lista de compras inteligente con seguimiento de items y recomendaciones de mejor
  dia/opcion de compra.
- Motor de promociones con sincronizacion periodica (configurable) de descuentos de bancos,
  tarjetas y comercios via APIs publicas y scraping.
- Notificaciones por ubicacion cuando el usuario esta cerca de comercios con promociones
  activas para sus medios de pago.

**D. Modulo de Inteligencia Artificial y Asesoria Financiera**
- Gestion centralizada de IA: la plataforma consume los servicios de LLM mediante una API
  key unificada a nivel backend; el usuario no configura claves personales.
- Agentes programados que generan diagnosticos periodicos de balance, alertas de desvio
  presupuestario y sugerencias orientativas de ahorro, retiro o inversion segun el perfil
  declarado.
- Chatbot financiero para consultas sobre el estado de las finanzas y sugerencias de
  distribucion de presupuesto.

**E. Modulo de Analitica y Reporteria**
- Dashboard visual con KPIs: balance mensual, promedio acumulado, tasa de ahorro y
  distribucion porcentual de gasto por categoria.

#### Fuera de alcance

- **Ejecucion transaccional de inversiones (brokerage)**: FINAZAPP no ejecuta ordenes de
  compra/venta de activos, acciones, bonos ni fondos comunes de inversion. Su funcion es
  100% analitica y orientativa; el usuario ejecuta las operaciones por su cuenta en su
  broker o banco.
- **Gestion empresarial y control de stock (B2B)**: el sistema no esta pensado para
  empresas, no soporta puntos de venta (POS) ni gestion de inventario de sucursales
  comerciales.
- **Finanzas compartidas / grupos familiares**: el modelo es de cuenta uninominal por
  individuo. Cuentas multiusuario o balances familiares consolidados quedan como trabajo
  futuro, fuera de este producto.
- **Manejo de API keys de IA por parte del usuario**: la infraestructura de backend absorbe
  el costo y la gestion de las APIs de IA; el usuario no gestiona credenciales propias de
  IA.

### Aclaraciones operativas sobre el alcance

| Consulta planteada | Resolucion tecnica / definicion de negocio |
| --- | --- |
| Rol de los operadores | Soporte de nivel 2 y 3: gestion de usuarios, auditoria de logs, backups y mantenimiento de conectores/scrapers cuando bancos o comercios modifican sus interfaces. |
| Complejidad financiera del usuario | No se requieren conocimientos tecnicos ni financieros; el sistema categoriza automaticamente y genera recomendaciones en lenguaje natural simple. |
| Conexiones bancarias heterogeneas | Arquitectura desacoplada: soporte via APIs oficiales cuando existan, o modulos de extraccion controlada con credenciales cifradas cuando no. |
| Actualizacion de descuentos | Procesos periodicos en segundo plano que actualizan la base de promociones sin bloquear las consultas del usuario. |
| Modelo de integracion de IA | Arquitectura backend-as-a-service: procesamiento con agentes especializados en el servidor, entrega de resultados a la interfaz del usuario. |

## G03. Planificacion del proyecto

Plan de trabajo por iteracion, alineado con el "Plan de entregas TD 2023" de la catedra y con
las 3 entregas formales obligatorias del cuatrimestre. La iteracion 1 (Clase 1, 11/08) ya se
cerro con la propuesta de proyecto, antes del inicio de las 3 entregas trackeadas; las
iteraciones 2 a 4 corresponden a la 1ra, 2da y 3ra entrega respectivamente. Este espacio se
actualiza en cada sesion de trabajo para reflejar el nivel de avance real.

| Iteracion | Cierre (clase / fecha) | Alcance exigido por la catedra | Estado a esta fecha (29/08/2026) |
| --- | --- | --- | --- |
| 1 | Clase 1 — 11/08 | Propuesta de proyecto (base para las entregas posteriores). | **Cerrada.** |
| 2 (1ra entrega) | Clase 5 — 08/09 | G01 Proposito · G02 Descripcion funcional y alcance · G03 Planificacion del proyecto · G04 Definiciones/Acronimos/Abreviaturas · G05 Roles · G06 Otros requisitos · N01 Proceso de negocio: analisis y diseno (roles intervinientes, descripcion funcional, diagrama de proceso, modelo conceptual, CU principales, diagrama de clases, diagramas de secuencia, modelos de datos) — aplica **a cada proceso de negocio del proyecto** (NEG01-movimientos, NEG02-categorias), no a un proceso generico unico. | **G01-G06 redactados** (este documento). **N01 analisis y diseno de NEG01-movimientos y NEG02-categorias: pendiente de documentar** — el codigo de negocio (movimientos y categorias) ya esta implementado, pero sus diagramas y especificacion de CU todavia no se generaron. |
| 3 (2da entrega) | Clase 11 — 20/10 | Continuacion de N01 (diagrama de clases y modelo de datos de cada proceso de negocio) · A01 Instalador, version inicial. | Pendiente. |
| 4 (3ra entrega, final) | Clase 15 — 17/11 | N02 Proceso de negocio: implementacion y pruebas — aplica **a cada proceso de negocio del proyecto** (NEG01-movimientos, NEG02-categorias), no a un proceso generico unico — · D01 Manual de instalacion · D02 Ayuda en linea · D03 Material de apoyo al usuario final · A01 Instalador, version final · A02 Informe/exportacion a PDF · A02 Serializacion · G07 Diagrama de clases completo · G08 Modelo de datos completo. | Pendiente. El esqueleto tecnico transversal (T01-T08) del que depende gran parte de este cierre ya esta implementado. |

## G04. Definiciones, Acronimos y Abreviaturas

### Definiciones

- **FINAZAPP**: nombre del sistema de gestion de finanzas personales (Personal Finance
  Management) descripto en este documento.
- **PFM (Personal Finance Management)**: categoria de software dedicada a centralizar y
  gestionar las finanzas de una persona individual.
- **Movimiento**: registro individual de un ingreso o egreso de dinero del usuario,
  clasificado por categoria y medio de pago.
- **Categoria**: clasificacion editable de un movimiento (por ejemplo: gasto fijo, gasto
  variable, gasto hormiga, ingreso por sueldo, ingreso freelance).
- **Presupuesto**: limite planificado de gasto para una categoria en un periodo determinado.
- **Ticket**: comprobante fisico o digital de una compra, sujeto a digitalizacion por OCR.
- **OCR (Optical Character Recognition)**: tecnica de extraccion automatica de texto (monto,
  fecha, concepto) a partir de la imagen de un ticket.
- **Scraping**: tecnica de recoleccion automatizada de informacion publicada en sitios web
  de terceros (por ejemplo, promociones bancarias o comerciales).
- **Geolocalizacion**: determinacion de la posicion fisica del usuario, utilizada para
  notificar promociones de comercios cercanos.
- **Motor de Promociones**: subsistema que centraliza, estructura y actualiza
  periodicamente las promociones de bancos, tarjetas y comercios.
- **Agente de IA**: proceso automatizado que consume un modelo de lenguaje (LLM) para
  analizar datos del usuario y generar diagnosticos o sugerencias sin intervencion humana.
- **Chatbot Financiero**: asistente conversacional que responde consultas del usuario sobre
  su situacion financiera.
- **Backend-as-a-Service (para IA)**: modelo en el que el servidor centraliza el consumo de
  servicios de IA con una unica credencial, sin exponer configuracion al usuario final.
- **Bitacora**: registro de auditoria de las operaciones realizadas por los usuarios del
  sistema (fecha, hora, usuario, actividad, informacion asociada).
- **Permiso atomico / compuesto**: permiso que representa una unica funcionalidad (atomico)
  o que agrupa un conjunto de permisos atomicos u otros compuestos (compuesto), segun el
  patron Composite.
- **Digito verificador**: valor de control calculado sobre un dato o registro, usado para
  detectar alteraciones no autorizadas de la base de datos.

### Acronimos

- **PFM**: Personal Finance Management.
- **OCR**: Optical Character Recognition.
- **LLM**: Large Language Model.
- **KPI**: Key Performance Indicator.
- **DER**: Diagrama de Entidad-Relacion.
- **UML**: Unified Modeling Language.
- **MDI**: Multiple Document Interface.
- **POO**: Programacion Orientada a Objetos.
- **TDD**: Test-Driven Development.
- **AES-256**: Advanced Encryption Standard, clave de 256 bits.
- **TLS**: Transport Layer Security.
- **API**: Application Programming Interface.

### Abreviaturas

- **ARG**: Argentina.
- **TP**: Trabajo Practico (Trabajo de Campo / Trabajo de Diploma).

## G05. Descripcion de las personas participantes en el desarrollo y los usuarios (Roles)

### Personas participantes en el desarrollo del proyecto

| Nombre | Descripcion | Responsabilidad |
| --- | --- | --- |
| Agustin Gutierrez | Alumno a cargo integral del proyecto FINAZAPP (Trabajo de Campo / Trabajo de Diploma). | Releva requerimientos, define el alcance, disena la arquitectura, implementa el sistema, documenta los artefactos (G0X/N0X/T0X/A0X/D0X) y ejecuta las pruebas. Cumple, para este TP individual, los roles que en un equipo real se repartirian entre analista funcional, arquitecto, desarrollador y tester. |
| Docente de catedra (Trabajo de Campo / Trabajo de Diploma) | Interesado academico del proyecto. | Define las pautas de entrega segun la Carpeta Modelo, evalua cada iteracion y aprueba el avance del proyecto. |

### Roles de usuario del sistema

Los roles de sistema se implementan mediante permisos compuestos (patron Composite,
T04), lo que permite asignarlos y ajustarlos dinamicamente sin cambiar el modelo de datos.
La siguiente tabla resume los roles previstos para el producto completo:

| Rol | Descripcion | Acceso |
| --- | --- | --- |
| Usuario Final | Persona que utiliza FINAZAPP para gestionar su economia personal. | Gestion de su propio perfil y preferencias; parametrizacion de sus cuentas financieras; registro y consulta de sus movimientos (manual, OCR o sincronizados); listas de compras y notificaciones de promociones geolocalizadas; dashboard analitico de su propia informacion; interaccion con el chatbot y las sugerencias de IA. |
| Administrador / Operador | Personal de soporte tecnico y operativo de la plataforma. | Gestion de usuarios (altas, bajas, estados); gestion de perfiles y permisos; consulta de bitacora y control de cambios; monitoreo y remediacion manual de conectores de sincronizacion bancaria y scrapers de promociones; ejecucion y verificacion de backups; administracion de idiomas disponibles; supervision de la verificacion de integridad de la base de datos. |

## G06. Otros Requisitos

### Del producto

**Estandares aplicables**
- Diseño y documentacion segun UML2, con todos los diagramas modelados en PlantUML y
  versionados como codigo fuente en el repositorio.
- Arquitectura en capas (minimo cuatro; este proyecto usa siete: UI, BLL, REPOSITORIO, DAL,
  BE, SERVICIOS, ABSTRACCIONES) respetando cohesion, acoplamiento controlado y reuso segun
  los criterios de la Programacion Orientada a Objetos.
- Persistencia sobre base de datos relacional normalizada (Tercera Forma Normal) con
  integridad referencial, sin frameworks de persistencia de terceros.

**Requisitos de sistema**
- Aplicacion de escritorio (.NET Framework, WinForms) para la gestion tecnica y de negocio
  del usuario y del administrador.
- Persistencia sobre SQL Server.
- Cifrado en reposo y en transito (AES-256 / TLS) para credenciales y tokens de acceso a
  servicios bancarios externos.

**Requisitos de desempeño**
- Las actualizaciones del motor de promociones y de los procesos de IA se ejecutan en
  procesos periodicos en segundo plano (frecuencia configurable), sin bloquear las
  consultas interactivas del usuario.

**Requisitos de entorno**
- Conexiones a entidades bancarias heterogeneas mediante una arquitectura desacoplada:
  APIs oficiales cuando esten disponibles, o modulos de extraccion controlada con
  credenciales cifradas cuando no.
- Disponibilidad de un motor de base de datos SQL Server accesible desde el entorno de
  ejecucion del sistema.

### Estandares normativos y marco regulatorio

- **Registro y consentimiento (Ley 25.326)**: la base de usuarios se registra ante la AAIP
  y el alta pide consentimiento explicito no premarcado; el sistema expone borrado real de
  cuenta y datos (derechos ARCO), porque FINAZAPP maneja datos patrimoniales y de
  geolocalizacion alcanzados por la ley de proteccion de datos personales.
- **IA explicable y minimizada (Disposicion AAIP 2/2023)**: las sugerencias del asistente
  muestran los parametros que las originan, y a los LLM solo se les envian datos
  anonimizados o seudonimizados, porque la norma exige explicabilidad y minimizacion de
  datos en cualquier procesamiento con IA sobre informacion personal.
- **Conexion bancaria de solo lectura (Ley 26.388)**: la sincronizacion con cuentas es
  read-only y nunca persiste 2FA ni claves de transferencia, porque la ley tipifica el
  acceso no autorizado a sistemas informaticos y los propios bancos prohiben compartir
  credenciales con terceros en sus terminos y condiciones.
- **Cifrado y trazabilidad (BCRA "A" 7724)**: credenciales cifradas en reposo con AES-256
  (T03), bitacora inmutable (T06) y permisos segregados por rol (T04), siguiendo las
  practicas de ciberresiliencia del BCRA aunque FINAZAPP no sea una entidad regulada por
  el organismo.
- **Sin ejecucion de ordenes (Ley 26.831)**: las recomendaciones de ahorro/inversion son
  orientativas y algoritmicas, con disclaimer explicito de que no hay asesoramiento
  personalizado, porque la intermediacion de inversion esta reservada a agentes registrados
  en la CNV y FINAZAPP no lo es.
- **Sin custodia de fondos (Ley 21.526)**: el sistema no capta depositos ni administra
  dinero de terceros, quedando asi fuera de la licencia bancaria que la ley exige a las
  entidades financieras.
- **Terminos claros y baja accesible (Ley 24.240)**: precios, alcance y limites de las
  recomendaciones se muestran sin letra chica, con boton de baja y de arrepentimiento
  visibles si el modelo de negocio incluye suscripcion paga, por exigencia de la ley de
  defensa del consumidor.
- **Scraping respetuoso (Ley 11.723)**: el motor de descuentos solo guarda datos facticos
  (comercio, descuento, vigencia) respetando `robots.txt`, sin reproducir contenido de
  terceros, por los limites que impone la ley de propiedad intelectual.

### De documentacion

Estos documentos son entregables planificados para etapas posteriores del proyecto (grupo
D0X de la Carpeta Modelo); se listan aqui como requisito porque forman parte del alcance
total del producto, aunque su redaccion todavia no se haya iniciado:

- **Manual de Usuario**: guia de uso de las funcionalidades del sistema orientada al
  Usuario Final.
- **Ayuda en linea**: mecanismo de ayuda contextual dentro de la aplicacion para las
  funcionalidades mas relevantes.
- **Guias de Instalacion, Configuracion y fichero Leame**: instrucciones para instalar,
  configurar y poner en marcha el sistema completo (incluyendo motor de base de datos y
  dependencias de terceros).
