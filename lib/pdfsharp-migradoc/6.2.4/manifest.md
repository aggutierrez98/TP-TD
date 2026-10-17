# Dependencias PDF distribuidas

DLL oficiales sin modificar, extraidas una vez para el desarrollo. La compilacion y la ejecucion del componente PDF no descargan paquetes ni requieren cache NuGet. .NET Framework 4.7.2 / Windows. Licencia MIT; se incluyen los textos de licencia y nuspec con procedencia y dependencias.

## Versiones y origen

Los paquetes se obtuvieron de `https://api.nuget.org/v3-flatcontainer/<id-minusculas>/<version>/<id-minusculas>.<version>.nupkg`.

| Paquete | Version | Activos |
| --- | --- | --- |
| PDFsharp-MigraDoc-GDI | 6.2.4 | net462 |
| PDFsharp-GDI | 6.2.4 | net462 |
| Microsoft.Extensions.Logging.Abstractions | 8.0.3 | net462 |
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.2 | net462 |
| System.Security.Cryptography.Pkcs | 8.0.1 | net462 |
| Microsoft.Bcl.AsyncInterfaces | 8.0.0 | net462 |
| System.Buffers | 4.5.1 | net461 |
| System.Memory | 4.5.5 | net461 |
| System.Threading.Tasks.Extensions | 4.5.4 | net461 |
| System.Numerics.Vectors | 4.5.0 | net46 |
| System.Runtime.CompilerServices.Unsafe | 4.5.3 | net461 |
| System.ValueTuple | 4.6.1 | net47 (fachada hacia mscorlib) |

`System.ValueTuple` incluye la licencia MIT del repositorio oficial
`dotnet/maintenance-packages`, commit `6b84308c9ad012f53240d72c1d716d7e42546483`
(`LICENSE`), porque el paquete declara la expresión MIT pero no contiene un archivo
de licencia. La fachada net47 conserva los tipos de tupla del framework y evita
redirecciones hacia una versión no distribuida de la fachada.

El grafo se resolvio siguiendo los grupos .NETFramework de cada nuspec, no los de .NET moderno/netstandard. Pkcs net462 no declara System.Formats.Asn1: usa System.Security del framework. Se incluyen tambien las DLL auxiliares de los paquetes principales para reproducir su distribucion.

`SERVICIOS/PdfSharp.references.props` usa referencias directas. En la solucion existente, las utilidades compartidas de System.Memory/AsyncInterfaces y afines conservan las versiones superiores que ya selecciona `SystemTextJson.references.props`; en una compilacion aislada del PDF se utilizan las copias compatibles incluidas aqui. No se cambia la configuracion de JSON.

## SHA-256 de los ensamblados

| Archivo | SHA-256 |
| --- | --- |
| MigraDoc.DocumentObjectModel.dll | 5d166b173f7c1bca92d31b2005a711b3d51536c9c456f5d85eccd0777de31944 |
| MigraDoc.Rendering-gdi.dll | ec68b0932598f0f82776904114fdcb3f43553a0daaf876e46ee87c93e9a7727e |
| MigraDoc.RtfRendering-gdi.dll | 1d75f7c8cdc99c663529ea5970e6b393fd8496aaaf2c7926f25512b6cc497f07 |
| PdfSharp.BarCodes-gdi.dll | 622f6b267236333ea9a0c74f46e43cec57bf475a6d7ef4ec5c248ecc5b23a79a |
| PdfSharp-gdi.dll | b0649ec5088c165fdb14b1ae1b013881f978c415cc752c52e3692a7560bb8ad6 |
| PdfSharp.System.dll | 361b75ae4da87cd63f4b451bbc2239770d9175d9cfaa466c41a6fb495991f221 |
| PdfSharp.Charting-gdi.dll | dec047eaf439382c4991fd5bef2c443f4ff356abf06615fbaa95f3eb8735e4e8 |
| PdfSharp.Quality-gdi.dll | 4788e55493fc48d61b011b7990600f5ddd8abc109df358225482c5ebbab78f06 |
| PdfSharp.WPFonts.dll | 5db891b3ae62ae87deda3e037b504336316003e4e5c4dc9574311da64824d712 |
| PdfSharp.Cryptography.dll | 1a69f4067bb551a38b8b03821c8d79625dc72aedb9106ecd368f18b87486143a |
| PdfSharp.Shared.dll | ad30af67d13f532372a350cd83b1c64d04fd8aad64c4695caaf2e4322609f235 |
| PdfSharp.Snippets-gdi.dll | d9e6bb73d641ea9a2afa58221223c39c9bf7ac3fb0e993d13aa752550aac4e3b |
| Microsoft.Extensions.Logging.Abstractions.dll | b4e0e778afdc76403792390b056f68de0ced742d16c5b0347d9cca6930f4761b |
| Microsoft.Extensions.DependencyInjection.Abstractions.dll | 19edd4f5e69a4589dc314909b5854a22f89c4b9ab040591771397b2cb1a17ba6 |
| System.Security.Cryptography.Pkcs.dll | 89436836e39a6bb4275613ce45226ffa10e590151296343560221441d7a79670 |
| Microsoft.Bcl.AsyncInterfaces.dll | 80678203bd0203a6594f4e330b22543c0de5059382bb1c9334b7868b8f31b1bc |
| System.Buffers.dll | accccfbe45d9f08ffeed9916e37b33e98c65be012cfff6e7fa7b67210ce1fefb |
| System.Memory.dll | bf3fb84664f4097f1a8a9bc71a51dcf8cf1a905d4080a4d290da1730866e856f |
| System.Threading.Tasks.Extensions.dll | 4f81ffd0dc7204db75afc35ea4291769b07c440592f28894260eea76626a23c6 |
| System.Numerics.Vectors.dll | 1d3ef8698281e7cf7371d1554afef5872b39f96c26da772210a33da041ba1183 |
| System.Runtime.CompilerServices.Unsafe.dll | 66409f670315afe8610f17a4d3a1ee52d72b6a46c544cec97544e8385f90ad74 |
| System.ValueTuple.dll | f47879d6525c835aaef8685647b5833c3253bef7fbe6ae48ace0921e6186252c |

## Alcance de portabilidad

La comprobacion del componente PDF no acredita portabilidad integral de TpIDS.sln. Las dependencias previas de UI (GMap, Krypton, LiveCharts y nativos/transitivos) tienen seguimiento en la tarea 5.2 de `openspec/changes/migrar-json-system-text-sin-nuget/tasks.md`. El laboratorio completo sigue requiriendo esa resolucion y una compilacion integral limpia.
