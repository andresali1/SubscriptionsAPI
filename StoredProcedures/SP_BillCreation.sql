Use WebApiAuthors
GO

CREATE PROCEDURE BillCreation(
	@BeginDate DATETIME,
	@EndDate DATETIME
)
AS
BEGIN
	DECLARE @PricePerRequest DECIMAL(4, 4) = 1.0/2; -- 1 Dolar per each 2 requests (For Testing)

	INSERT INTO Bill (UserId, Price, GenerationDate, Paid, PaymentDate)
	SELECT K.UserId, COUNT(*) * @PricePerRequest AS Price,
	       GETDATE() AS GenerationDate, 0 AS Paid,
		   DATEADD(d, 60, GETDATE()) AS PaymentDate
	FROM Request R
	INNER JOIN APIKey K ON K.Id = R.KeyId
	WHERE K.KeyType != 1
	AND R.RequestDate >= @BeginDate
	AND R.RequestDate < @EndDate
	GROUP BY K.UserId
	
	INSERT INTO GeneratedBill (Month, Year)
	SELECT
		CASE MONTH(GETDATE())
			WHEN 1 THEN 12
			ELSE MONTH(GETDATE()) - 1
		END AS Month,
		CASE MONTH(GETDATE())
			WHEN 1 THEN YEAR(GETDATE()) - 1
			ELSE YEAR(GETDATE())
		END AS Year
END
GO