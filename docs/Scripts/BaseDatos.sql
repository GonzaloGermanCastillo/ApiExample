/*USE master;
GO

CREATE DATABASE Devsu ON
(NAME = Sales_dat,
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Devsu.mdf',
    SIZE = 10,
    MAXSIZE = 50,
    FILEGROWTH = 5)
LOG ON
(NAME = Devsu_log,
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Devsu.ldf',
    SIZE = 5 MB,
    MAXSIZE = 25 MB,
    FILEGROWTH = 5 MB);
GO*/

USE Devsu;
GO

BEGIN TRANSACTION CreateTables
/* Table: Personas */
CREATE TABLE [dbo].[Personas](
	[Id] [uniqueidentifier] NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Genero] [int] NULL,
	[Edad] [int] NULL,
	[Identificacion] [varchar](50) NULL,
	[Direccion] [varchar](150) NOT NULL,
	[Telefono] [varchar](20) NOT NULL,
 CONSTRAINT [PK_Personas] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Personas_Nombre] ON [dbo].[Personas]([Nombre] ASC) ON [PRIMARY]
GO

/* Table: Clientes */
CREATE TABLE [dbo].[Clientes](
	[Id] [uniqueidentifier] NOT NULL,
	[Contrasenia] [int] NOT NULL,
	[Estado] [bit] NOT NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Clientes] ADD  CONSTRAINT [DF_Cliente_Estado]  DEFAULT ((1)) FOR [Estado]
GO

ALTER TABLE [dbo].[Clientes]  WITH CHECK ADD  CONSTRAINT [FK_Cliente_Persona] FOREIGN KEY([Id])
REFERENCES [dbo].[Personas] ([Id])
GO

ALTER TABLE [dbo].[Clientes] CHECK CONSTRAINT [FK_Cliente_Persona]
GO

/* Table: Cuentas */

CREATE TABLE [dbo].[Cuentas](
	[Id] [uniqueidentifier] NOT NULL,
	[Numero] [int] NOT NULL,
	[TipoCuenta] [int] NOT NULL,
	[SaldoInicial] [float] NOT NULL,
	[Estado] [bit] NOT NULL,
	[ClienteId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Cuentas] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Cuentas] ADD  CONSTRAINT [DF_Cuenta_SaldoInicial]  DEFAULT ((0)) FOR [SaldoInicial]
GO

ALTER TABLE [dbo].[Cuentas] ADD  CONSTRAINT [DF_Cuenta_Estado]  DEFAULT ((1)) FOR [Estado]
GO

ALTER TABLE [dbo].[Cuentas]  WITH CHECK ADD  CONSTRAINT [FK_Cuenta_Cliente] FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Clientes] ([Id])
GO

ALTER TABLE [dbo].[Cuentas] CHECK CONSTRAINT [FK_Cuenta_Cliente]
GO

CREATE NONCLUSTERED INDEX [FK_Cuentas_ClienteId] ON [dbo].[Cuentas]([ClienteId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Cuentas_ClienteNumero] ON [dbo].[Cuentas](
	[Id] ASC,
	[Numero] ASC
)
GO

/* Table: Movimientos */

CREATE TABLE [dbo].[Movimientos](
	[Id] [uniqueidentifier] NOT NULL,
	[Fecha] [datetime2](7) NOT NULL,
	[TipoMovimiento] [int] NOT NULL,
	[Valor] [float] NOT NULL,
	[Saldo] [float] NOT NULL,
	[CuentaId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Movimientos] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Movimientos]  WITH CHECK ADD  CONSTRAINT [FK_Movimiento_Cuenta] FOREIGN KEY([CuentaId])
REFERENCES [dbo].[Cuentas] ([Id])
GO

ALTER TABLE [dbo].[Movimientos] CHECK CONSTRAINT [FK_Movimiento_Cuenta]
GO

CREATE NONCLUSTERED INDEX [FK_Movimientos_CuentaId] ON [dbo].[Movimientos](	[CuentaId] ASC)
GO

COMMIT TRANSACTION CreateTables

BEGIN TRANSACTION PopulateTables

INSERT INTO [dbo].[Personas]
	([Id], [Nombre], [Genero], [Edad], [Identificacion], [Direccion], [Telefono])
VALUES
	('AD297609-4006-40FB-9C1D-DBF809F63A8A', 'Gonzalo', 0, 38, 'A4254', 'Av Siempreviva 123', '+541154254785')
GO

INSERT INTO [dbo].[Clientes]
	([Id], [Contrasenia], [Estado])
VALUES
	('AD297609-4006-40FB-9C1D-DBF809F63A8A', 1452, 1)
GO

INSERT INTO [dbo].[Cuentas]
	([Id], [Numero], [TipoCuenta], [SaldoInicial], [Estado], [ClienteId])
VALUES
	('0DC24BF6-7188-4E35-91A5-240020A1933F', 1, 0, 0, 1, 'AD297609-4006-40FB-9C1D-DBF809F63A8A')    
GO

INSERT INTO [dbo].[Movimientos]
	([Id], [Fecha], [TipoMovimiento], [Valor], [Saldo],[CuentaId])
VALUES
	('03745A12-8912-4199-BA29-065CF0BD1A75', '2023-08-15 12:28:10.2410254',	0,	10000,	10200, '0DC24BF6-7188-4E35-91A5-240020A1933F'),
	('B5801A9A-DD5D-40EB-836A-11A475AB1C34', '2023-08-15 12:18:16.9825649',	0,	100,      100, '0DC24BF6-7188-4E35-91A5-240020A1933F'),
	('BB551A78-6B56-47C4-A62B-3C90307CCF61', '2023-08-15 12:20:43.6760508',	0,	100,      200, '0DC24BF6-7188-4E35-91A5-240020A1933F'),
	('A6CDAB7A-4F32-45E1-959F-E9FE9A2901E6', '2023-08-15 12:28:37.8233688',	1,	350,     9850, '0DC24BF6-7188-4E35-91A5-240020A1933F')

COMMIT TRANSACTION PopulateTables