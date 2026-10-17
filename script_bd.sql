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
    IntentosFallidos INT NOT NULL CONSTRAINT DF_Usuarios_IntentosFallidos DEFAULT (0),
    Bloqueado BIT NOT NULL CONSTRAINT DF_Usuarios_Bloqueado DEFAULT (0),
    BloqueadoHasta DATETIME NULL,
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
    Valor NVARCHAR(300) NOT NULL,
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
DECLARE @PermisoGestionMovimientosId INT;
DECLARE @PermisoGestionCategoriasId INT;
DECLARE @PermisoGestionListaComprasId INT;
DECLARE @PermisoGestionBackupId INT;
DECLARE @PermisoGestionNotificacionesId INT;
DECLARE @PermisoGestionComerciosId INT;
DECLARE @PermisoGestionMonedasId INT;
DECLARE @PermisoGestionReglasCategorizacionId INT;
DECLARE @PermisoUsarAsistentePersonalId INT;
DECLARE @PermisoVerDashboardAnaliticasId INT;
DECLARE @PermisoVerBalanceGeneralId INT;
DECLARE @PermisoGestionMediosPagoId INT;
DECLARE @PermisoGestionEntidadesFinancierasId INT;
DECLARE @PermisoGestionCuentasFinancierasId INT;
DECLARE @GrupoAdminId INT;
DECLARE @GrupoBasicoId INT;
DECLARE @GrupoOperadorId INT;
DECLARE @AdminUserId INT;
DECLARE @UsuarioBasicoId INT;
DECLARE @UsuarioOperadorId INT;

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

-- NEG01/NEG02 (movimientos y categorias): a diferencia del resto (tecnicos, solo grupo admin),
-- estos dos son el producto en si: todo usuario basico los necesita desde el alta.
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionmovimientos', 0);
SET @PermisoGestionMovimientosId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestioncategorias', 0);
SET @PermisoGestionCategoriasId = SCOPE_IDENTITY();

-- Lista de compras: igual que movimientos/categorias, nucleo de producto disponible desde el alta.
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionlistacompras', 0);
SET @PermisoGestionListaComprasId = SCOPE_IDENTITY();

-- T08: tecnico, solo grupo admin (igual que gestionidiomas/verbitacora).
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionbackup', 0);
SET @PermisoGestionBackupId = SCOPE_IDENTITY();

-- Notificaciones (back-office): rol "operador" nuevo, exclusivo (no va a admin ni a basico).
-- El operador administra notificaciones de todos los usuarios, no solo las propias.
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionnotificaciones', 0);
SET @PermisoGestionNotificacionesId = SCOPE_IDENTITY();

-- Comercios (back-office): tambien rol "operador", pero a diferencia de notificaciones
-- el usuario pidio que admin lo tenga desde el arranque (no como agregado posterior).
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestioncomercios', 0);
SET @PermisoGestionComerciosId = SCOPE_IDENTITY();

-- Monedas (back-office): rol "operador", y al igual que Comercios el usuario pidio que
-- admin lo tenga desde el arranque.
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionmonedas', 0);
SET @PermisoGestionMonedasId = SCOPE_IDENTITY();

-- Reglas de categorizacion (Finanzas): nucleo disponible para admin y basico.
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionreglascategorizacion', 0);
SET @PermisoGestionReglasCategorizacionId = SCOPE_IDENTITY();

-- Asistente Personal (chat con IA): mismo alcance que Reglas de categorizacion,
-- nucleo de producto disponible para admin y basico desde el alta.
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('usarasistentepersonal', 0);
SET @PermisoUsarAsistentePersonalId = SCOPE_IDENTITY();

-- Analiticas (Dashboard): nucleo de producto disponible para admin y basico desde el alta.
-- verbalancegeneral se declara ya (Balance General es una pantalla futura todavia sin
-- item de menu propio) para no tener que volver a tocar este bloque cuando se agregue.
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('verdashboardanaliticas', 0);
SET @PermisoVerDashboardAnaliticasId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('verbalancegeneral', 0);
SET @PermisoVerBalanceGeneralId = SCOPE_IDENTITY();

-- Medios de pago y Entidades financieras (back-office): catalogos globales del rol
-- "operador", y admin los tiene desde el arranque (igual que Comercios/Monedas).
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionmediospago', 0);
SET @PermisoGestionMediosPagoId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestionentidadesfinancieras', 0);
SET @PermisoGestionEntidadesFinancierasId = SCOPE_IDENTITY();

-- Cuentas financieras (Finanzas): lista personal, mismo alcance que gestionmovimientos
-- (admin y basico).
INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('gestioncuentasfinancieras', 0);
SET @PermisoGestionCuentasFinancierasId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('admin', 1);
SET @GrupoAdminId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('basico', 1);
SET @GrupoBasicoId = SCOPE_IDENTITY();

INSERT INTO dbo.Permiso (Nombre, EsGrupo) VALUES ('operador', 1);
SET @GrupoOperadorId = SCOPE_IDENTITY();

INSERT INTO dbo.PermisoCompuesto (PermisoPadreId, PermisoHijoId)
VALUES
    (@GrupoAdminId, @PermisoGestionUsuariosId),
    (@GrupoAdminId, @PermisoGestionPermisosId),
    (@GrupoAdminId, @PermisoVerBitacoraId),
    (@GrupoAdminId, @PermisoHistorialUsuariosId),
    (@GrupoAdminId, @PermisoConfiguracionUsuarioId),
    (@GrupoAdminId, @PermisoGestionIdiomasId),
    (@GrupoAdminId, @PermisoGestionMovimientosId),
    (@GrupoAdminId, @PermisoGestionCategoriasId),
    (@GrupoAdminId, @PermisoGestionListaComprasId),
    (@GrupoAdminId, @PermisoGestionBackupId),
    (@GrupoAdminId, @PermisoGestionNotificacionesId),
    (@GrupoAdminId, @PermisoGestionComerciosId),
    (@GrupoAdminId, @PermisoGestionMonedasId),
    (@GrupoAdminId, @PermisoGestionReglasCategorizacionId),
    (@GrupoAdminId, @PermisoUsarAsistentePersonalId),
    (@GrupoAdminId, @PermisoVerDashboardAnaliticasId),
    (@GrupoAdminId, @PermisoVerBalanceGeneralId),
    (@GrupoAdminId, @PermisoGestionMediosPagoId),
    (@GrupoAdminId, @PermisoGestionEntidadesFinancierasId),
    (@GrupoAdminId, @PermisoGestionCuentasFinancierasId),
    (@GrupoBasicoId, @PermisoConfiguracionUsuarioId),
    (@GrupoBasicoId, @PermisoGestionMovimientosId),
    (@GrupoBasicoId, @PermisoGestionCategoriasId),
    (@GrupoBasicoId, @PermisoGestionListaComprasId),
    (@GrupoBasicoId, @PermisoGestionReglasCategorizacionId),
    (@GrupoBasicoId, @PermisoUsarAsistentePersonalId),
    (@GrupoBasicoId, @PermisoVerDashboardAnaliticasId),
    (@GrupoBasicoId, @PermisoVerBalanceGeneralId),
    (@GrupoBasicoId, @PermisoGestionCuentasFinancierasId),
    (@GrupoOperadorId, @PermisoGestionNotificacionesId),
    (@GrupoOperadorId, @PermisoGestionComerciosId),
    (@GrupoOperadorId, @PermisoGestionMonedasId),
    (@GrupoOperadorId, @PermisoGestionMediosPagoId),
    (@GrupoOperadorId, @PermisoGestionEntidadesFinancierasId);

INSERT INTO dbo.Usuarios (Username, PasswordHash, Activo)
VALUES
    ('admin', 'jdLqOYiW5l0Rzxvs2F6C9s+XPP1lpDouX9r4gg8K72K00snMfMzAo84u8OAQP4Hc', 1),
    ('user', 'DDQdLlDvTDqmjKrOv8/nV7l2SmyoVkxqURLPHM1EG8fYYF61cjoTPjh45V3iiql9', 1),
    ('operador', 'AS8qXPgTV1LxO3ERPzBGoPb09ETNny2aNSkTIAGGd1Ytdi9AcGD5WoDKdSxxgp8Z', 1);

SELECT @AdminUserId = Id FROM dbo.Usuarios WHERE Username = 'admin';
SELECT @UsuarioBasicoId = Id FROM dbo.Usuarios WHERE Username = 'user';
SELECT @UsuarioOperadorId = Id FROM dbo.Usuarios WHERE Username = 'operador';
SELECT @GrupoAdminId = Id FROM dbo.Permiso WHERE Nombre = 'admin';
SELECT @GrupoBasicoId = Id FROM dbo.Permiso WHERE Nombre = 'basico';
SELECT @GrupoOperadorId = Id FROM dbo.Permiso WHERE Nombre = 'operador';

INSERT INTO dbo.UsuarioPermiso (UsuarioId, PermisoId)
VALUES
    (@AdminUserId, @GrupoAdminId),
    (@UsuarioBasicoId, @GrupoBasicoId),
    (@UsuarioOperadorId, @GrupoOperadorId);
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

-- NEG01/NEG02: nucleo de negocio de FINAZAPP (movimientos financieros y sus categorias).
CREATE TABLE dbo.Categoria
(
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    UsuarioId INT NULL,
    CONSTRAINT PK_Categoria PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Categoria_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id)
);
GO

-- Unicidad de nombre separada para catalogo global (UsuarioId NULL) vs. personal:
-- UNIQUE normal no permite esta semantica NULL-friendly, por eso son indices filtrados.
-- Los indices filtrados exigen QUOTED_IDENTIFIER ON en la sesion que los crea.
SET QUOTED_IDENTIFIER ON;
GO

CREATE UNIQUE INDEX UQ_Categoria_Nombre_Global
    ON dbo.Categoria (Nombre)
    WHERE UsuarioId IS NULL;
GO

CREATE UNIQUE INDEX UQ_Categoria_Nombre_Usuario
    ON dbo.Categoria (UsuarioId, Nombre)
    WHERE UsuarioId IS NOT NULL;
GO

CREATE TABLE dbo.Movimiento
(
    Id INT IDENTITY(1,1) NOT NULL,
    UsuarioId INT NOT NULL,
    CategoriaId INT NOT NULL,
    Tipo VARCHAR(10) NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Fecha DATE NOT NULL,
    Descripcion VARCHAR(300) NULL,
    FechaCreacion DATETIME NOT NULL CONSTRAINT DF_Movimiento_FechaCreacion DEFAULT (GETDATE()),
    CONSTRAINT PK_Movimiento PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Movimiento_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT FK_Movimiento_Categoria FOREIGN KEY (CategoriaId) REFERENCES dbo.Categoria(Id),
    CONSTRAINT CK_Movimiento_Tipo CHECK (Tipo IN ('Ingreso', 'Egreso')),
    CONSTRAINT CK_Movimiento_Monto CHECK (Monto > 0)
);
GO

CREATE INDEX IX_Movimiento_UsuarioId_Fecha
    ON dbo.Movimiento (UsuarioId, Fecha DESC);
GO

-- Piloto DAL/REPOSITORIO sobre el modelo de dominio de FINAZAPP (G00): EntidadFinanciera
-- y CuentaFinanciera son las primeras clases <<Vision>> en tener persistencia real.
CREATE TABLE dbo.EntidadFinanciera
(
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    CONSTRAINT PK_EntidadFinanciera PRIMARY KEY CLUSTERED (Id)
);
GO

ALTER TABLE dbo.EntidadFinanciera
ADD CONSTRAINT UQ_EntidadFinanciera_Nombre UNIQUE (Nombre);
GO

CREATE TABLE dbo.CuentaFinanciera
(
    Id INT IDENTITY(1,1) NOT NULL,
    UsuarioId INT NOT NULL,
    EntidadFinancieraId INT NOT NULL,
    Alias VARCHAR(100) NOT NULL,
    Tipo VARCHAR(30) NOT NULL,
    CredencialesCifradas VARCHAR(500) NULL,
    CONSTRAINT PK_CuentaFinanciera PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_CuentaFinanciera_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT FK_CuentaFinanciera_EntidadFinanciera FOREIGN KEY (EntidadFinancieraId) REFERENCES dbo.EntidadFinanciera(Id),
    CONSTRAINT CK_CuentaFinanciera_Tipo CHECK (Tipo IN ('BilleteraDigital', 'CuentaCorriente', 'CajaAhorro'))
);
GO

CREATE INDEX IX_CuentaFinanciera_UsuarioId
    ON dbo.CuentaFinanciera (UsuarioId);
GO

-- MedioPago es el catalogo global (Tipo + Marca, ej. "Visa Credito"); MedioPagoEntidad es el
-- producto concreto que ofrece una EntidadFinanciera (ej. Galicia + Visa Credito + Eminent).
-- CuentaFinancieraMedioPagoEntidad (N:M) = subconjunto que el usuario contrato en su cuenta.
CREATE TABLE dbo.MedioPago
(
    Id INT IDENTITY(1,1) NOT NULL,
    Tipo VARCHAR(30) NOT NULL,
    Marca VARCHAR(50) NULL,
    CONSTRAINT PK_MedioPago PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT CK_MedioPago_Tipo CHECK (Tipo IN ('TarjetaCredito', 'TarjetaDebito', 'TarjetaPrepaga', 'CuentaBilleteraVirtual', 'CuentaDNI')),
    CONSTRAINT UQ_MedioPago_Tipo_Marca UNIQUE (Tipo, Marca)
);
GO

CREATE TABLE dbo.MedioPagoEntidad
(
    Id INT IDENTITY(1,1) NOT NULL,
    EntidadFinancieraId INT NOT NULL,
    MedioPagoId INT NOT NULL,
    Segmento VARCHAR(50) NULL,
    CONSTRAINT PK_MedioPagoEntidad PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_MedioPagoEntidad_EntidadFinanciera FOREIGN KEY (EntidadFinancieraId) REFERENCES dbo.EntidadFinanciera(Id),
    CONSTRAINT FK_MedioPagoEntidad_MedioPago FOREIGN KEY (MedioPagoId) REFERENCES dbo.MedioPago(Id),
    CONSTRAINT UQ_MedioPagoEntidad_Entidad_Medio_Segmento UNIQUE (EntidadFinancieraId, MedioPagoId, Segmento)
);
GO

CREATE INDEX IX_MedioPagoEntidad_EntidadFinancieraId
    ON dbo.MedioPagoEntidad (EntidadFinancieraId);
GO

CREATE INDEX IX_MedioPagoEntidad_MedioPagoId
    ON dbo.MedioPagoEntidad (MedioPagoId);
GO

CREATE TABLE dbo.CuentaFinancieraMedioPagoEntidad
(
    CuentaFinancieraId INT NOT NULL,
    MedioPagoEntidadId INT NOT NULL,
    CONSTRAINT PK_CuentaFinancieraMedioPagoEntidad PRIMARY KEY CLUSTERED (CuentaFinancieraId, MedioPagoEntidadId),
    CONSTRAINT FK_CuentaFinancieraMedioPagoEntidad_CuentaFinanciera FOREIGN KEY (CuentaFinancieraId) REFERENCES dbo.CuentaFinanciera(Id),
    CONSTRAINT FK_CuentaFinancieraMedioPagoEntidad_MedioPagoEntidad FOREIGN KEY (MedioPagoEntidadId) REFERENCES dbo.MedioPagoEntidad(Id)
);
GO

-- Cluster Comercio/Producto sobre el modelo de dominio de FINAZAPP (G00).
CREATE TABLE dbo.RubroComercio
(
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    CONSTRAINT PK_RubroComercio PRIMARY KEY CLUSTERED (Id)
);
GO

ALTER TABLE dbo.RubroComercio
ADD CONSTRAINT UQ_RubroComercio_Nombre UNIQUE (Nombre);
GO

CREATE TABLE dbo.CategoriaProducto
(
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    CONSTRAINT PK_CategoriaProducto PRIMARY KEY CLUSTERED (Id)
);
GO

ALTER TABLE dbo.CategoriaProducto
ADD CONSTRAINT UQ_CategoriaProducto_Nombre UNIQUE (Nombre);
GO

-- Prerequisito minimo para Producto.MonedaId (no forma parte de este lote de negocio,
-- solo el catalogo base; TasaConversion queda para el cluster Multi-Moneda).
CREATE TABLE dbo.Moneda
(
    Id INT IDENTITY(1,1) NOT NULL,
    Codigo VARCHAR(10) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    CONSTRAINT PK_Moneda PRIMARY KEY CLUSTERED (Id)
);
GO

ALTER TABLE dbo.Moneda
ADD CONSTRAINT UQ_Moneda_Codigo UNIQUE (Codigo);
GO

CREATE TABLE dbo.Comercio
(
    Id INT IDENTITY(1,1) NOT NULL,
    RubroComercioId INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Direccion VARCHAR(200) NULL,
    Localidad VARCHAR(100) NULL,
    Provincia VARCHAR(100) NULL,
    Latitud DECIMAL(9,6) NULL,
    Longitud DECIMAL(9,6) NULL,
    CONSTRAINT PK_Comercio PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Comercio_RubroComercio FOREIGN KEY (RubroComercioId) REFERENCES dbo.RubroComercio(Id)
);
GO

CREATE INDEX IX_Comercio_RubroComercioId
    ON dbo.Comercio (RubroComercioId);
GO

-- Comercio opcional para Movimiento
ALTER TABLE dbo.Movimiento
ADD ComercioId INT NULL,
    CONSTRAINT FK_Movimiento_Comercio FOREIGN KEY (ComercioId) REFERENCES dbo.Comercio(Id);
GO

CREATE INDEX IX_Movimiento_ComercioId
    ON dbo.Movimiento (ComercioId);
GO

CREATE TABLE dbo.Producto
(
    Id INT IDENTITY(1,1) NOT NULL,
    ComercioId INT NOT NULL,
    CategoriaProductoId INT NOT NULL,
    MonedaId INT NOT NULL,
    Nombre VARCHAR(150) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_Producto PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Producto_Comercio FOREIGN KEY (ComercioId) REFERENCES dbo.Comercio(Id),
    CONSTRAINT FK_Producto_CategoriaProducto FOREIGN KEY (CategoriaProductoId) REFERENCES dbo.CategoriaProducto(Id),
    CONSTRAINT FK_Producto_Moneda FOREIGN KEY (MonedaId) REFERENCES dbo.Moneda(Id),
    CONSTRAINT CK_Producto_Precio CHECK (Precio > 0)
);
GO

CREATE INDEX IX_Producto_ComercioId
    ON dbo.Producto (ComercioId);
GO

-- Promocion sobre el modelo de dominio de FINAZAPP (G00). Las relaciones de Promocion son
-- todas N:M (aplica a uno o varios medios de pago de entidad / comercios / rubros / dias de
-- semana), resueltas con tablas intermedias sin columna Id propia (mismo patron que
-- UsuarioPermiso). PromocionDiaSemana sin filas = la promocion aplica todos los dias.
CREATE TABLE dbo.Promocion
(
    Id INT IDENTITY(1,1) NOT NULL,
    Descripcion VARCHAR(300) NOT NULL,
    Descuento DECIMAL(5,2) NOT NULL,
    TopeDescuento DECIMAL(10,2) NULL,
    VigenciaDesde DATE NOT NULL,
    VigenciaHasta DATE NOT NULL,
    CONSTRAINT PK_Promocion PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT CK_Promocion_Descuento CHECK (Descuento > 0 AND Descuento <= 100),
    CONSTRAINT CK_Promocion_TopeDescuento CHECK (TopeDescuento IS NULL OR TopeDescuento > 0),
    CONSTRAINT CK_Promocion_Vigencia CHECK (VigenciaHasta >= VigenciaDesde)
);
GO

CREATE TABLE dbo.PromocionDiaSemana
(
    PromocionId INT NOT NULL,
    DiaSemana VARCHAR(15) NOT NULL,
    CONSTRAINT PK_PromocionDiaSemana PRIMARY KEY CLUSTERED (PromocionId, DiaSemana),
    CONSTRAINT FK_PromocionDiaSemana_Promocion FOREIGN KEY (PromocionId) REFERENCES dbo.Promocion(Id),
    CONSTRAINT CK_PromocionDiaSemana_DiaSemana CHECK (DiaSemana IN ('Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado', 'Domingo'))
);
GO

CREATE TABLE dbo.PromocionMedioPagoEntidad
(
    PromocionId INT NOT NULL,
    MedioPagoEntidadId INT NOT NULL,
    CONSTRAINT PK_PromocionMedioPagoEntidad PRIMARY KEY CLUSTERED (PromocionId, MedioPagoEntidadId),
    CONSTRAINT FK_PromocionMedioPagoEntidad_Promocion FOREIGN KEY (PromocionId) REFERENCES dbo.Promocion(Id),
    CONSTRAINT FK_PromocionMedioPagoEntidad_MedioPagoEntidad FOREIGN KEY (MedioPagoEntidadId) REFERENCES dbo.MedioPagoEntidad(Id)
);
GO

CREATE TABLE dbo.PromocionComercio
(
    PromocionId INT NOT NULL,
    ComercioId INT NOT NULL,
    CONSTRAINT PK_PromocionComercio PRIMARY KEY CLUSTERED (PromocionId, ComercioId),
    CONSTRAINT FK_PromocionComercio_Promocion FOREIGN KEY (PromocionId) REFERENCES dbo.Promocion(Id),
    CONSTRAINT FK_PromocionComercio_Comercio FOREIGN KEY (ComercioId) REFERENCES dbo.Comercio(Id)
);
GO

CREATE TABLE dbo.PromocionRubroComercio
(
    PromocionId INT NOT NULL,
    RubroComercioId INT NOT NULL,
    CONSTRAINT PK_PromocionRubroComercio PRIMARY KEY CLUSTERED (PromocionId, RubroComercioId),
    CONSTRAINT FK_PromocionRubroComercio_Promocion FOREIGN KEY (PromocionId) REFERENCES dbo.Promocion(Id),
    CONSTRAINT FK_PromocionRubroComercio_RubroComercio FOREIGN KEY (RubroComercioId) REFERENCES dbo.RubroComercio(Id)
);
GO

-- ListaDeCompras/ItemLista y Notificacion sobre el modelo de dominio de FINAZAPP (G00).
-- Notificacion, RecordatorioCompra y RecordatorioInversion (herencia real en BE) se
-- persisten con Table Per Hierarchy: una sola tabla Notificacion, columnas de subclase
-- nullable, discriminadas por TipoNotificacion.
CREATE TABLE dbo.ListaDeCompras
(
    Id INT IDENTITY(1,1) NOT NULL,
    UsuarioId INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    FechaCreacion DATETIME NOT NULL CONSTRAINT DF_ListaDeCompras_FechaCreacion DEFAULT (GETDATE()),
    CONSTRAINT PK_ListaDeCompras PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_ListaDeCompras_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id)
);
GO

CREATE INDEX IX_ListaDeCompras_UsuarioId
    ON dbo.ListaDeCompras (UsuarioId);
GO

-- ItemLista.Descripcion es texto libre tipeado por el usuario (ej. "Leche descremada 1L");
-- el mapeo futuro ItemLista -> Producto (para asignarle promocion) queda fuera de alcance
-- de este lote y no se modela aqui.
CREATE TABLE dbo.ItemLista
(
    Id INT IDENTITY(1,1) NOT NULL,
    ListaDeComprasId INT NOT NULL,
    Descripcion VARCHAR(200) NOT NULL,
    Cantidad INT NOT NULL,
    Comprado BIT NOT NULL CONSTRAINT DF_ItemLista_Comprado DEFAULT (0),
    CONSTRAINT PK_ItemLista PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_ItemLista_ListaDeCompras FOREIGN KEY (ListaDeComprasId) REFERENCES dbo.ListaDeCompras(Id),
    CONSTRAINT CK_ItemLista_Cantidad CHECK (Cantidad > 0)
);
GO

CREATE INDEX IX_ItemLista_ListaDeComprasId
    ON dbo.ItemLista (ListaDeComprasId);
GO

-- TipoNotificacion es el discriminador de la jerarquia Notificacion / RecordatorioCompra /
-- RecordatorioInversion (Table Per Hierarchy: una sola tabla, columnas de subclase nullable).
CREATE TABLE dbo.Notificacion
(
    Id INT IDENTITY(1,1) NOT NULL,
    UsuarioId INT NOT NULL,
    TipoNotificacion VARCHAR(30) NOT NULL,
    Mensaje VARCHAR(300) NOT NULL,
    Fecha DATETIME NOT NULL CONSTRAINT DF_Notificacion_Fecha DEFAULT (GETDATE()),
    Leida BIT NOT NULL CONSTRAINT DF_Notificacion_Leida DEFAULT (0),
    ItemListaId INT NULL,
    PromocionId INT NULL,
    FechaVencimiento DATE NULL,
    CONSTRAINT PK_Notificacion PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Notificacion_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT FK_Notificacion_ItemLista FOREIGN KEY (ItemListaId) REFERENCES dbo.ItemLista(Id),
    CONSTRAINT FK_Notificacion_Promocion FOREIGN KEY (PromocionId) REFERENCES dbo.Promocion(Id),
    CONSTRAINT CK_Notificacion_Tipo CHECK (TipoNotificacion IN ('Notificacion', 'RecordatorioCompra', 'RecordatorioInversion')),
    CONSTRAINT CK_Notificacion_RecordatorioCompra_Campos CHECK (
        (TipoNotificacion <> 'RecordatorioCompra') OR (ItemListaId IS NOT NULL AND FechaVencimiento IS NOT NULL)
    )
);
GO

CREATE INDEX IX_Notificacion_UsuarioId
    ON dbo.Notificacion (UsuarioId);
GO

-- ReglaCategorizacion sobre el modelo de dominio de FINAZAPP (G00). Ultima tabla
-- intermedia N:M del modelo: una regla puede disparar en uno o varios comercios.
CREATE TABLE dbo.ReglaCategorizacion
(
    Id INT IDENTITY(1,1) NOT NULL,
    UsuarioId INT NOT NULL,
    CategoriaId INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    PalabrasClave VARCHAR(300) NULL,
    CONSTRAINT PK_ReglaCategorizacion PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_ReglaCategorizacion_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT FK_ReglaCategorizacion_Categoria FOREIGN KEY (CategoriaId) REFERENCES dbo.Categoria(Id)
);
GO

CREATE INDEX IX_ReglaCategorizacion_UsuarioId
    ON dbo.ReglaCategorizacion (UsuarioId);
GO

CREATE TABLE dbo.ComercioReglaCategorizacion
(
    ComercioId INT NOT NULL,
    ReglaCategorizacionId INT NOT NULL,
    CONSTRAINT PK_ComercioReglaCategorizacion PRIMARY KEY CLUSTERED (ComercioId, ReglaCategorizacionId),
    CONSTRAINT FK_ComercioReglaCategorizacion_Comercio FOREIGN KEY (ComercioId) REFERENCES dbo.Comercio(Id),
    CONSTRAINT FK_ComercioReglaCategorizacion_ReglaCategorizacion FOREIGN KEY (ReglaCategorizacionId) REFERENCES dbo.ReglaCategorizacion(Id)
);
GO

-- ChatbotFinanciero/ConsultaChatbot sobre el modelo de dominio de FINAZAPP (G00),
-- paquete "Inteligencia Artificial": de las 4 clases <<Vision>> solo se implementan
-- estas 2 (las que tienen asociacion real con el chat). AgenteIA y SugerenciaFinanciera
-- quedan fuera de alcance (recomendaciones financieras, iteracion futura).
CREATE TABLE dbo.ChatbotFinanciero
(
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    CONSTRAINT PK_ChatbotFinanciero PRIMARY KEY CLUSTERED (Id)
);
GO

-- Una fila = un intercambio completo (pregunta + respuesta ya resuelta), insertado
-- una unica vez cuando ya se tiene la respuesta de Gemini. EsPregunta queda reservado
-- a futuro para entradas proactivas del bot sin pregunta de usuario (siempre 1 por ahora).
CREATE TABLE dbo.ConsultaChatbot
(
    Id INT IDENTITY(1,1) NOT NULL,
    ChatbotFinancieroId INT NOT NULL,
    UsuarioId INT NOT NULL,
    Pregunta VARCHAR(MAX) NOT NULL,
    Respuesta VARCHAR(MAX) NOT NULL,
    Fecha DATETIME NOT NULL,
    EsPregunta BIT NOT NULL CONSTRAINT DF_ConsultaChatbot_EsPregunta DEFAULT (1),
    CONSTRAINT PK_ConsultaChatbot PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_ConsultaChatbot_ChatbotFinanciero FOREIGN KEY (ChatbotFinancieroId) REFERENCES dbo.ChatbotFinanciero(Id),
    CONSTRAINT FK_ConsultaChatbot_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id)
);
GO

CREATE INDEX IX_ConsultaChatbot_UsuarioId_Fecha
    ON dbo.ConsultaChatbot (UsuarioId, Fecha);
GO

-- Fila unica sembrada: no hay alta/baja de chatbots desde la UI, solo lectura.
INSERT INTO dbo.ChatbotFinanciero (Nombre) VALUES (N'Asistente Financiero FINAZAPP');
GO

-- Categorias globales por defecto (UsuarioId NULL), visibles para todos los usuarios.
INSERT INTO dbo.Categoria (Nombre, UsuarioId)
VALUES
    ('Alimentacion', NULL),
    ('Transporte', NULL),
    ('Vivienda', NULL),
    ('Salud', NULL),
    ('Entretenimiento', NULL),
    ('Sueldo', NULL),
    ('Otros Ingresos', NULL),
    ('Otros Egresos', NULL);
GO

-- Dashboard y TarjetaDashboard sobre el modelo de dominio de FINAZAPP (G00). Un usuario
-- tiene a lo sumo un Dashboard (0..1 en el diagrama, de ahi el UNIQUE en UsuarioId);
-- Dashboard *-- TarjetaDashboard es composicion (una tarjeta no existe sin su dashboard).
CREATE TABLE dbo.Dashboard
(
    Id INT IDENTITY(1,1) NOT NULL,
    UsuarioId INT NOT NULL,
    CONSTRAINT PK_Dashboard PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Dashboard_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT UQ_Dashboard_UsuarioId UNIQUE (UsuarioId)
);
GO

CREATE TABLE dbo.TarjetaDashboard
(
    Id INT IDENTITY(1,1) NOT NULL,
    DashboardId INT NOT NULL,
    Tipo VARCHAR(30) NOT NULL,
    Orden INT NOT NULL,
    CONSTRAINT PK_TarjetaDashboard PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_TarjetaDashboard_Dashboard FOREIGN KEY (DashboardId) REFERENCES dbo.Dashboard(Id),
    CONSTRAINT CK_TarjetaDashboard_Tipo CHECK (Tipo IN ('GastosPorCategoria', 'IngresosPorCategoria', 'ComparativoMensual', 'EvolucionBalance')),
    CONSTRAINT UQ_TarjetaDashboard_DashboardId_Tipo UNIQUE (DashboardId, Tipo)
);
GO

CREATE INDEX IX_TarjetaDashboard_DashboardId
    ON dbo.TarjetaDashboard (DashboardId);
GO

-- TasaConversion sobre el modelo de dominio de FINAZAPP (G00). Primera entidad con 2 FK
-- distintas a la misma tabla (Moneda): moneda base y moneda de conversion.
CREATE TABLE dbo.TasaConversion
(
    Id INT IDENTITY(1,1) NOT NULL,
    MonedaBaseId INT NOT NULL,
    MonedaConversionId INT NOT NULL,
    CalculoConversion DECIMAL(18,6) NOT NULL,
    FechaActualizacion DATETIME NOT NULL CONSTRAINT DF_TasaConversion_FechaActualizacion DEFAULT (GETDATE()),
    CONSTRAINT PK_TasaConversion PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_TasaConversion_MonedaBase FOREIGN KEY (MonedaBaseId) REFERENCES dbo.Moneda(Id),
    CONSTRAINT FK_TasaConversion_MonedaConversion FOREIGN KEY (MonedaConversionId) REFERENCES dbo.Moneda(Id),
    CONSTRAINT CK_TasaConversion_MonedasDistintas CHECK (MonedaBaseId <> MonedaConversionId),
    CONSTRAINT CK_TasaConversion_Calculo CHECK (CalculoConversion > 0)
);
GO

CREATE UNIQUE INDEX UQ_TasaConversion_Par
    ON dbo.TasaConversion (MonedaBaseId, MonedaConversionId);
GO

IF EXISTS (SELECT 1 FROM dbo.Moneda WHERE Codigo = 'ARS')
    UPDATE dbo.Moneda SET Nombre = N'Peso Argentino' WHERE Codigo = 'ARS';
ELSE
    INSERT INTO dbo.Moneda (Codigo, Nombre) VALUES ('ARS', N'Peso Argentino');

IF EXISTS (SELECT 1 FROM dbo.Moneda WHERE Codigo = 'USD')
    UPDATE dbo.Moneda SET Nombre = N'Dólar Estadounidense' WHERE Codigo = 'USD';
ELSE
    INSERT INTO dbo.Moneda (Codigo, Nombre) VALUES ('USD', N'Dólar Estadounidense');
GO

DECLARE @MonedaUsdId INT = (SELECT Id FROM dbo.Moneda WHERE Codigo = 'USD');
DECLARE @MonedaArsId INT = (SELECT Id FROM dbo.Moneda WHERE Codigo = 'ARS');
DECLARE @FechaTasaSeed DATETIME = CONVERT(DATETIME, '20260920', 112);

IF EXISTS
(
    SELECT 1
    FROM dbo.TasaConversion
    WHERE MonedaBaseId = @MonedaUsdId
      AND MonedaConversionId = @MonedaArsId
)
    UPDATE dbo.TasaConversion
    SET CalculoConversion = 1511.2481,
        FechaActualizacion = @FechaTasaSeed
    WHERE MonedaBaseId = @MonedaUsdId
      AND MonedaConversionId = @MonedaArsId;
ELSE
    INSERT INTO dbo.TasaConversion
    (
        MonedaBaseId,
        MonedaConversionId,
        CalculoConversion,
        FechaActualizacion
    )
    VALUES
    (
        @MonedaUsdId,
        @MonedaArsId,
        1511.2481,
        @FechaTasaSeed
    );
GO

-- BEGIN AUTO-GENERATED I18N BASE SEED
-- AUTO-GENERATED BY .agents/skills/translation-base-seed-sync/scripts/sync-i18n-base-seed.ps1
-- SOURCE OF TRUTH: translation-seeds/base.es.json
-- Si ejecuta desde SSMS en SQLCMD Mode, cambie RepoRoot por la ruta absoluta del repo en su maquina.
-- Con setup-database.ps1 deje RepoRoot = . : el wrapper lo reemplaza solo.
:setvar RepoRoot "."
-- Formas soportadas de ejecutar este script: SSMS 19/22 en SQLCMD Mode, o .\setup-database.ps1 desde la raiz del repo.
-- OJO: NO ejecutarlo directo con sqlcmd ODBC 15.x: al llegar a la linea !! corta en silencio (exit 0) y la BD queda sin traducciones.
-- El helper prepara una copia publica UTF-16LE del JSON para que OPENROWSET preserve Unicode y no dependa de permisos sobre el perfil del usuario.
!! powershell -NoProfile -ExecutionPolicy Bypass -File "$(RepoRoot)\.agents\skills\translation-base-seed-sync\scripts\emit-seed-json-sqlcmd-path.ps1" -RepoRoot "$(RepoRoot)" -OutputFile "NUL"
GO
DECLARE @IdiomaBaseId INT;
INSERT INTO dbo.Idioma (Nombre, [Default]) VALUES (N'Español', 1);
SET @IdiomaBaseId = SCOPE_IDENTITY();

DECLARE @RutaTraduccionesBase NVARCHAR(4000) = N'C:\Users\Public\Documents\TpIDS\SqlSeed\base.es.json';
DECLARE @TraduccionesBaseJson NVARCHAR(MAX);
DECLARE @SqlCargaJson NVARCHAR(MAX);

SET @SqlCargaJson = N'SELECT @JsonOut = BulkColumn
FROM OPENROWSET(
    BULK ''' + REPLACE(@RutaTraduccionesBase, '''', '''''') + ''',
    SINGLE_NCLOB
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
    Valor NVARCHAR(300) NOT NULL
);

INSERT INTO @Seed ([Key], Valor)
SELECT [key], TRY_CAST([value] AS NVARCHAR(300))
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
