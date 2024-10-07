SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CreateUsers]
    @UsersData UserTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Users (FirstName, LastName, Email, Age, CreatedAt, Country, Province, City, PasswordHash)
    SELECT FirstName, LastName, Email, Age, CreatedAt, Country, Province, City, PasswordHash
    FROM @UsersData;

    SELECT Id, FirstName, LastName, Email, Age, CreatedAt, Country, Province, City, PasswordHash
    FROM Users
    WHERE Id IN (SELECT SCOPE_IDENTITY());
END;
GO
