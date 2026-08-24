-- +goose Up
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.verification_attempts', N'U') IS  NULL
BEGIN 

CREATE TABLE  verification_attempts (cif NVARCHAR(20) PRIMARY KEY,
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
