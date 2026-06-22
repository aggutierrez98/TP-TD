# TpIDS PlantUML Environment

Usar esta referencia cuando la tarea de PlantUML ocurra dentro del repositorio TpIDS (resolver rutas desde la raiz del repo, no desde una ruta absoluta de una maquina puntual).

## Runtime disponible

- Windows 11: `plantuml`, `java`, `dot` y `python` quedaron instalados y accesibles desde una terminal nueva.
- WSL Ubuntu: `plantuml`, `java` y `dot` tambien quedaron disponibles mediante wrappers de usuario.
- Variable de entorno esperada: `PLANTUML_JAR` apuntando al `plantuml.jar` instalado en cada maquina (ej. `%USERPROFILE%\plantuml.jar` en Windows y `$HOME/plantuml.jar` en WSL). Ajustar segun donde quede instalado.

## Convenciones del repo

- Guardar el codigo fuente `.puml` bajo `diagramas/<requisito>/`.
- Usar las carpetas existentes por requisito: `T02-login-logout`, `T03-encriptado`, `T04-perfiles-usuario`, `T05-multidioma`, `T06a-bitacora`, `T06b-control-cambios`, `T07-digitos-verificadores`.
- El nombre del archivo debe expresar requisito y tipo de diagrama. Preferir nombres ordenables y descriptivos.
- El `.puml` es la fuente de verdad. Las imagenes derivadas son artefactos secundarios.

## Flujo recomendado en TpIDS

1. Confirmar que el usuario realmente quiere diagramar o editar diagramas.
2. Identificar el requisito tecnico del TP impactado.
3. Crear o editar el `.puml` bajo `diagramas/<requisito>/`.
4. Si el cambio tambien impacta documentacion formal, volver a la skill central [`../../../../tp-uml-y-casos-de-uso/SKILL.md`](../../../../tp-uml-y-casos-de-uso/SKILL.md).
5. Mantener el diagrama alineado con nombres reales de clases, metodos, tablas y relaciones del repo.

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
