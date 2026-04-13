    CREATE TABLE scopes (
        id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(100) NOT NULL UNIQUE, -- e.g., 'read:users'
        description NVARCHAR(255) NULL
    );
