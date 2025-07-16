CREATE PROCEDURE sp_InsertOrUpdateUserDetails
    @UserID INT,
    @Aadhar_No NVARCHAR(20) = NULL,
    @Pan_No NVARCHAR(20) = NULL,
    @Gender NVARCHAR(10) = NULL,
    @Highest_Qualification NVARCHAR(100) = NULL,
    @Company_Name NVARCHAR(100) = NULL,
    @Dept NVARCHAR(100) = NULL,
    @Post NVARCHAR(100) = NULL,
    @Mode NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM User_Details WHERE UserID = @UserID)
    BEGIN
        UPDATE User_Details
        SET Aadhar_No = @Aadhar_No,
            Pan_No = @Pan_No,
            Gender = @Gender,
            Highest_Qualification = @Highest_Qualification,
            Company_Name = @Company_Name,
            Dept = @Dept,
            Post = @Post,
            Mode = @Mode
        WHERE UserID = @UserID;
    END
    ELSE
    BEGIN
        INSERT INTO User_Details (UserID, Aadhar_No, Pan_No, Gender, Highest_Qualification, Company_Name, Dept, Post, Mode)
        VALUES (@UserID, @Aadhar_No, @Pan_No, @Gender, @Highest_Qualification, @Company_Name, @Dept, @Post, @Mode);
    END
END 