-- +goose Up
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.transactions', N'U') IS NULL
BEGIN  
    CREATE TABLE transactions (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    source_account_id  UNIQUEIDENTIFIER FOREIGN KEY REFERENCES accounts(id),
    destination_account_id  UNIQUEIDENTIFIER FOREIGN KEY REFERENCES accounts(id),
    amount DECIMAL(18, 4) NOT NULL,
    narration NVARCHAR(MAX) NOT NULL ,
    reference  NVARCHAR(50) NOT NULL ,
    status NVARCHAR(20) NOT NULL DEFAULT 'successful', -- successful | failed
    created_at DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    INDEX ix_transaction_source_account (source_account_id)
    );
   
END;
-- +goose StatementEnd

-- +goose Down
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.transactions', N'U') IS NOT NULL
BEGIN
    DROP TABLE transactions;
END;
-- +goose StatementEnd