# Stack .NET WinForms (v4.7.2)

1. **Framework**: El proyecto estrictamente usa .NET Framework 4.7.2. NO uses APIs, bibliotecas o sintaxis exclusivas de .NET Core, .NET 5+ u otras versiones modernas que no sean soportadas por C# 7.3.
2. **UI**: La interfaz de usuario es **WinForms**. No sugerir WPF, MAUI ni web. Todo el código de UI debe integrarse dentro de Formularios de Windows.
3. **Persistencia**: Preferir ADO.NET clásico, evitando Entity Framework Core a menos que se trate del Entity Framework 6 compatible con .NET Framework.
