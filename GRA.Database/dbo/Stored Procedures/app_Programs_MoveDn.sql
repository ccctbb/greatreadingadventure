﻿
CREATE PROCEDURE [dbo].[app_Programs_MoveDn] @PID INT
AS
DECLARE @TenID INT

SELECT @TenID = TenID
FROM Programs
WHERE PID = @PID

EXEC [dbo].[app_Programs_Reorder] @TenID

DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT

SELECT @CurrentRecordLocation = POrder
FROM Programs
WHERE PID = @PID

IF @CurrentRecordLocation < (
		SELECT MAX(POrder)
		FROM Programs
		WHERE TenID = @TenID
		)
BEGIN
	SELECT @NextRecordID = PID
	FROM Programs
	WHERE POrder = (@CurrentRecordLocation + 1)
		AND TenID = @TenID

	UPDATE Programs
	SET POrder = @CurrentRecordLocation + 1
	WHERE PID = @PID

	UPDATE Programs
	SET POrder = @CurrentRecordLocation
	WHERE PID = @NextRecordID
END
