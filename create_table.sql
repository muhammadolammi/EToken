IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EToken')
BEGIN
    CREATE DATABASE EToken;
END
GO

-- 2. Switch context to your database
USE EToken;
GO