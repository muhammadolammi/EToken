-- +goose Up
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.verification_attempts', N'U') IS  NULL
BEGIN 

CREATE TABLE  verification_attempts (
    cif UNIQUEIDENTIFIER NOT NULL REFERENCES users(cif) ON DELETE CASCADE, 
failed_count INT NOT NULL DEFAULT 0,
locked_until DATETIMEOFFSET NULL
);
END;
-- +goose StatementEnd
 
-- +goose Down
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.verification_attempts', N'U') IS NOT NULL
BEGIN 
DROP TABLE  verification_attempts;
END;
-- +goose StatementEnd
