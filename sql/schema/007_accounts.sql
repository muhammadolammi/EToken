-- +goose Up
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.accounts', N'U') IS NULL
BEGIN  
    CREATE TABLE accounts (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        cif UNIQUEIDENTIFIER NOT NULL REFERENCES users(cif) ON DELETE CASCADE, 
        number  NVARCHAR(10) UNIQUE NOT NULL , 
        balance  DECIMAL(18, 4) NOT NULL CONSTRAINT DF_Balance DEFAULT 0.0000,
        type NVARCHAR(20) NOT NULL DEFAULT 'savings', -- active | current

        created_at DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        status NVARCHAR(20) NOT NULL DEFAULT 'active', -- active | locked
        
        INDEX ix_accounts_cif (cif)
    );
END;
-- +goose StatementEnd

-- +goose Down
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.accounts', N'U') IS NOT NULL
BEGIN
    DROP TABLE accounts;
END;
-- +goose StatementEnd