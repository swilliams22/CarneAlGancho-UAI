USE [CarneAlGancho]
GO
/****** Object:  StoredProcedure [dbo].[SP_RecordDVH]    Script Date: 25/05/2025 22:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_RecordDVH]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentDVV VARCHAR(64);
    DECLARE @TableName VARCHAR(100) = 'Usuario'; -- La tabla de usuarios
	DECLARE @IsAppActive BIT;

	-- **Paso clave: Verificar el estado de la aplicación**
    SELECT @IsAppActive = IsActive FROM AppStatus;

    -- **Solo proceder si la aplicación está activa**
    IF @IsAppActive = 1 
    BEGIN
        -- Paso 1: Concatenar todos los DVH (hashes por fila) de la tabla Usuario y calcular el DVH (hash horizontal)
        SELECT @CurrentDVV = CONVERT(VARCHAR(64),
                                     HASHBYTES('SHA2_256', STRING_AGG(DVH, '') WITHIN GROUP (ORDER BY Id ASC)),
                                     2)
        FROM Usuario;

        -- Manejar el caso de una tabla vacía
        IF @CurrentDVV IS NULL AND (SELECT COUNT(*) FROM Usuario) = 0
        BEGIN
            SET @CurrentDVV = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ''), 2);
        END;

        -- Paso 2: Guardar el DVH recién calculado en la tabla de auditoría (Audit.DVV_Tables)
        -- Esto siempre actualizará/insertará el DVH, asumiendo que la tabla está en un estado consistente al cerrar.
        IF EXISTS (SELECT 1 FROM DVV_Tables WHERE table_name = @TableName) 
        BEGIN
             UPDATE DVV_Tables SET table_DVV = @CurrentDVV, time = GETDATE() WHERE table_name = @TableName;
        PRINT 'DVV para la tabla ' + @TableName + ' actualizado al cierre.';
        END
        ELSE
        BEGIN
            INSERT INTO DVV_Tables (table_name, table_DVV, time) VALUES (@TableName, @CurrentDVV, GETDATE());
			PRINT 'DVV para la tabla ' + @TableName + ' insertado al cierre.';
        END;
    END
    ELSE
    BEGIN
        -- Si la aplicación no está activa, no se recalcula ni se guarda el DVH.
        PRINT 'Omitting DVH recording for ' + @TableName + ' because application is inactive. An administrator must resolve the issue and reactivate the app.';
        -- Opcional: Podrías registrar este evento en Audit.SecurityIncidents si lo consideras relevante,
        -- para tener un log de cuándo se omitió el registro del DVH.
        -- INSERT INTO Audit.SecurityIncidents (IncidentDate, TableName, EventType, Description)
        -- VALUES (GETDATE(), @TableName, 'DVHOmittedOnInactiveApp', 'Registro de DVH omitido al cierre porque la aplicación está inactiva.');
    END;

    -- Esta SP no devuelve un valor específico de éxito/fallo ya que su rol es registrar, no verificar/alertar.
    -- Si falla la ejecución de la SP misma (ej. error de DB), se lanzará una excepción a la aplicación.
END;





  