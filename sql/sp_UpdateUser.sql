ALTER PROCEDURE sp_UpdateUser
    @UserID INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @DOB DATE,
    @ImagePath NVARCHAR(255)
AS
BEGIN
    UPDATE User_Info
    SET
        FirstName = @FirstName,
        LastName = @LastName,
        DOB = @DOB,
        ImagePath = @ImagePath
    WHERE UserID = @UserID
END 