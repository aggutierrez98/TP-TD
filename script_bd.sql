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
        CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Usuarios_Username UNIQUE (Username)
    );
END
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
        CONSTRAINT PK_Palabra PRIMARY KEY CLUSTERED (Id)
    );
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

IF EXISTS (SELECT 1 FROM dbo.Idioma)
BEGIN
    ;WITH Defaults AS
    (
        SELECT Id,
               ROW_NUMBER() OVER (ORDER BY CASE WHEN Nombre = 'Español' THEN 0 ELSE 1 END, Id) AS Orden
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
        ORDER BY CASE WHEN Nombre = 'Español' THEN 0 ELSE 1 END, Id;

        UPDATE dbo.Idioma
        SET [Default] = CASE WHEN Id = @IdiomaDefaultId THEN 1 ELSE 0 END;
    END
END
GO
