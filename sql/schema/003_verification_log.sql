-- +goose Up

-- +goose StatementBegin
IF OBJECT_ID(N'dbo.verification_log', N'U') IS  NULL
BEGIN 
CREATE TABLE verification_log (
id BIGINT IDENTITY PRIMARY KEY,
cif NVARCHAR(20) NOT NULL,
device_id UNIQUEIDENTIFIER NOT NULL,
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
