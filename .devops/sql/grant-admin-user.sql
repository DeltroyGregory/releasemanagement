-- Grants a human Entra ID account db_owner for manual inspection/admin work. Idempotent — safe to
-- run on every deploy. Requires the SQL server's identity to have Directory Readers in Entra ID
-- (same requirement as grant-managed-identity.sql) to resolve the account by UPN/email.
--
-- If this fails to resolve a guest account by its plain email, Azure AD guests sometimes need the
-- B2B UPN format instead: 'name_domain.com#EXT#@<tenant>.onmicrosoft.com'.
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = '$(AdminEmail)')
BEGIN
    CREATE USER [$(AdminEmail)] FROM EXTERNAL PROVIDER;
END

ALTER ROLE db_owner ADD MEMBER [$(AdminEmail)];
