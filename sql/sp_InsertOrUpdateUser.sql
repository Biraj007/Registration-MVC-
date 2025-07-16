CREATE PROCEDURE sp_InsertOrUpdateUser
    @UserID INT = NULL,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @DOB DATE,
    @Email NVARCHAR(255),
    @ImagePath NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF @UserID IS NULL
    BEGIN
        INSERT INTO User_Info (FirstName, LastName, DOB, Email, ImagePath)
        VALUES (@FirstName, @LastName, @DOB, @Email, @ImagePath);
        SELECT SCOPE_IDENTITY() AS UserID;
    END
    ELSE
    BEGIN
        UPDATE User_Info
        SET FirstName = @FirstName,
            LastName = @LastName,
            DOB = @DOB,
            Email = @Email,
            ImagePath = @ImagePath
        WHERE UserID = @UserID;
        SELECT @UserID AS UserID;
    END
END 