SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetUsers]
    @Page INT,
    @PageSize INT,
    @Age INT = NULL,
    @Country NVARCHAR(100) = NULL,
    @TotalCount INT OUTPUT,
    @TotalPages INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @TotalCount = COUNT(*)
    FROM Users
    WHERE (@Age IS NULL OR Age = @Age)
      AND (@Country IS NULL OR Country = @Country)
      AND DeletedAt IS NULL;

    SET @TotalPages = CEILING(CAST(@TotalCount AS FLOAT) / @PageSize);

    SELECT Id, FirstName, LastName, Age, CreatedAt, UpdatedAt, Country, Province, City, Email, PasswordHash
    FROM Users
    WHERE (@Age IS NULL OR Age = @Age)
      AND (@Country IS NULL OR Country = @Country)
      AND DeletedAt IS NULL
    ORDER BY CreatedAt DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
