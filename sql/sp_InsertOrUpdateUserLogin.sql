CREATE PROCEDURE sp_InsertOrUpdateUserLogin
    @UserID INT,
    @Email NVARCHAR(255),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM User_Login WHERE UserID = @UserID AND Email = @Email)
    BEGIN
        UPDATE User_Login SET Password = @Password WHERE UserID = @UserID AND Email = @Email;
    END
    ELSE
    BEGIN
        INSERT INTO User_Login (UserID, Email, Password) VALUES (@UserID, @Email, @Password);
    END
END 