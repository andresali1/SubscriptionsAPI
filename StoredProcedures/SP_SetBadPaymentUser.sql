USE WebApiAuthors
GO

CREATE PROCEDURE SetBadPaymentUser
AS
BEGIN
	UPDATE AspNetUsers SET BadPaymentHistory = 'True'
	FROM Bill B
	INNER JOIN AspNetUsers U ON U.Id = B.UserId
	WHERE B.Paid = 'False'
	AND B.PaymentDate < GETDATE()
END
GO