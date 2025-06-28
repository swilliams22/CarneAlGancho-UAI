USE [CarneAlGancho]
GO
/****** Object:  StoredProcedure [dbo].[SP_CalculateAndVerifyUsersDVV]    Script Date: 25/05/2025 22:49:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- 2: Stored para verificar DVV
ALTER   PROCEDURE [dbo].[SP_CalculateAndVerifyUsersDVV]
AS
BEGIN
    SET NOCOUNT ON; -- Mejora el rendimiento al suprimir el conteo de filas afectadas.

    DECLARE @CurrentDVV VARCHAR(64); -- Esta variable almacenar� el hash horizontal (DVH) calculado
    DECLARE @LastSavedDVV VARCHAR(64); -- Esta variable almacenar� el �ltimo DVH guardado
    DECLARE @TableName VARCHAR(100) = 'Usuario'; -- Nombre de la tabla de usuarios a auditar
    DECLARE @ErrorMessage NVARCHAR(MAX);

    -- Paso 1: Concatenar todos los DVH (hashes por fila) de la tabla Usuario y calcular el DVH (hash horizontal)
    -- La columna 'DVH' en tu tabla 'Usuario' es el hash por fila.
    SELECT @CurrentDVV = CONVERT(VARCHAR(64),
                                 HASHBYTES('SHA2_256', STRING_AGG(DVH, '') WITHIN GROUP (ORDER BY Id ASC)),
                                 2) -- '2' para formato hexadecimal sin prefijo 0x
    FROM Usuario;

    -- Manejar el caso de una tabla vac�a: el DVH de una tabla vac�a es el hash de una cadena vac�a.
    IF @CurrentDVV IS NULL AND (SELECT COUNT(*) FROM Usuario) = 0
    BEGIN
        SET @CurrentDVV = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', ''), 2);
    END;

    -- Paso 2: Recuperar el �ltimo DVH (hash horizontal) guardado de la tabla de auditor�a 'Audit.DVV_Tables'
    SELECT @LastSavedDVV = table_DVV -- La columna 'table_DVV' en tu tabla de auditor�a almacena el DVH
    FROM DVV_Tables
    WHERE table_name = @TableName;

    -- Paso 3: Comparar y Registrar/Alertar
    IF @CurrentDVV = @LastSavedDVV
    BEGIN
        -- Integridad OK
        PRINT 'DVV para la tabla ' + @TableName + ' coincide. Integridad OK.';
        
        -- Actualizar la fecha de �ltima verificaci�n en la tabla de auditor�a
        -- UPDATE DVV_Tables SET time = GETDATE() WHERE table_name = @TableName;
        
        RETURN 1; -- Devuelve 1 para indicar �xito
    END
    ELSE
    BEGIN
        -- �ALERTA! La integridad ha sido comprometida
        PRINT '�ALERTA DE INTEGRIDAD! El DVH para la tabla ' + @TableName + ' NO COINCIDE.';
        PRINT 'DVV Actual: ' + ISNULL(@CurrentDVV, 'NULL') + ', DVH Previo: ' + ISNULL(@LastSavedDVV, 'NULL');

		 -- Construir el mensaje de error para la tabla AppStatus
        SET @ErrorMessage = 'Aplicaci�n inactiva: Inconsistencia de integridad detectada en la tabla ' + @TableName + '.'
                          + ' DVV actual: ' + ISNULL(@CurrentDVV, '[NULO ACTUAL]')
                          + ', DVV previo guardado: ' + ISNULL(@LastSavedDVV, '[NULO GUARDADO]');

        -- **ACCI�N CLAVE: Desactivar la aplicaci�n y actualizar el mensaje en Audit.AppStatus**
        UPDATE AppStatus
        SET IsActive = 0,
            LastUpdated = GETDATE(),
            Message = @ErrorMessage;

        RETURN 0; -- Devuelve 0 para indicar fallo
    END;

END;
