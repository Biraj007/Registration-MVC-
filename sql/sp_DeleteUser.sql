CREATE PROCEDURE sp_DeleteUser
    @UserID INT
AS
BEGIN
    DELETE FROM User_Info WHERE UserID = @UserID
END 