# Formato de especificacion de casos de uso

Toda especificacion de caso de uso (CU) del TP usa exactamente estos campos, en este
orden. El script `scripts/build-use-case-spec.py` los lee desde un JSON y los renderiza
en una tabla de una sola columna por CU (Word) mas un espejo Markdown.

## Campos

| Campo (JSON) | Etiqueta visible | Obligatorio | Tipo | Con que se completa |
| --- | --- | --- | --- | --- |
| `id_nombre` | ID y Nombre | Si | texto | Codigo unico + nombre con verbo en infinitivo. Global (`CU01 Login`) o ligado a un requisito (`CU-T05-01 Agregar idioma`). |
| `estado` | Estado | Si | texto | Madurez del CU: `Implementado`, `Definido` o `Propuesto`. Otros valores generan un aviso, no un error. |
| `descripcion` | Descripcion | Si | texto | Meta del actor en 1-2 frases (que logra, no como). |
| `actor_principal` | Actor Principal | Si | texto | Quien inicia/dispara el CU (rol o usuario). |
| `actor_secundario` | Actor Secundario | Si | texto | Actores o sistemas de apoyo. Usar `-` si no hay. En el TP suele listar componentes internos colaboradores (BLL/DAL/Service). |
| `precondiciones` | Precondiciones | Si | texto | Lo que debe cumplirse o existir antes de iniciar el CU. |
| `puntos_extension` | Puntos de Extension | Si | texto | Relacion con otros CU (include/extend) o condicion que dispara un flujo extendido/refresco. Usar `No aplica.` si no hay. |
| `condicion` | Condicion | Si | texto | El disparador (trigger): evento o motivacion que inicia el CU. |
| `escenario_principal` | Escenario Principal | Si | lista | Pasos numerados del flujo de exito (actor <-> sistema). Cada item incluye su propio numero, ej. `"1. El usuario..."`. |
| `flujos_alternativos` | Flujos Alternativos | No | lista | Variantes y excepciones numeradas (`"1a. ..."` o `"A1. ..."`). Si se omite o queda vacio, el documento muestra "Sin flujos alternativos relevantes." |
| `postcondiciones` | Postcondiciones | Si | texto | Estado final del sistema tras finalizar con exito. |

## Convenciones de contenido

- **ID**: prefijo `CU` + numero. Para ligar a un requisito del TP usar `CU-<requisito>-NN`
  (ej. `CU-T05-03`). Mantener el numero correlativo dentro del requisito.
- **Nombre**: verbo en infinitivo + objeto (`Agregar idioma`, `Registrar usuario`).
- **Escenario principal**: una accion por paso, alternando actor y sistema, terminando en
  el objetivo cumplido. Numerar `1.`, `2.`, ...
- **Flujos alternativos / excepciones**: numerar enganchando al paso del escenario principal
  que varia (`4a.`, `5a.`) o como secuencia propia (`A1.`, `A2.`). Incluir aqui los flujos de
  excepcion/error (datos invalidos, no encontrado, excepcion tecnica).
- **Reglas de negocio**: cuando una regla aplica al CU (ej. "la contrasena debe tener al
  menos 8 caracteres"), incorporarla dentro de la precondicion, el paso de validacion del
  escenario principal o el flujo alternativo correspondiente.

## Esquema JSON

```json
{
  "requisito": "T05-multidioma",
  "titulo": "T05. Gestion de Multiples Idiomas",
  "subtitulo": "Especificaciones de casos de uso",
  "introduccion": "Texto opcional de contexto.",
  "casos_de_uso": [
    {
      "id_nombre": "CU-T05-01 Agregar idioma",
      "estado": "Implementado",
      "descripcion": "...",
      "actor_principal": "...",
      "actor_secundario": "-",
      "precondiciones": "...",
      "puntos_extension": "...",
      "condicion": "...",
      "escenario_principal": ["1. ...", "2. ..."],
      "flujos_alternativos": ["A1. ...", "A2. ..."],
      "postcondiciones": "..."
    }
  ]
}
```

- `requisito`, `titulo`, `subtitulo` e `introduccion` son metadatos del documento.
  `requisito` ademas define la ubicacion por defecto de las salidas
  (`diagramas/<requisito>/<requisito>-casos-de-uso.docx` y `.md`).
- `casos_de_uso` es obligatorio y no puede estar vacio.

Ver un ejemplo completo en `examples/ejemplo-casos-de-uso.json`.
