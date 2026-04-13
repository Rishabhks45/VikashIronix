    CREATE TABLE role_scopes (
        role_id INT NOT NULL,
        scope_id INT NOT NULL,
        PRIMARY KEY (role_id, scope_id),
        FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE,
        FOREIGN KEY (scope_id) REFERENCES scopes(id) ON DELETE CASCADE
    );
