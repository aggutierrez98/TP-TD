IF DB_ID('TpCatedraIDS') IS NULL
BEGIN
    CREATE DATABASE TpCatedraIDS;
END
GO

USE TpCatedraIDS;
GO

IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Username VARCHAR(50) NOT NULL,
        PasswordHash VARCHAR(MAX) NOT NULL,
        Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
        CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Usuarios_Username UNIQUE (Username)
    );
END
GO

IF COL_LENGTH('dbo.Usuarios', 'Activo') IS NULL
BEGIN
    ALTER TABLE dbo.Usuarios
    ADD Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1);
END
GO

UPDATE dbo.Usuarios
SET Activo = 1
WHERE Activo IS NULL;
GO

IF OBJECT_ID('dbo.Idioma', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Idioma
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Nombre VARCHAR(100) NOT NULL,
        [Default] BIT NOT NULL CONSTRAINT DF_Idioma_Default DEFAULT (0),
        CONSTRAINT PK_Idioma PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF COL_LENGTH('dbo.Idioma', 'Default') IS NULL
BEGIN
    ALTER TABLE dbo.Idioma
    ADD [Default] BIT NOT NULL CONSTRAINT DF_Idioma_Default DEFAULT (0);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UQ_Idioma_Nombre'
      AND object_id = OBJECT_ID('dbo.Idioma'))
BEGIN
    ALTER TABLE dbo.Idioma
    ADD CONSTRAINT UQ_Idioma_Nombre UNIQUE (Nombre);
END
GO

IF OBJECT_ID('dbo.Palabra', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Palabra
    (
        Id INT IDENTITY(1,1) NOT NULL,
        [Key] VARCHAR(150) NOT NULL,
        EsBase BIT NOT NULL CONSTRAINT DF_Palabra_EsBase DEFAULT (0),
        CONSTRAINT PK_Palabra PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF COL_LENGTH('dbo.Palabra', 'EsBase') IS NULL
BEGIN
    ALTER TABLE dbo.Palabra
    ADD EsBase BIT NOT NULL CONSTRAINT DF_Palabra_EsBase DEFAULT (0);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UQ_Palabra_Key'
      AND object_id = OBJECT_ID('dbo.Palabra'))
BEGIN
    ALTER TABLE dbo.Palabra
    ADD CONSTRAINT UQ_Palabra_Key UNIQUE ([Key]);
END
GO

IF OBJECT_ID('dbo.Traduccion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Traduccion
    (
        PalabraId INT NOT NULL,
        IdiomaId INT NOT NULL,
        Valor VARCHAR(300) NOT NULL
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE name = 'PK_Traduccion'
      AND parent_object_id = OBJECT_ID('dbo.Traduccion'))
BEGIN
    ALTER TABLE dbo.Traduccion
    ADD CONSTRAINT PK_Traduccion PRIMARY KEY CLUSTERED (PalabraId, IdiomaId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Traduccion_Palabra')
BEGIN
    ALTER TABLE dbo.Traduccion
    ADD CONSTRAINT FK_Traduccion_Palabra FOREIGN KEY (PalabraId) REFERENCES dbo.Palabra(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Traduccion_Idioma')
BEGIN
    ALTER TABLE dbo.Traduccion
    ADD CONSTRAINT FK_Traduccion_Idioma FOREIGN KEY (IdiomaId) REFERENCES dbo.Idioma(Id);
END
GO

IF OBJECT_ID('dbo.UsuarioPreferencias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioPreferencias
    (
        UsuarioId INT NOT NULL,
        IdiomaId INT NOT NULL
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE name = 'PK_UsuarioPreferencias'
      AND parent_object_id = OBJECT_ID('dbo.UsuarioPreferencias'))
BEGIN
    ALTER TABLE dbo.UsuarioPreferencias
    ADD CONSTRAINT PK_UsuarioPreferencias PRIMARY KEY CLUSTERED (UsuarioId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_UsuarioPreferencias_Usuario')
BEGIN
    ALTER TABLE dbo.UsuarioPreferencias
    ADD CONSTRAINT FK_UsuarioPreferencias_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_UsuarioPreferencias_Idioma')
BEGIN
    ALTER TABLE dbo.UsuarioPreferencias
    ADD CONSTRAINT FK_UsuarioPreferencias_Idioma FOREIGN KEY (IdiomaId) REFERENCES dbo.Idioma(Id);
END
GO

IF OBJECT_ID('dbo.Permiso', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permiso
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Nombre VARCHAR(100) NOT NULL,
        CONSTRAINT PK_Permiso PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UQ_Permiso_Nombre'
      AND object_id = OBJECT_ID('dbo.Permiso'))
BEGIN
    ALTER TABLE dbo.Permiso
    ADD CONSTRAINT UQ_Permiso_Nombre UNIQUE (Nombre);
END
GO

IF OBJECT_ID('dbo.Grupo', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Grupo
    (
        GrupoId INT NOT NULL,
        PermisoId INT NOT NULL
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE name = 'PK_Grupo'
      AND parent_object_id = OBJECT_ID('dbo.Grupo'))
BEGIN
    ALTER TABLE dbo.Grupo
    ADD CONSTRAINT PK_Grupo PRIMARY KEY CLUSTERED (GrupoId, PermisoId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Grupo_Grupo')
BEGIN
    ALTER TABLE dbo.Grupo
    ADD CONSTRAINT FK_Grupo_Grupo FOREIGN KEY (GrupoId) REFERENCES dbo.Permiso(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Grupo_Permiso')
BEGIN
    ALTER TABLE dbo.Grupo
    ADD CONSTRAINT FK_Grupo_Permiso FOREIGN KEY (PermisoId) REFERENCES dbo.Permiso(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_Grupo_Distinto')
BEGIN
    ALTER TABLE dbo.Grupo
    ADD CONSTRAINT CK_Grupo_Distinto CHECK (GrupoId <> PermisoId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UQ_Grupo_PermisoId'
      AND object_id = OBJECT_ID('dbo.Grupo'))
BEGIN
    CREATE UNIQUE INDEX UQ_Grupo_PermisoId
        ON dbo.Grupo (PermisoId);
END
GO

IF OBJECT_ID('dbo.UsuarioPermiso', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioPermiso
    (
        UsuarioId INT NOT NULL,
        PermisoId INT NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.Bitacora', 'U') IS NULL
BEGIN
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
        CONSTRAINT PK_Bitacora PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Bitacora_Usuario')
BEGIN
    ALTER TABLE dbo.Bitacora
    ADD CONSTRAINT FK_Bitacora_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Bitacora_FechaHora'
      AND object_id = OBJECT_ID('dbo.Bitacora'))
BEGIN
    CREATE INDEX IX_Bitacora_FechaHora
        ON dbo.Bitacora (FechaHora DESC);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Bitacora_UsuarioId_Actividad'
      AND object_id = OBJECT_ID('dbo.Bitacora'))
BEGIN
    CREATE INDEX IX_Bitacora_UsuarioId_Actividad
        ON dbo.Bitacora (UsuarioId, Actividad);
END
GO

IF OBJECT_ID('dbo.UsuarioHistorial', 'U') IS NULL
BEGIN
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
        CONSTRAINT PK_UsuarioHistorial PRIMARY KEY CLUSTERED (HistorialId)
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_UsuarioHistorial_Usuario')
BEGIN
    ALTER TABLE dbo.UsuarioHistorial
    ADD CONSTRAINT FK_UsuarioHistorial_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_UsuarioHistorial_ActorUsuario')
BEGIN
    ALTER TABLE dbo.UsuarioHistorial
    ADD CONSTRAINT FK_UsuarioHistorial_ActorUsuario FOREIGN KEY (ActorUsuarioId) REFERENCES dbo.Usuarios(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_UsuarioHistorial_UsuarioId_FechaHora'
      AND object_id = OBJECT_ID('dbo.UsuarioHistorial'))
BEGIN
    CREATE INDEX IX_UsuarioHistorial_UsuarioId_FechaHora
        ON dbo.UsuarioHistorial (UsuarioId, FechaHora DESC, HistorialId DESC);
END
GO

IF OBJECT_ID('dbo.UsuarioHistorialPermiso', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioHistorialPermiso
    (
        HistorialId INT NOT NULL,
        PermisoId INT NOT NULL
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE name = 'PK_UsuarioHistorialPermiso'
      AND parent_object_id = OBJECT_ID('dbo.UsuarioHistorialPermiso'))
BEGIN
    ALTER TABLE dbo.UsuarioHistorialPermiso
    ADD CONSTRAINT PK_UsuarioHistorialPermiso PRIMARY KEY CLUSTERED (HistorialId, PermisoId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_UsuarioHistorialPermiso_Historial')
BEGIN
    ALTER TABLE dbo.UsuarioHistorialPermiso
    ADD CONSTRAINT FK_UsuarioHistorialPermiso_Historial FOREIGN KEY (HistorialId) REFERENCES dbo.UsuarioHistorial(HistorialId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE name = 'PK_UsuarioPermiso'
      AND parent_object_id = OBJECT_ID('dbo.UsuarioPermiso'))
BEGIN
    ALTER TABLE dbo.UsuarioPermiso
    ADD CONSTRAINT PK_UsuarioPermiso PRIMARY KEY CLUSTERED (UsuarioId, PermisoId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_UsuarioPermiso_Usuario')
BEGIN
    ALTER TABLE dbo.UsuarioPermiso
    ADD CONSTRAINT FK_UsuarioPermiso_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_UsuarioPermiso_Permiso')
BEGIN
    ALTER TABLE dbo.UsuarioPermiso
    ADD CONSTRAINT FK_UsuarioPermiso_Permiso FOREIGN KEY (PermisoId) REFERENCES dbo.Permiso(Id);
END
GO

-- BEGIN AUTO-GENERATED I18N BASE SEED
-- AUTO-GENERATED BY .agents/skills/translation-base-seed-sync/scripts/sync-i18n-base-seed.ps1
DECLARE @IdiomaBaseId INT;
SELECT TOP 1 @IdiomaBaseId = Id FROM dbo.Idioma WHERE Nombre = N'Español';
IF @IdiomaBaseId IS NULL
BEGIN
    INSERT INTO dbo.Idioma (Nombre, [Default]) VALUES (N'Español', 0);
    SET @IdiomaBaseId = SCOPE_IDENTITY();
END
DECLARE @PalabraId INT;

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.ActivityLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.ActivityLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Actividad:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Actividad:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.FiltersGroup';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.FiltersGroup', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Filtros' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Filtros');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.FromCheck';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.FromCheck', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Desde' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Desde');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.Level.Error';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.Level.Error', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Error' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Error');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.Level.Information';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.Level.Information', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Informacion' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Informacion');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.Level.Warning';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.Level.Warning', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Advertencia' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Advertencia');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.LevelLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.LevelLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Nivel:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Nivel:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.OnlyTechnicalCheck';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.OnlyTechnicalCheck', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Solo tecnicas' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Solo tecnicas');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.SearchButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.SearchButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Buscar' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Buscar');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.Title';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.Title', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Bitacora' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Bitacora');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.TitleLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.TitleLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Bitacora' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Bitacora');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.ToCheck';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.ToCheck', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Hasta' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Hasta');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormBitacora.UserLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormBitacora.UserLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Usuario:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Usuario:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormConfiguracionUsuario.CurrentUserPrefix';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormConfiguracionUsuario.CurrentUserPrefix', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Usuario actual:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Usuario actual:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormConfiguracionUsuario.LanguageLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormConfiguracionUsuario.LanguageLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Idioma para la sesion:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Idioma para la sesion:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormConfiguracionUsuario.SaveButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormConfiguracionUsuario.SaveButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Guardar' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Guardar');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormConfiguracionUsuario.Title';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormConfiguracionUsuario.Title', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Configuracion del usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Configuracion del usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormConfiguracionUsuario.TitleLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormConfiguracionUsuario.TitleLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Configuracion del usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Configuracion del usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.AddButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.AddButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Agregar idioma' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Agregar idioma');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.DefaultCheck';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.DefaultCheck', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Idioma default' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Idioma default');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.DeleteButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.DeleteButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Eliminar idioma' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Eliminar idioma');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.DeleteTranslationButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.DeleteTranslationButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Eliminar traduccion' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Eliminar traduccion');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.Grid.Key';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.Grid.Key', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Key' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Key');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.Grid.Value';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.Grid.Value', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Traduccion' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Traduccion');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.ImportButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.ImportButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Importar JSON' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Importar JSON');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.LanguageGroup';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.LanguageGroup', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Datos del idioma' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Datos del idioma');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.ManualKeyLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.ManualKeyLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Key:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Key:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.ManualValueLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.ManualValueLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Traduccion:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Traduccion:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.NameLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.NameLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Nombre del idioma:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Nombre del idioma:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.SaveTranslationButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.SaveTranslationButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Guardar traduccion' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Guardar traduccion');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.SelectLanguageLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.SelectLanguageLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Idioma seleccionado:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Idioma seleccionado:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.Title';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.Title', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Gestion de idiomas' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Gestion de idiomas');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.TitleLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.TitleLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Gestion de idiomas' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Gestion de idiomas');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.TranslationsGroup';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.TranslationsGroup', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Traducciones' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Traducciones');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionIdiomas.UpdateButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionIdiomas.UpdateButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Actualizar idioma' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Actualizar idioma');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.ChildButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.ChildButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Agregar hijo al grupo seleccionado' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Agregar hijo al grupo seleccionado');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.DeleteButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.DeleteButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Eliminar seleccionado' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Eliminar seleccionado');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.EditionGroup';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.EditionGroup', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Edicion del catalogo' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Edicion del catalogo');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.NameLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.NameLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Nombre:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Nombre:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.RenameButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.RenameButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Renombrar seleccionado' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Renombrar seleccionado');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.RootButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.RootButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Agregar nodo raiz' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Agregar nodo raiz');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.SaveButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.SaveButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Guardar catalogo' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Guardar catalogo');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.Title';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.Title', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Gestion de permisos' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Gestion de permisos');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.TitleLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.TitleLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Gestion de permisos' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Gestion de permisos');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.TreeGroup';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.TreeGroup', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Catalogo de permisos' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Catalogo de permisos');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.TypeGroup';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.TypeGroup', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Grupo' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Grupo');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.TypeLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.TypeLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Tipo:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Tipo:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionPermisos.TypeLeaf';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionPermisos.TypeLeaf', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Permiso' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Permiso');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.AddButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.AddButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Agregar usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Agregar usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.BitacoraButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.BitacoraButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Bitacora' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Bitacora');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.ConfigurationButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.ConfigurationButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Configuracion' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Configuracion');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.DeleteButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.DeleteButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Eliminar usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Eliminar usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.EditButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.EditButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Editar usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Editar usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.Grid.Active';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.Grid.Active', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Activo' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Activo');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.Grid.Id';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.Grid.Id', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'ID' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'ID');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.Grid.Username';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.Grid.Username', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Nombre de usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Nombre de usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.HistoryButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.HistoryButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Historial de usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Historial de usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.LanguageManagementButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.LanguageManagementButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Gestion de idiomas' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Gestion de idiomas');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.PasswordLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.PasswordLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Contrasena (solo para agregar usuario):' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Contrasena (solo para agregar usuario):');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.PermissionManagementButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.PermissionManagementButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Gestion de permisos' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Gestion de permisos');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.PermissionTreeGroup';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.PermissionTreeGroup', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Permisos y grupos asignados' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Permisos y grupos asignados');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.SavePermissionsButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.SavePermissionsButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Guardar permisos del usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Guardar permisos del usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.SessionLabelPrefix';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.SessionLabelPrefix', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Sesion activa:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Sesion activa:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.Title';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.Title', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Gestion de Usuarios' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Gestion de Usuarios');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.TitleLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.TitleLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Gestion de Usuarios' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Gestion de Usuarios');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.UserDataGroup';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.UserDataGroup', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Datos del usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Datos del usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormGestionUsuarios.UsernameLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormGestionUsuarios.UsernameLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Nombre de usuario:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Nombre de usuario:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormHistorialUsuario.RefreshButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormHistorialUsuario.RefreshButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Refrescar historial' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Refrescar historial');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormHistorialUsuario.RestoreButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormHistorialUsuario.RestoreButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Restaurar version' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Restaurar version');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormHistorialUsuario.Title';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormHistorialUsuario.Title', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Historial de usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Historial de usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormHistorialUsuario.TitleLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormHistorialUsuario.TitleLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Historial de usuario' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Historial de usuario');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormLogin.LoginButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormLogin.LoginButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Ingresar' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Ingresar');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormLogin.NicknameLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormLogin.NicknameLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Ingrese su nickname:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Ingrese su nickname:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormLogin.PasswordLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormLogin.PasswordLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Ingrese su contrasena:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Ingrese su contrasena:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormLogin.RegisterButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormLogin.RegisterButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Registrarme' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Registrarme');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormLogin.RegisterPrompt';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormLogin.RegisterPrompt', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'No tenes usuario?' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'No tenes usuario?');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormLogin.Title';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormLogin.Title', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Menu de ingreso' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Menu de ingreso');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormRegistro.NicknameLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormRegistro.NicknameLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Ingrese su nickname:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Ingrese su nickname:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormRegistro.PasswordLabel';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormRegistro.PasswordLabel', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Ingrese su contrasena:' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Ingrese su contrasena:');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormRegistro.RegisterButton';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormRegistro.RegisterButton', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Registrarse' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Registrarse');
END

SET @PalabraId = NULL;
SELECT @PalabraId = Id FROM dbo.Palabra WHERE [Key] = N'FormRegistro.Title';
IF @PalabraId IS NULL
BEGIN
    INSERT INTO dbo.Palabra ([Key], EsBase) VALUES (N'FormRegistro.Title', 1);
    SET @PalabraId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Palabra SET EsBase = 1 WHERE Id = @PalabraId;
END
IF EXISTS (SELECT 1 FROM dbo.Traduccion WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId)
BEGIN
    UPDATE dbo.Traduccion
    SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Valor)), '') IS NULL THEN N'Registro' ELSE Valor END
    WHERE PalabraId = @PalabraId AND IdiomaId = @IdiomaBaseId;
END
ELSE
BEGIN
    INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor) VALUES (@PalabraId, @IdiomaBaseId, N'Registro');
END

-- END AUTO-GENERATED I18N BASE SEED

IF EXISTS (SELECT 1 FROM dbo.Idioma)
BEGIN
    ;WITH Defaults AS
    (
        SELECT Id,
               ROW_NUMBER() OVER (ORDER BY Id) AS Orden
        FROM dbo.Idioma
        WHERE [Default] = 1
    )
    UPDATE i
    SET [Default] = CASE WHEN d.Orden = 1 THEN 1 ELSE 0 END
    FROM dbo.Idioma i
    INNER JOIN Defaults d ON d.Id = i.Id;

    IF NOT EXISTS (SELECT 1 FROM dbo.Idioma WHERE [Default] = 1)
    BEGIN
        DECLARE @IdiomaDefaultId INT;

        SELECT TOP 1 @IdiomaDefaultId = Id
        FROM dbo.Idioma
        ORDER BY Nombre, Id;

        UPDATE dbo.Idioma
        SET [Default] = CASE WHEN Id = @IdiomaDefaultId THEN 1 ELSE 0 END;
    END
END
GO

