-- init.sql
    CREATE DATABASE rwa;
GO

USE rwa;
GO

CREATE TABLE tableData (
                           id INT IDENTITY(1,1) PRIMARY KEY,
                           name VARCHAR(255)
);
GO