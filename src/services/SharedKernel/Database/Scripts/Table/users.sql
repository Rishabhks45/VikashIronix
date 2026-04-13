    CREATE TABLE users (
        id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        FirstName nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	    PhoneNumber nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	    LastName nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        LastLoginAt datetime2 NULL,
	    RefreshToken nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	    RefreshTokenExpiry datetime2 NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        role_id INT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        DeletedAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Users_Roles FOREIGN KEY (role_id) REFERENCES roles(id)
    );
