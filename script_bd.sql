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
-- SOURCE OF TRUTH: translation-seeds/base.es.json
DECLARE @IdiomaBaseId INT;
SELECT TOP 1 @IdiomaBaseId = Id FROM dbo.Idioma WHERE Nombre = N'Español';
IF @IdiomaBaseId IS NULL
BEGIN
    INSERT INTO dbo.Idioma (Nombre, [Default]) VALUES (N'Español', 0);
    SET @IdiomaBaseId = SCOPE_IDENTITY();
END

DECLARE @TraduccionesBaseJson NVARCHAR(MAX) = N'';
SET @TraduccionesBaseJson = @TraduccionesBaseJson + N'{
    "Dialog.Title.Confirmation":  "Confirmacion",
    "Dialog.Title.Error":  "Error",
    "Dialog.Title.Success":  "Exito",
    "Dialog.Title.Warning":  "Atencion",
    "Error.Idioma.CannotDeleteBaseTranslation":  "No se pueden eliminar traducciones de keys base del sistema.",
    "Error.Idioma.CannotDeleteLastLanguage":  "No se puede eliminar el unico idioma existente.",
    "Error.Idioma.DefaultNotFound":  "No se encontro un idioma default configurado.",
    "Error.Idioma.ExactlyOneDefaultRequired":  "Debe existir exactamente un idioma default.",
    "Error.Idioma.ExportPathRequired":  "Debe seleccionar una ruta de destino para exportar el JSON.",
    "Error.Idioma.IncompleteSelection":  "El idioma seleccionado no esta completo y no puede usarse en la sesion.",
    "Error.Idioma.JsonEmpty":  "El archivo JSON no contiene traducciones.",
    "Error.Idioma.JsonFileNotFound":  "No se encontro el archivo JSON seleccionado.",
    "Error.Idioma.JsonInvalidFormat":  "El archivo JSON no tiene un formato valido.",
    "Error.Idioma.KeyRequired":  "La key no puede estar vacia.",
    "Error.Idioma.NameAlreadyExists":  "Ya existe un idioma con ese nombre.",
    "Error.Idioma.NameRequired":  "El nombre del idioma no puede estar vacio.",
    "Error.Idioma.NotFound":  "No se encontro el idioma seleccionado.",
    "Error.Idioma.PreferenceSaveFailed":  "No se pudo guardar la preferencia de idioma.",
    "Error.Idioma.TranslationRequired":  "La traduccion no puede estar vacia.",
    "Error.Idioma.UserPreferenceSaveFailed":  "No se pudo guardar la preferencia del usuario.",
    "Error.Permiso.CatalogEmpty":  "Debe existir al menos un permiso o grupo.",
    "Error.Permiso.CycleDetected":  "No se pueden generar ciclos en el arbol de permisos.",
    "Error.Permiso.DuplicateNames":  "No puede haber permisos o grupos con el mismo nombre.",
    "Error.Permiso.EmptyGroupsNotAllowed":  "No se pueden guardar grupos vacios.",
    "Error.Permiso.NameRequired":  "El nombre del permiso no puede estar vacio.",
    "Error.Permiso.SaveUserPermissionsFailed":  "No se pudieron guardar los permisos del usuario.",
    "Error.Permiso.UserNotFound":  "No se encontro el usuario seleccionado.",
    "Error.Tecnico.SchemaInitializationFailed":  "No se pudo inicializar el esquema de base de datos.",
    "Error.Tecnico.Unexpected":  "Ocurrio un error tecnico inesperado.",
    "Error.Usuario.CannotDeactivateActiveSession":  "No se puede desactivar al usuario que tiene la sesion activa.",
    "Error.Usuario.CannotDeleteActiveSession":  "No se puede eliminar al usuario que tiene la sesion activa.",
    "Error.Usuario.CannotRestoreInactiveCurrentSession":  "No se puede restaurar al estado inactivo del usuario con sesion actual.",
    "Error.Usuario.DeleteFailed":  "No se pudo eliminar el usuario.",
    "Error.Usuario.EmptyCredentials":  "El usuario y la contrasena no pueden estar vacios.",
    "Error.Usuario.HistoryUserNotFound":  "No se encontro el usuario asociado al historial.",
    "Error.Usuario.NotFound":  "No se encontro el usuario seleccionado.",
    "Error.Usuario.RegisterFailed":  "No se pudo registrar el usuario.",
    "Error.Usuario.RestoreVersionFailed":  "No se pudo restaurar la version seleccionada.",
    "Error.Usuario.RestoreVersionNotFound":  "No se encontro la version seleccionada.",
    "Error.Usuario.UpdateFailed":  "No se pudo actualizar el usuario.",
    "Error.Usuario.UsernameExists":  "El nombre de usuario ya existe. P';
SET @TraduccionesBaseJson = @TraduccionesBaseJson + N'or favor elegi otro.",
    "Error.Usuario.UsernameRequired":  "El nombre de usuario no puede estar vacio.",
    "FormBitacora.ActivityLabel":  "Actividad:",
    "FormBitacora.FiltersGroup":  "Filtros",
    "FormBitacora.FromCheck":  "Desde",
    "FormBitacora.Grid.Actividad":  "Actividad",
    "FormBitacora.Grid.EsTecnica":  "Es tecnica",
    "FormBitacora.Grid.FechaHora":  "Fecha y hora",
    "FormBitacora.Grid.Id":  "ID",
    "FormBitacora.Grid.Informacion":  "Informacion",
    "FormBitacora.Grid.Nivel":  "Nivel",
    "FormBitacora.Grid.Origen":  "Origen",
    "FormBitacora.Grid.UsernameSnapshot":  "Usuario",
    "FormBitacora.Grid.UsuarioId":  "ID usuario",
    "FormBitacora.Level.Error":  "Error",
    "FormBitacora.Level.Information":  "Informacion",
    "FormBitacora.Level.Warning":  "Advertencia",
    "FormBitacora.LevelLabel":  "Nivel:",
    "FormBitacora.OnlyTechnicalCheck":  "Solo tecnicas",
    "FormBitacora.SearchButton":  "Buscar",
    "FormBitacora.Title":  "Bitacora",
    "FormBitacora.TitleLabel":  "Bitacora",
    "FormBitacora.ToCheck":  "Hasta",
    "FormBitacora.UserLabel":  "Usuario:",
    "FormBitacora.Value.Activity.ActualizacionPreferenciaIdioma":  "Actualizacion preferencia idioma",
    "FormBitacora.Value.Activity.ActualizacionTraduccion":  "Actualizacion traduccion",
    "FormBitacora.Value.Activity.AltaIdioma":  "Alta de idioma",
    "FormBitacora.Value.Activity.AltaUsuario":  "Alta de usuario",
    "FormBitacora.Value.Activity.AltaUsuarioRechazada":  "Alta de usuario rechazada",
    "FormBitacora.Value.Activity.BajaIdioma":  "Baja de idioma",
    "FormBitacora.Value.Activity.BajaLogicaUsuario":  "Baja logica de usuario",
    "FormBitacora.Value.Activity.BajaTraduccion":  "Baja de traduccion",
    "FormBitacora.Value.Activity.BorradoFisicoUsuario":  "Borrado fisico de usuario",
    "FormBitacora.Value.Activity.BorradoFisicoUsuarioRechazado":  "Borrado fisico de usuario rechazado",
    "FormBitacora.Value.Activity.EdicionIdioma":  "Edicion de idioma",
    "FormBitacora.Value.Activity.EdicionUsuario":  "Edicion de usuario",
    "FormBitacora.Value.Activity.EdicionUsuarioRechazada":  "Edicion de usuario rechazada",
    "FormBitacora.Value.Activity.ExportacionTraducciones":  "Exportacion de traducciones",
    "FormBitacora.Value.Activity.ImportacionTraducciones":  "Importacion de traducciones",
    "FormBitacora.Value.Activity.LoginExitoso":  "Login exitoso",
    "FormBitacora.Value.Activity.LoginFallido":  "Login fallido",
    "FormBitacora.Value.Activity.ReactivacionUsuario":  "Reactivacion de usuario",
    "FormBitacora.Value.Activity.RestauracionUsuario":  "Restauracion de usuario",
    "FormBitacora.Value.Info.ActualizacionPreferenciaIdioma":  "Se actualizo el idioma preferido del usuario.",
    "FormBitacora.Value.Info.ActualizacionTraduccion":  "Se guardo la traduccion.",
    "FormBitacora.Value.Info.AltaIdioma":  "Se creo el idioma.",
    "FormBitacora.Value.Info.AltaUsuario":  "Se creo el usuario.",
    "FormBitacora.Value.Info.AltaUsuarioDatosIncompletos":  "Se intento registrar un usuario con datos incompletos.",
    "FormBitacora.Value.Info.BajaIdioma":  "Se elimino el idioma.",
    "FormBitacora.Value.Info.BajaLogicaUsuario":  "Se desactivo el usuario.",
    "FormBitacora.Value.Info.BajaTraduccion":  "Se elimino la traduccion.",
    "FormBitacora.Value.Info.BorradoFisicoSesion":  "Se intento eliminar al usuario en sesion.",
    "FormBitacora.Value.Inf';
SET @TraduccionesBaseJson = @TraduccionesBaseJson + N'o.BorradoFisicoUsuario":  "Se elimino definitivamente el usuario y su historial asociado.",
    "FormBitacora.Value.Info.EdicionDesactivarSesion":  "Se intento desactivar al usuario en sesion.",
    "FormBitacora.Value.Info.EdicionIdioma":  "Se actualizo el idioma.",
    "FormBitacora.Value.Info.EdicionUsuario":  "Se actualizo el usuario.",
    "FormBitacora.Value.Info.EdicionUsuarioNombreVacio":  "Se intento guardar un nombre vacio.",
    "FormBitacora.Value.Info.ExportacionTraducciones":  "Se exportaron traducciones JSON.",
    "FormBitacora.Value.Info.ImportacionTraducciones":  "Se importaron traducciones JSON.",
    "FormBitacora.Value.Info.LoginCamposVacios":  "Intento de login con campos vacios.",
    "FormBitacora.Value.Info.LoginCredencialesInvalidas":  "Credenciales invalidas.",
    "FormBitacora.Value.Info.LoginExitoso":  "Ingreso de usuario.",
    "FormBitacora.Value.Info.LoginUsuarioInactivo":  "Intento de acceso sobre usuario inactivo.",
    "FormBitacora.Value.Info.LoginUsuarioInexistente":  "Intento de login con usuario inexistente.",
    "FormBitacora.Value.Info.NombreUsuarioYaExiste":  "El nombre de usuario ya existe.",
    "FormBitacora.Value.Info.ReactivacionUsuario":  "Se reactivo el usuario.",
    "FormBitacora.Value.Info.RestauracionUsuario":  "Se restauro el usuario desde el historial.",
    "FormConfiguracionUsuario.CurrentUserPrefix":  "Usuario actual:",
    "FormConfiguracionUsuario.LanguageLabel":  "Idioma para la sesion:",
    "FormConfiguracionUsuario.SaveButton":  "Guardar",
    "FormConfiguracionUsuario.Title":  "Configuracion del usuario",
    "FormConfiguracionUsuario.TitleLabel":  "Configuracion del usuario",
    "FormGestionIdiomas.AddButton":  "Agregar idioma",
    "FormGestionIdiomas.DefaultCheck":  "Idioma default",
    "FormGestionIdiomas.DeleteButton":  "Eliminar idioma",
    "FormGestionIdiomas.DeleteTranslationButton":  "Eliminar traduccion",
    "FormGestionIdiomas.ExportButton":  "Exportar JSON",
    "FormGestionIdiomas.Grid.Key":  "Key",
    "FormGestionIdiomas.Grid.Value":  "Traduccion",
    "FormGestionIdiomas.ImportButton":  "Importar JSON",
    "FormGestionIdiomas.LanguageGroup":  "Datos del idioma",
    "FormGestionIdiomas.ManualKeyLabel":  "Key:",
    "FormGestionIdiomas.ManualValueLabel":  "Traduccion:",
    "FormGestionIdiomas.NameLabel":  "Nombre del idioma:",
    "FormGestionIdiomas.SaveTranslationButton":  "Guardar traduccion",
    "FormGestionIdiomas.SelectLanguageLabel":  "Idioma seleccionado:",
    "FormGestionIdiomas.Title":  "Gestion de idiomas",
    "FormGestionIdiomas.TitleLabel":  "Gestion de idiomas",
    "FormGestionIdiomas.TranslationsGroup":  "Traducciones",
    "FormGestionIdiomas.UpdateButton":  "Actualizar idioma",
    "FormGestionPermisos.ChildButton":  "Agregar hijo al grupo seleccionado",
    "FormGestionPermisos.DeleteButton":  "Eliminar seleccionado",
    "FormGestionPermisos.EditionGroup":  "Edicion del catalogo",
    "FormGestionPermisos.NameLabel":  "hola",
    "FormGestionPermisos.RenameButton":  "Renombrar seleccionado",
    "FormGestionPermisos.RootButton":  "Agregar nodo raiz",
    "FormGestionPermisos.SaveButton":  "Guardar catalogo",
    "FormGestionPermisos.Title":  "o",
    "FormGestionPermisos.TitleLabel":  "Gestion de permisos",
    "FormGestionPermisos.TreeGroup":  "Catalogo de permisos",
    "FormGestionPermisos.TypeGroup":  "Grupo",
    "FormGestionPermisos.TypeLabel":  "Tipo:",
    "FormGestionPe';
SET @TraduccionesBaseJson = @TraduccionesBaseJson + N'rmisos.TypeLeaf":  "Permiso",
    "FormGestionUsuarios.ActiveCheckbox":  "Usuario activo",
    "FormGestionUsuarios.AddButton":  "Agregar usuario",
    "FormGestionUsuarios.BitacoraButton":  "Bitacora",
    "FormGestionUsuarios.ConfigurationButton":  "Configuracion",
    "FormGestionUsuarios.DeleteButton":  "Eliminar usuario",
    "FormGestionUsuarios.EditButton":  "Editar usuario",
    "FormGestionUsuarios.Grid.Active":  "Activo",
    "FormGestionUsuarios.Grid.Id":  "ID",
    "FormGestionUsuarios.Grid.Username":  "Nombre de usuario",
    "FormGestionUsuarios.HistoryButton":  "Historial de usuario",
    "FormGestionUsuarios.LanguageManagementButton":  "Gestion de idiomas",
    "FormGestionUsuarios.PasswordLabel":  "Contrasena (obligatoria al agregar, opcional al editar):",
    "FormGestionUsuarios.PermissionManagementButton":  "Gestion de permisos",
    "FormGestionUsuarios.PermissionTreeGroup":  "Permisos y grupos asignados",
    "FormGestionUsuarios.SavePermissionsButton":  "Guardar permisos del usuario",
    "FormGestionUsuarios.SessionLabelPrefix":  "Sesion activa:",
    "FormGestionUsuarios.Title":  "Gestion de Usuarios",
    "FormGestionUsuarios.TitleLabel":  "Gestion de Usuarios",
    "FormGestionUsuarios.UserDataGroup":  "Datos del usuario",
    "FormGestionUsuarios.UsernameLabel":  "Nombre de usuario:",
    "FormHistorialUsuario.Confirm.RestoreVersionMessage":  "Esta seguro que desea restaurar esta version?",
    "FormHistorialUsuario.Confirm.RestoreVersionTitle":  "Confirmar restauracion",
    "FormHistorialUsuario.DiffGrid.Campo":  "Campo",
    "FormHistorialUsuario.DiffGrid.ValorAnterior":  "Valor anterior",
    "FormHistorialUsuario.DiffGrid.ValorNuevo":  "Valor nuevo",
    "FormHistorialUsuario.Grid.Accion":  "Accion",
    "FormHistorialUsuario.Grid.Activo":  "Activo",
    "FormHistorialUsuario.Grid.ActorUsernameSnapshot":  "Actor",
    "FormHistorialUsuario.Grid.ActorUsuarioId":  "ID actor",
    "FormHistorialUsuario.Grid.FechaHora":  "Fecha y hora",
    "FormHistorialUsuario.Grid.HistorialId":  "ID historial",
    "FormHistorialUsuario.Grid.IdiomaId":  "ID idioma",
    "FormHistorialUsuario.Grid.Observaciones":  "Observaciones",
    "FormHistorialUsuario.Grid.Username":  "Nombre de usuario",
    "FormHistorialUsuario.Grid.UsuarioId":  "ID usuario",
    "FormHistorialUsuario.RefreshButton":  "Refrescar historial",
    "FormHistorialUsuario.RestoreButton":  "Restaurar version",
    "FormHistorialUsuario.Success.VersionRestored":  "La version fue restaurada correctamente.",
    "FormHistorialUsuario.Title":  "Historial de usuario",
    "FormHistorialUsuario.TitleLabel":  "Historial de usuario",
    "FormHistorialUsuario.Value.Action.ActualizacionIdiomaPreferido":  "Actualizacion de idioma preferido",
    "FormHistorialUsuario.Value.Action.Alta":  "Alta",
    "FormHistorialUsuario.Value.Action.BajaLogica":  "Baja logica",
    "FormHistorialUsuario.Value.Action.Edicion":  "Edicion",
    "FormHistorialUsuario.Value.Action.Reactivacion":  "Reactivacion",
    "FormHistorialUsuario.Value.Action.Restauracion":  "Restauracion",
    "FormHistorialUsuario.Value.Field.Activo":  "Activo",
    "FormHistorialUsuario.Value.Field.IdiomaId":  "Idioma",
    "FormHistorialUsuario.Value.Field.PasswordHash":  "Contrasena",
    "FormHistorialUsuario.Value.Field.Permisos":  "Permisos",
    "FormHistorialUsuario.Value.Field.SinCambios":  "Sin cambios",
    "FormHistorialUsuario.Value.Field.Userna';
SET @TraduccionesBaseJson = @TraduccionesBaseJson + N'me":  "Nombre de usuario",
    "FormHistorialUsuario.Value.Marker.Actualizada":  "Actualizada",
    "FormHistorialUsuario.Value.Marker.Configurada":  "Configurada",
    "FormHistorialUsuario.Value.Marker.False":  "No",
    "FormHistorialUsuario.Value.Marker.True":  "Si",
    "FormHistorialUsuario.Value.Obs.ActualizacionIdioma":  "Actualizacion de la preferencia de idioma del usuario.",
    "FormHistorialUsuario.Value.Obs.ActualizacionPassword":  "Actualizacion del password del usuario.",
    "FormHistorialUsuario.Value.Obs.ActualizacionUsername":  "Actualizacion del nombre de usuario.",
    "FormHistorialUsuario.Value.Obs.ActualizacionUsuario":  "Actualizacion del usuario.",
    "FormHistorialUsuario.Value.Obs.AltaInicial":  "Alta inicial del usuario.",
    "FormHistorialUsuario.Value.Obs.Desactivacion":  "Desactivacion del usuario desde gestion.",
    "FormHistorialUsuario.Value.Obs.Reactivacion":  "Reactivacion del usuario desde gestion.",
    "FormHistorialUsuario.Value.Obs.Restauracion":  "Restauracion desde el historial.",
    "FormHistorialUsuario.Warning.SelectVersion":  "Debe seleccionar una version del historial.",
    "FormLogin.LoginButton":  "Ingresar",
    "FormLogin.NicknameLabel":  "Ingrese su nickname:",
    "FormLogin.PasswordLabel":  "Ingrese su contrasena:",
    "FormLogin.RegisterButton":  "Registrarme",
    "FormLogin.RegisterPrompt":  "No tenes usuario?",
    "FormLogin.Title":  "Menu de ingreso",
    "FormRegistro.NicknameLabel":  "Ingrese su nickname:",
    "FormRegistro.PasswordLabel":  "Ingrese su contrasena:",
    "FormRegistro.RegisterButton":  "Registrarse",
    "FormRegistro.Title":  "Registro"
}';

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = DB_NAME() AND compatibility_level < 130)
BEGIN
    ALTER DATABASE CURRENT SET COMPATIBILITY_LEVEL = 130;
END

DECLARE @Seed TABLE
(
    [Key] VARCHAR(150) NOT NULL,
    Valor VARCHAR(300) NOT NULL
);

INSERT INTO @Seed ([Key], Valor)
SELECT [key], TRY_CAST([value] AS VARCHAR(300))
FROM OPENJSON(@TraduccionesBaseJson)
WHERE [type] IN (1, 2);

MERGE dbo.Palabra AS Target
USING (SELECT [Key] FROM @Seed) AS Source
    ON Target.[Key] = Source.[Key]
WHEN MATCHED THEN
    UPDATE SET EsBase = 1
WHEN NOT MATCHED THEN
    INSERT ([Key], EsBase) VALUES (Source.[Key], 1);

MERGE dbo.Traduccion AS Target
USING
(
    SELECT p.Id AS PalabraId, @IdiomaBaseId AS IdiomaId, s.Valor
    FROM @Seed s
    INNER JOIN dbo.Palabra p ON p.[Key] = s.[Key]
) AS Source
    ON Target.PalabraId = Source.PalabraId AND Target.IdiomaId = Source.IdiomaId
WHEN MATCHED THEN
    UPDATE SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Target.Valor)), '') IS NULL THEN Source.Valor ELSE Target.Valor END
WHEN NOT MATCHED THEN
    INSERT (PalabraId, IdiomaId, Valor) VALUES (Source.PalabraId, Source.IdiomaId, Source.Valor);

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

