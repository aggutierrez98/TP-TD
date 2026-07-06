# Comandos de test para el ciclo TDD

## Comando canonico (usar siempre para rojo y verde)

Unit (SERVICIOS.Tests, siempre corre completo):

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1
```

El runner compila `tests/TpIDS.Tests.sln`, valida contra el TRX que se ejecuto al menos 1 test y que no hubo fallos, y devuelve exit code 0 solo si todo esta en verde.

## Opcional: un test individual durante un rojo largo

Solo para iterar rapido sobre un unico test. Requiere haber compilado antes `tests/TpIDS.Tests.sln`. Usa los DLL exactos, igual que el runner:

```powershell
& "<ruta a vstest.console.exe>" `
    "tests\SERVICIOS.Tests\bin\Debug\net472\SERVICIOS.Tests.dll" `
    /TestCaseFilter:"FullyQualifiedName~<Clase>.<Metodo>"
```

Advertencias:

- No tiene el guard de "0 tests ejecutados" del runner: verificar en el output que el test efectivamente corrio (un filtro con typo reporta verde vacio).
- No reemplaza la corrida completa del runner antes de declarar el verde final.

## Criterio de aceptacion

- **Rojo** = falla solo el test nuevo, preferentemente por assertion. Un error de compilacion solo vale como rojo si el test expresa un seam deliberadamente inexistente (en ese caso el runner corta en el build y no deja TRX).
- **Verde** = exit code 0 del runner, con contadores reales leidos del TRX en `TestResults/`.
