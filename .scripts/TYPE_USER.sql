CREATE TYPE [dbo].[UserTableType] AS TABLE(
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Age] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Country] [nvarchar](50) NOT NULL,
	[Province] [nvarchar](50) NOT NULL,
	[City] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](200) NOT NULL,
	[PasswordHash] [nvarchar](100) NOT NULL
)
GO
