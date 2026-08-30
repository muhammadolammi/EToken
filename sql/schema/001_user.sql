-- +goose Up
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.users', N'U') IS NULL
BEGIN  
CREATE TABLE users (
    cif UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    username NVARCHAR(256) NOT NULL,
    normalized_username NVARCHAR(256) NULL,
    email NVARCHAR(256) NOT  NULL,
    normalized_email NVARCHAR(256) NULL,
    email_confirmed BIT NOT NULL DEFAULT 0,
    password_hash NVARCHAR(MAX) NULL,
    security_stamp NVARCHAR(MAX) NULL,
    concurrency_stamp NVARCHAR(MAX) NULL,
    phone_number NVARCHAR(MAX) NOT NULL,
    phone_number_confirmed BIT NOT NULL DEFAULT 0,
    two_factor_enabled BIT NOT NULL DEFAULT 0,
    lockout_end DATETIMEOFFSET NULL,
    lockout_enabled BIT NOT NULL DEFAULT 1,
    access_failed_count INT NOT NULL DEFAULT 0,
    encrypted_password VARBINARY(MAX) NULL,
    first_name NVARCHAR(MAX) NOT NULL,
    last_name NVARCHAR(MAX) NOT NULL,



    INDEX ix_customer_devices_cif (normalized_username),
    INDEX ix_users_email  (normalized_email)

);


END;
-- +goose StatementEnd

-- +goose Down
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.users', N'U') IS NOT NULL
BEGIN
    DROP TABLE users;
END;
-- +goose StatementEnd