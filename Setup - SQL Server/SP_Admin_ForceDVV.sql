USE [CarneAlGancho]
GO
/****** Object:  StoredProcedure [dbo].[SP_Admin_ForceDVV]    Script Date: 25/05/2025 22:48:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Admin_ForceDVV]
    @AdminMessage NVARCHAR(MAX) = 'Integridad resuelta y aplicaci�n activada por administrador.'
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Iniciar la transacci�n para asegurar que ambas operaciones (DVH y estado de app) sean at�micas
        BEGIN TRANSACTION;

		UPDATE AppStatus
        SET IsActive = 1,
            LastUpdated = GETDATE(),
            Message = @AdminMessage;

        -- Forzar el rec�lculo y registro del DVH (llamando a tu SP_RecordDVH)
        EXEC dbo.SP_RecordDVH;
        
        -- Si todo fue bien, confirmar la transacci�n
        COMMIT TRANSACTION;
        PRINT 'Proceso de resoluci�n de integridad y activaci�n de aplicaci�n completado con �xito.';

    END TRY
    BEGIN CATCH
        -- Si ocurre alg�n error, revertir la transacci�n
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

    END CATCH
END;
