-- Grants the web app's system-assigned managed identity access to its database. Idempotent —
-- safe to run on every deploy. Must run as an AAD-authenticated connection (the SQL AAD admin
-- configured in main.tf), never with a SQL login — none exists on this server.
--
-- $(WebAppName) must match the App Service resource name exactly: that's also the display name
-- its system-assigned identity registers under in Azure AD.
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = '$(WebAppName)')
BEGIN
    CREATE USER [$(WebAppName)] FROM EXTERNAL PROVIDER;
END

ALTER ROLE db_datareader ADD MEMBER [$(WebAppName)];
ALTER ROLE db_datawriter ADD MEMBER [$(WebAppName)];
-- EF Core's Database.MigrateAsync() (run automatically on app startup) issues schema DDL.
ALTER ROLE db_ddladmin ADD MEMBER [$(WebAppName)];
