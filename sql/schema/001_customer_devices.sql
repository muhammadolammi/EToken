-- +goose Up
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.customer_devices', N'U') IS NULL
BEGIN  
    CREATE TABLE customer_devices (
        device_id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        cif NVARCHAR(20) NOT NULL,
        device_model NVARCHAR(100),
        status NVARCHAR(20) NOT NULL DEFAULT 'active', -- active | revoked
        registered_at DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        revoked_at DATETIMEOFFSET NULL,
        INDEX ix_customer_devices_cif (cif)
    );
END;
-- +goose StatementEnd

-- +goose Down
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.customer_devices', N'U') IS NOT NULL
BEGIN
    DROP TABLE customer_devices;
END;
-- +goose StatementEnd