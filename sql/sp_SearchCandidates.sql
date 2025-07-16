CREATE PROCEDURE sp_SearchCandidates
    @term NVARCHAR(100)
AS
BEGIN
    SELECT * FROM User_Info
    WHERE FirstName LIKE @term OR LastName LIKE @term OR Email LIKE @term
END 