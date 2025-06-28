USE [CarneAlGancho]
GO
/****** Object:  StoredProcedure [dbo].[SP_Admin_ForceDVV]    Script Date: 25/05/2025 22:48:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Admin_ForceDVV]
    @AdminMessage NVARCHAR(MAX) = 'Integridad resuelta y aplicación activada por administrador.'
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Iniciar la transacción para asegurar que ambas operaciones (DVH y estado de app) sean atómicas
        BEGIN TRANSACTION;

		UPDATE AppStatus
        SET IsActive = 1,
            LastUpdated = GETDATE(),
            Message = @AdminMessage;

        -- Forzar el recálculo y registro del DVH (llamando a tu SP_RecordDVH)
        EXEC dbo.SP_RecordDVH;
        
        -- Si todo fue bien, confirmar la transacción
        COMMIT TRANSACTION;
        PRINT 'Proceso de resolución de integridad y activación de aplicación completado con éxito.';

    END TRY
    BEGIN CATCH
        -- Si ocurre algún error, revertir la transacción
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

    END CATCH
END;
