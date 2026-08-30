-- +goose Up

-- +goose StatementBegin
IF OBJECT_ID(N'dbo.verification_log', N'U') IS  NULL
BEGIN 
CREATE TABLE verification_log (
id BIGINT IDENTITY PRIMARY KEY,
cif UNIQUEIDENTIFIER NOT NULL REFERENCES users(cif) ON DELETE CASCADE, 
device_id UNIQUEIDENTIFIER NOT NULL REFERENCES customer_devices(device_id),
action_type NVARCHAR(20) NOT NULL, -- login | transaction | other
result NVARCHAR(20) NOT NULL, -- success | failed | locked_out
ip_address NVARCHAR(45),
created_at DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
INDEX ix_verification_log_cif_created (cif, created_at)
);
END;
-- +goose   StatementEnd

-- +goose Down
-- +goose StatementBegin

IF OBJECT_ID(N'dbo.verification_log', N'U') IS NOT NULL
BEGIN
DROP TABLE  verification_log;
END;
-- +goose   StatementEnd
