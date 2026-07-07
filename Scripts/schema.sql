IF DB_ID(N'UserCacheDb') IS NULL
BEGIN
    CREATE DATABASE UserCacheDb;
END;
GO

USE UserCacheDb;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id int NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Name nvarchar(200) NOT NULL,
        Username nvarchar(100) NOT NULL,
        Email nvarchar(200) NOT NULL,
        Phone nvarchar(100) NULL,
        Website nvarchar(200) NULL,
        CompanyName nvarchar(200) NULL,
        City nvarchar(200) NULL,
        LastFetchedAt datetimeoffset NOT NULL
    );
END;
GO