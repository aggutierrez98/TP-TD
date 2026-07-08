USE [master];
GO

IF DB_ID(N'TpCatedraIDS') IS NOT NULL
BEGIN
    ALTER DATABASE [TpCatedraIDS] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TpCatedraIDS];
END
GO

CREATE DATABASE [TpCatedraIDS];
GO

ALTER DATABASE [TpCatedraIDS] SET COMPATIBILITY_LEVEL = 130;
GO

USE [TpCatedraIDS];
GO

CREATE TABLE dbo.Usuarios
(
    Id INT IDENTITY(1,1) NOT NULL,
    Username VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(MAX) NOT NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
    DVH VARCHAR(100) NULL,
    CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Usuarios_Username UNIQUE (Username)
);
GO

-- T07: tabla de control de digitos verificadores verticales (DVV).
-- Almacena, por cada tabla bajo analisis, la firma consolidada de sus DVH.
CREATE TABLE dbo.DigitoVerificadorVertical
(
    NombreTabla VARCHAR(100) NOT NULL,
    Valor VARCHAR(100) NOT NULL,
    FechaActualizacion DATETIME NOT NULL,
    CONSTRAINT PK_DigitoVerificadorVertical PRIMARY KEY CLUSTERED (NombreTabla)
);
GO

CREATE TABLE dbo.Idioma
(
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    [Default] BIT NOT NULL CONSTRAINT DF_Idioma_Default DEFAULT (0),
    CONSTRAINT PK_Idioma PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Idioma_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE dbo.Palabra
(
    Id INT IDENTITY(1,1) NOT NULL,
    [Key] VARCHAR(150) NOT NULL,
    CONSTRAINT PK_Palabra PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Palabra_Key UNIQUE ([Key])
);
GO

CREATE TABLE dbo.Traduccion
(
    PalabraId INT NOT NULL,
    IdiomaId INT NOT NULL,
    Valor VARCHAR(300) NOT NULL,
    CONSTRAINT PK_Traduccion PRIMARY KEY CLUSTERED (PalabraId, IdiomaId),
    CONSTRAINT FK_Traduccion_Palabra FOREIGN KEY (PalabraId) REFERENCES dbo.Palabra(Id),
    CONSTRAINT FK_Traduccion_Idioma FOREIGN KEY (IdiomaId) REFERENCES dbo.Idioma(Id)
);
GO

CREATE TABLE dbo.UsuarioPreferencias
(
    UsuarioId INT NOT NULL,
    IdiomaId INT NOT NULL,
    CONSTRAINT PK_UsuarioPreferencias PRIMARY KEY CLUSTERED (UsuarioId),
    CONSTRAINT FK_UsuarioPreferencias_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT FK_UsuarioPreferencias_Idioma FOREIGN KEY (IdiomaId) REFERENCES dbo.Idioma(Id)
);
GO

CREATE TABLE dbo.Permiso
(
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    EsGrupo BIT NOT NULL,
    CONSTRAINT PK_Permiso PRIMARY KEY CLUSTERED (Id)
);
GO

ALTER TABLE dbo.Permiso
ADD CONSTRAINT UQ_Permiso_Nombre UNIQUE (Nombre);
GO

CREATE TABLE dbo.PermisoCompuesto
(
    PermisoPadreId INT NOT NULL,
    PermisoHijoId INT NOT NULL,
    CONSTRAINT PK_PermisoCompuesto PRIMARY KEY CLUSTERED (PermisoPadreId, PermisoHijoId),
    CONSTRAINT FK_PermisoCompuesto_Padre FOREIGN KEY (PermisoPadreId) REFERENCES dbo.Permiso(Id),
    CONSTRAINT FK_PermisoCompuesto_Hijo FOREIGN KEY (PermisoHijoId) REFERENCES dbo.Permiso(Id),
    CONSTRAINT CK_PermisoCompuesto_Distinto CHECK (PermisoPadreId <> PermisoHijoId)
);
GO

CREATE TABLE dbo.UsuarioPermiso
(
    UsuarioId INT NOT NULL,
    PermisoId INT NOT NULL,
    CONSTRAINT PK_UsuarioPermiso PRIMARY KEY CLUSTERED (UsuarioId, PermisoId),
    CONSTRAINT FK_UsuarioPermiso_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT FK_UsuarioPermiso_Permiso FOREIGN KEY (PermisoId) REFERENCES dbo.Permiso(Id)
);
GO

DECLARE @PermisoGestionUsuariosId INT;
DECLARE @PermisoGestionPermisosId INT;
DECLARE @PermisoVerBitacoraId INT;
DECLARE @PermisoHistorialUsuariosId INT;
DECLARE @PermisoConfiguracionUsuarioId INT;
DECLARE @PermisoGestionIdiomasId INT;
DECLARE @GrupoAdminId INT;
DECLARE @GrupoBasicoId INT;
DECLARE @AdminUserId INT;
DECLARE @UsuarioBasicoId INT;

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionusuarios', 0);
SET @PermisoGestionUsuariosId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionpermisos', 0);
SET @PermisoGestionPermisosId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('verbitacora', 0);
SET @PermisoVerBitacoraId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('historialusuarios', 0);
SET @PermisoHistorialUsuariosId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('configuracionusuario', 0);
SET @PermisoConfiguracionUsuarioId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionidiomas', 0);
SET @PermisoGestionIdiomasId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('admin', 1);
SET @GrupoAdminId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('basico', 1);
SET @GrupoBasicoId = SCOPE_IDENTITY();

INSERT INTO dbo.PermisoCompuesto (PermisoPadreId, PermisoHijoId)
VALUES
    (@GrupoAdminId, @PermisoGestionUsuariosId),
    (@GrupoAdminId, @PermisoGestionPermisosId),
    (@GrupoAdminId, @PermisoVerBitacoraId),
    (@GrupoAdminId, @PermisoHistorialUsuariosId),
    (@GrupoAdminId, @PermisoConfiguracionUsuarioId),
    (@GrupoAdminId, @PermisoGestionIdiomasId),
    (@GrupoBasicoId, @PermisoConfiguracionUsuarioId);

INSERT INTO dbo.Usuarios (Username, PasswordHash, Activo)
VALUES
    ('admin', 'jdLqOYiW5l0Rzxvs2F6C9s+XPP1lpDouX9r4gg8K72K00snMfMzAo84u8OAQP4Hc', 1),
    ('user', 'DDQdLlDvTDqmjKrOv8/nV7l2SmyoVkxqURLPHM1EG8fYYF61cjoTPjh45V3iiql9', 1);

SELECT @AdminUserId = Id FROM dbo.Usuarios WHERE Username = 'admin';
SELECT @UsuarioBasicoId = Id FROM dbo.Usuarios WHERE Username = 'user';
SELECT @GrupoAdminId = Id FROM dbo.Permiso WHERE Nombre = 'admin';
SELECT @GrupoBasicoId = Id FROM dbo.Permiso WHERE Nombre = 'basico';

INSERT INTO dbo.UsuarioPermiso (UsuarioId, PermisoId)
VALUES
    (@AdminUserId, @GrupoAdminId),
    (@UsuarioBasicoId, @GrupoBasicoId);
GO

CREATE TABLE dbo.Bitacora
(
    Id INT IDENTITY(1,1) NOT NULL,
    FechaHora DATETIME NOT NULL,
    UsuarioId INT NULL,
    UsernameSnapshot VARCHAR(50) NULL,
    Actividad VARCHAR(100) NOT NULL,
    Informacion VARCHAR(500) NULL,
    Nivel VARCHAR(20) NOT NULL,
    Origen VARCHAR(100) NOT NULL,
    EsTecnica BIT NOT NULL CONSTRAINT DF_Bitacora_EsTecnica DEFAULT (0),
    CONSTRAINT PK_Bitacora PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Bitacora_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id)
);
GO

CREATE INDEX IX_Bitacora_FechaHora
    ON dbo.Bitacora (FechaHora DESC);
GO

CREATE INDEX IX_Bitacora_UsuarioId_Actividad
    ON dbo.Bitacora (UsuarioId, Actividad);
GO

CREATE TABLE dbo.UsuarioHistorial
(
    HistorialId INT IDENTITY(1,1) NOT NULL,
    UsuarioId INT NOT NULL,
    FechaHora DATETIME NOT NULL,
    ActorUsuarioId INT NULL,
    ActorUsernameSnapshot VARCHAR(50) NULL,
    Accion VARCHAR(100) NOT NULL,
    Observaciones VARCHAR(500) NULL,
    Username VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(MAX) NOT NULL,
    Activo BIT NOT NULL,
    IdiomaId INT NULL,
    CONSTRAINT PK_UsuarioHistorial PRIMARY KEY CLUSTERED (HistorialId),
    CONSTRAINT FK_UsuarioHistorial_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT FK_UsuarioHistorial_ActorUsuario FOREIGN KEY (ActorUsuarioId) REFERENCES dbo.Usuarios(Id)
);
GO

CREATE INDEX IX_UsuarioHistorial_UsuarioId_FechaHora
    ON dbo.UsuarioHistorial (UsuarioId, FechaHora DESC, HistorialId DESC);
GO

CREATE TABLE dbo.UsuarioHistorialPermiso
(
    HistorialId INT NOT NULL,
    PermisoId INT NOT NULL,
    CONSTRAINT PK_UsuarioHistorialPermiso PRIMARY KEY CLUSTERED (HistorialId, PermisoId),
    CONSTRAINT FK_UsuarioHistorialPermiso_Historial FOREIGN KEY (HistorialId) REFERENCES dbo.UsuarioHistorial(HistorialId)
);
GO

-- BEGIN AUTO-GENERATED I18N BASE SEED
-- AUTO-GENERATED BY .agents/skills/translation-base-seed-sync/scripts/sync-i18n-base-seed.ps1
-- SOURCE OF TRUTH: translation-seeds/base.es.json
-- Si ejecuta desde SSMS en SQLCMD Mode, cambie RepoRoot por la ruta absoluta del repo en su maquina.
-- Con setup-database.ps1 deje RepoRoot = . : el wrapper lo reemplaza solo.
:setvar RepoRoot "D:\ian.local\Escritorio\TP-IDS-main"
-- Formas soportadas de ejecutar este script: SSMS 19/22 en SQLCMD Mode, o .\setup-database.ps1 desde la raiz del repo.
-- OJO: NO ejecutarlo directo con sqlcmd ODBC 15.x: al llegar a la linea !! corta en silencio (exit 0) y la BD queda sin traducciones.
-- El helper prepara una copia publica del JSON para que OPENROWSET no dependa de permisos sobre el perfil del usuario.
!! powershell -NoProfile -ExecutionPolicy Bypass -File "$(RepoRoot)\.agents\skills\translation-base-seed-sync\scripts\emit-seed-json-sqlcmd-path.ps1" -RepoRoot "$(RepoRoot)" -OutputFile "NUL"
GO
DECLARE @IdiomaBaseId INT;
INSERT INTO dbo.Idioma (Nombre, [Default]) VALUES (N'Español', 1);
SET @IdiomaBaseId = SCOPE_IDENTITY();

DECLARE @RutaTraduccionesBase NVARCHAR(4000) = N'C:\Users\Public\Documents\TpIDS\SqlSeed\base.es.json';
DECLARE @TraduccionesBaseJson NVARCHAR(MAX);
DECLARE @SqlCargaJson NVARCHAR(MAX);

SET @SqlCargaJson = N'SELECT @JsonOut = CONVERT(NVARCHAR(MAX), BulkColumn)
FROM OPENROWSET(
    BULK ''' + REPLACE(@RutaTraduccionesBase, '''', '''''') + ''',
    SINGLE_CLOB,
    CODEPAGE = ''65001''
) AS JsonFile;';

BEGIN TRY
    EXEC sys.sp_executesql
        @SqlCargaJson,
        N'@JsonOut NVARCHAR(MAX) OUTPUT',
        @JsonOut = @TraduccionesBaseJson OUTPUT;
END TRY
BEGIN CATCH
    THROW 50000, 'No se pudo leer translation-seeds/base.es.json desde SQL Server. Verifique la ruta, los permisos del servicio y el acceso del servidor al archivo.', 1;
END CATCH

IF NULLIF(LTRIM(RTRIM(@TraduccionesBaseJson)), N'') IS NULL
BEGIN
    THROW 50001, 'El archivo translation-seeds/base.es.json esta vacio o no pudo leerse correctamente.', 1;
END

DECLARE @Seed TABLE
(
    [Key] VARCHAR(150) NOT NULL PRIMARY KEY,
    Valor VARCHAR(300) NOT NULL
);

INSERT INTO @Seed ([Key], Valor)
SELECT [key], TRY_CAST([value] AS VARCHAR(300))
FROM OPENJSON(@TraduccionesBaseJson)
WHERE [type] IN (1, 2);

IF NOT EXISTS (SELECT 1 FROM @Seed)
BEGIN
    THROW 50002, 'El archivo translation-seeds/base.es.json no contiene traducciones validas.', 1;
END

IF EXISTS (SELECT 1 FROM @Seed WHERE [Key] IS NULL OR Valor IS NULL)
BEGIN
    THROW 50003, 'El archivo translation-seeds/base.es.json contiene keys o valores incompatibles con el esquema esperado.', 1;
END

INSERT INTO dbo.Palabra ([Key])
SELECT [Key]
FROM @Seed
ORDER BY [Key];

INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor)
SELECT p.Id, @IdiomaBaseId, s.Valor
FROM @Seed s
INNER JOIN dbo.Palabra p ON p.[Key] = s.[Key];

-- END AUTO-GENERATED I18N BASE SEED
GO
