SELECT * FROM Usuario;

SELECT * FROM DVV_Tables;

SELECT * from appstatus;

-- Stored para calcular el DVV actual y comprar contra el nuevo
exec SP_CalculateAndVerifyUsersDVV

-- Stored para actualizar los DVV al cerrar la app
exec SP_RecordDVH

-- Stored que forza a actualizar los DVV en caso de fallo de integridad
exec SP_Admin_ForceDVV

-- TABLA DE ESTADO DE LA APP
drop table appstatus;

CREATE TABLE AppStatus (
    AppName VARCHAR(100) NOT NULL PRIMARY KEY, -- Podría ser 'MainApp' o 'UsuariosApp'
    IsActive BIT NOT NULL DEFAULT 1,           -- 1 = Activa, 0 = Inactiva (por problemas de integridad)
    LastUpdated DATETIME DEFAULT GETDATE(),    -- Fecha de la última actualización del estado
    Message NVARCHAR(MAX)                      -- Mensaje opcional sobre el estado
);
GO

-- Insertar el estado inicial de la aplicación (activa por defecto)
INSERT INTO AppStatus (AppName, IsActive, Message)
VALUES ('CarneAlGancho', 1, 'Aplicación activa y operativa.');