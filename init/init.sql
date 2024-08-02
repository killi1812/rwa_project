-- init.sql
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'rwa')
BEGIN
    CREATE DATABASE rwa;
END
GO

USE rwa;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tableData')
BEGIN
    CREATE TABLE tableData (
        id INT IDENTITY(1,1) PRIMARY KEY,
        name VARCHAR(255)
    );
END
GO