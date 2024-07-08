-- Create Books Table if not exists
CREATE TABLE IF NOT EXISTS Books (
    Id UUID PRIMARY KEY,
    Title VARCHAR(150) NOT NULL,
    Author VARCHAR(150) NOT NULL,
    ISBN VARCHAR(13),
    CreatedBy VARCHAR(150) NOT NULL,
    CreatedDateUtc TIMESTAMPTZ NOT NULL,
    ModifiedBy VARCHAR(150),
    ModifiedDateUtc TIMESTAMPTZ,
    DeletedBy VARCHAR(150),
    DeletedDateUtc TIMESTAMPTZ
);

-- Create Users Table if not exists
CREATE TABLE IF NOT EXISTS Users (
    Id UUID PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Salt VARCHAR(255) NOT NULL
);

-- Create Index on Username if not exists
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE c.relname = 'idx_users_username'
        AND n.nspname = 'public'
    ) THEN
        CREATE INDEX idx_users_username ON Users (Username);
    END IF;
END $$;
