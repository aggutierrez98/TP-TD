# Build Command

## Primary command

Usar este comando desde la raiz del repo:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' TpIDS.sln /t:Build /p:Configuration=Debug /m
```

## Fallback discovery

Si esa ruta no existe, buscar `MSBuild.exe` en:

- `C:\Program Files\Microsoft Visual Studio\*\*\MSBuild\Current\Bin\MSBuild.exe`
- `C:\Program Files (x86)\Microsoft Visual Studio\*\*\MSBuild\Current\Bin\MSBuild.exe`

## Acceptance

- La solucion debe compilar con `0 errores`.
- No cerrar la tarea con errores de compilacion pendientes.
- Si aparecen warnings nuevos relevantes, mencionarlos en la respuesta final.
