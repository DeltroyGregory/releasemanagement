subscription_id      = "f1face66-23ea-4977-925e-ba992cc94597"
location             = "centralus"
resource_group_name  = "dg-use-nonprod-rmp-01"
app_service_name     = "dg-use-nonprod-rmp-app-01"
sql_server_name      = "dg-use-nonprod-rmp-sql-01"
sql_database_name    = "dg-use-nonprod-rmp-sqldb-01"
key_vault_name        = "dg-use-nonprod-rmp-kv-01"

# Enterprise application (service principal) Object ID for releasemgmtport-dev.
deploy_principal_object_id = "03382cf0-bb91-4856-bad0-253ac9ffae08"

# Filled in once a real Azure AD app registration exists for the app's own sign-in (separate from
# the GitHub Actions OIDC deployment identity above). Leave blank for now — Program.cs falls back
# to DevAuthHandler when AzureAd:TenantId is unset.
azure_ad_tenant_id = ""
azure_ad_client_id = ""
azure_ad_audience  = ""
