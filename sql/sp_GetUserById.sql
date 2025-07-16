CREATE PROCEDURE sp_GetUserById
    @UserID INT
AS
BEGIN
    SELECT * FROM User_Info WHERE UserID = @UserID
END 