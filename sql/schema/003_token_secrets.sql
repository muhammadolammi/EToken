-- +goose Up
-- +goose StatementBegin

IF OBJECT_ID(N'dbo.token_secrets', N'U') IS NULL
BEGIN
CREATE TABLE  token_secrets (
id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
cif UNIQUEIDENTIFIER NOT NULL REFERENCES users(cif) ON DELETE CASCADE, 
device_id UNIQUEIDENTIFIER NOT NULL REFERENCES customer_devices(device_id),
encrypted_secret VARBINARY(MAX) NOT NULL, -- ciphertext from KMS/Key Vault
last_accepted_bucket BIGINT NOT NULL DEFAULT 0,
status NVARCHAR(20) NOT NULL DEFAULT 'active',
created_at DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
INDEX ix_token_secrets_cif (cif)
);
END;

-- +goose StatementEnd


-- +goose Down
-- +goose StatementBegin

IF OBJECT_ID(N'dbo.token_secrets', N'U') IS NOT NULL
BEGIN
 DROP TABLE token_secrets;
END;
-- +goose StatementEnd
