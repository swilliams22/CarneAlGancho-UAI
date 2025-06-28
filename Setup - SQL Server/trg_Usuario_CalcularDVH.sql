-- 1: TRIGGER 
CREATE TRIGGER trg_Usuario_CalcularDVH
  ON Usuario
  AFTER INSERT, UPDATE, DELETE
AS
BEGIN
  SET NOCOUNT ON;

  -- Recalcula DVH para las filas insertadas o modificadas
  UPDATE U
     SET DVH = dbo.CalcularDVH(
                  CAST(U.Id AS VARCHAR(36))
                + U.Usuario
                + U.Password
                + CAST(U.Retries AS VARCHAR)
                + CAST(CASE WHEN U.Active=1 THEN '1' ELSE '0' END AS VARCHAR)
				+ U.Salt
               )
  FROM Usuarios U
  JOIN (
    SELECT Id FROM inserted
    UNION
    SELECT Id FROM deleted
  ) x ON U.Id = x.Id;
END;