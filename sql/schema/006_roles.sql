
-- +goose Up
-- +goose StatementBegin
-- 1. Roles table
IF OBJECT_ID(N'dbo.roles', N'U') IS NULL
BEGIN
    CREATE TABLE roles (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        name NVARCHAR(256) NULL,
        normalized_name NVARCHAR(256) NULL,
        concurrency_stamp NVARCHAR(MAX) NULL,
        INDEX ix_roles_normalized_name  (normalized_name) WHERE normalized_name IS NOT NULL
    );
END;

-- 2. User-Roles join table
IF OBJECT_ID(N'dbo.user_roles', N'U') IS NULL
BEGIN
    CREATE TABLE user_roles (
        user_id UNIQUEIDENTIFIER NOT NULL,
        role_id UNIQUEIDENTIFIER NOT NULL,
        PRIMARY KEY (user_id, role_id),
        CONSTRAINT fk_user_roles_users FOREIGN KEY (user_id) REFERENCES users (cif) ON DELETE CASCADE,
        CONSTRAINT fk_user_roles_roles FOREIGN KEY (role_id) REFERENCES roles (id) ON DELETE CASCADE,
         INDEX ix_user_roles_role_id  (role_id)
    );
END;




-- +goose StatementEnd
 


-- +goose Down
-- +goose StatementBegin
IF OBJECT_ID(N'dbo.user_roles', N'U') IS NOT NULL
BEGIN 
DROP TABLE  user_roles;
END;

IF OBJECT_ID(N'dbo.roles', N'U') IS NOT NULL
BEGIN 
DROP TABLE  roles;
END;
-- +goose StatementEnd
