# TpIDS PlantUML Environment

Usar esta referencia cuando la tarea de PlantUML ocurra dentro del repositorio TpIDS (resolver rutas desde la raiz del repo, no desde una ruta absoluta de una maquina puntual).

## Runtime disponible

- Windows 11: `plantuml`, `java`, `dot` y `python` quedaron instalados y accesibles desde una terminal nueva.
- WSL Ubuntu: `plantuml`, `java` y `dot` tambien quedaron disponibles mediante wrappers de usuario.
- Variable de entorno esperada: `PLANTUML_JAR` apuntando al `plantuml.jar` instalado en cada maquina (ej. `%USERPROFILE%\plantuml.jar` en Windows y `$HOME/plantuml.jar` en WSL). Ajustar segun donde quede instalado.

## Convenciones del repo

- Guardar el codigo fuente `.puml` bajo `diagramas/<requisito>/`.
- Usar las carpetas existentes por requisito: `T02-login-logout`, `T03-encriptado`, `T04-perfiles-usuario`, `T05-multidioma`, `T06-bitacora-control-cambios`, `T07-digitos-verificadores`.
- El nombre del archivo debe expresar requisito y tipo de diagrama. Preferir nombres ordenables y descriptivos.
- El `.puml` es la fuente de verdad. Las imagenes derivadas son artefactos secundarios.

## Flujo recomendado en TpIDS

1. Confirmar que el usuario realmente quiere diagramar o editar diagramas.
2. Identificar el requisito tecnico del TP impactado.
3. Crear o editar el `.puml` bajo `diagramas/<requisito>/`.
4. Renderizar y validar la proporcion ancho:alto (ver seccion siguiente) antes de dar el diagrama por terminado.
5. Si el cambio tambien impacta documentacion formal, volver a la skill central [`../../../../tp-uml-y-casos-de-uso/SKILL.md`](../../../../tp-uml-y-casos-de-uso/SKILL.md).
6. Mantener el diagrama alineado con nombres reales de clases, metodos, tablas y relaciones del repo.

## Validacion de proporcion (ancho:alto)

Las entregas de este repo se pegan como imagen en un documento Word (hoja
portrait). Un diagrama de clases con muchas entidades sin relaciones entre
si hace que Graphviz (el motor de layout de PlantUML) las alinee en una
sola fila, generando imagenes extremadamente anchas y bajas -- ilegibles
al pegarlas a ancho de pagina.

**Rango objetivo**: relacion ancho:alto entre 1:1 y 2.5:1. Fuera de ese
rango el diagrama no esta apto para entrega y hay que corregirlo antes de
darlo por terminado.

### Paso de validacion (obligatorio despues de generar o editar un `.puml`)

1. Renderizar a SVG:
   `python3 .agents/skills/tp-delivery-standards/embedded-skills/plantuml/scripts/convert_puml.py <archivo.puml> --format svg`
2. Leer el ancho y alto reales del header del SVG generado:
   `head -c 400 <archivo.svg> | grep -o 'style="width:[0-9]*px;height:[0-9]*px'`
3. Calcular `ancho / alto`. Si el resultado es <= 2.5, el diagrama pasa la
   validacion y no hace falta tocar nada mas.
4. Si supera 2.5 (o, en el otro extremo, queda por debajo de 1 y termina
   demasiado angosto y alto), aplicar la iteracion de correccion de abajo
   y volver al paso 1. Repetir hasta 3-4 iteraciones; si despues de eso
   sigue fuera de rango, reportarlo al usuario en vez de seguir iterando a
   ciegas -- puede ser sintoma de que el diagrama necesita dividirse en
   mas de un archivo.

### Iteracion de correccion (probar en este orden)

1. **Fusionar pares interfaz+implementacion triviales.** Si una interfaz
   de ABSTRACCIONES la implementa una unica clase y ademas nadie la
   referencia de forma polimorfica dentro de ese mismo diagrama (no es el
   tipo de una coleccion, ni de un campo/parametro compartido por mas de
   un implementador), no dibujarla como caja aparte: mostrarla como
   estereotipo extra en la clase que la implementa
   (`class Sesion <<singleton>> <<ISesion>>`) y borrar la flecha de
   realizacion. Reduce nodos y aristas sin perder informacion real -- es
   la primera tecnica a probar, antes de tocar el layout.
   Mantener la interfaz como caja aparte cuando: la implementan 2+
   clases, participa de un patron estructural (ej. Composite:
   `PermisoGrupo o--> "*" IPermiso`), o la referencian por tipo otras
   clases del mismo diagrama (ej. varias clases BLL dependiendo de la
   misma interfaz de contrato).
2. **Grilla con enlaces ocultos.** Para las clases sin relaciones fuertes
   entre si que sigan quedando en una sola fila:
   - Agruparlas en filas de ~3-4 elementos.
   - Encadenar cada fila con `ClaseA -[hidden]right-> ClaseB` entre
     elementos consecutivos.
   - Encadenar el primer elemento de cada fila con el primero de la fila
     siguiente usando `-[hidden]down->` para apilarlas verticalmente.
   - Marcar el bloque agregado con un comentario del estilo
     `' --- Quiebres de layout (edges ocultos, no representan relaciones reales) ---`
     para que quede claro que no son relaciones de dominio.
   - Menos columnas por fila = diagrama mas angosto y alto (baja el
     ratio); mas columnas = mas ancho y bajo (sube el ratio). Ajustar el
     numero de columnas segun el ratio medido y volver a renderizar.
3. **Sacar informacion no esencial (ultimo recurso).** Solo si los dos
   pasos anteriores no alcanzan: quitar atributos/metodos secundarios de
   una clase, o dividir el diagrama en mas de un archivo por subgrupo
   tematico. Si se hace, dejarlo explicito en la respuesta al usuario --
   que informacion se saco y por que -- nunca en silencio.

Ejemplo real aplicado en `diagramas/G07-clases-por-capa/`:
`G07-clases-servicios.puml` paso de 6727x418px (16:1) a 1251x938px (1.3:1)
fusionando interfaces triviales; `G07-clases-bll.puml` paso de 4614x972px
(4.7:1) a 2164x1278px (1.7:1) con grilla de enlaces ocultos (ahi no se
podia fusionar: las 6 interfaces de "Contratos internos" las usan varias
clases BLL a la vez, no son 1:1).

## Comandos utiles

### Windows PowerShell

```powershell
plantuml -version
python .agents/skills/tp-delivery-standards/embedded-skills/plantuml/scripts/check_setup.py
python .agents/skills/tp-delivery-standards/embedded-skills/plantuml/scripts/convert_puml.py .\diagramas\T05-multidioma\archivo.puml --format svg
```

### WSL

```bash
plantuml -version
python3 .agents/skills/tp-delivery-standards/embedded-skills/plantuml/scripts/check_setup.py
python3 .agents/skills/tp-delivery-standards/embedded-skills/plantuml/scripts/convert_puml.py ./diagramas/T05-multidioma/archivo.puml --format svg
```
