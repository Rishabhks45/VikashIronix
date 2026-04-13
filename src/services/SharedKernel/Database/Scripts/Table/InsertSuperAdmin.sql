DECLARE @UserId UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleId INT;
DECLARE @Email NVARCHAR(255) = 'superadmin@VikashIronix.com';
DECLARE @PasswordHash NVARCHAR(MAX) = 'tqJSY4/fcKJ1Ol07SFn0q/4oQV3LRRpBoSxnczfdfnWQLjMnUXseLNtbmo3+sRl17XN742g=';

-- 1. Get or Create Super Admin Role
SELECT @RoleId = id FROM roles WHERE name = 'Super Admin';

IF @RoleId IS NULL
BEGIN
    INSERT INTO roles (name, description) 
    VALUES ('Super Admin', 'Full system access');
    SET @RoleId = SCOPE_IDENTITY();
END

-- 2. Insert User if not exists
IF NOT EXISTS (SELECT 1 FROM users WHERE Email = @Email)
BEGIN
    INSERT INTO users (
        id, FirstName, LastName, PhoneNumber, Email, PasswordHash, role_id, IsActive, CreatedAt, UpdatedAt
    )
    VALUES (
        @UserId, 
        'Super', 
        'Admin', 
        '1234567890',
        @Email, 
        @PasswordHash, 
        @RoleId,
        1, 
        SYSDATETIME(), 
        SYSDATETIME()
    );



    PRINT 'Super Admin user created successfully.';
END
ELSE
BEGIN
    PRINT 'User with this email already exists.';
END
