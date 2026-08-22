subscription_id      = "f1face66-23ea-4977-925e-ba992cc94597"
location             = "centralus"
resource_group_name  = "dg-use-nonprod-rmp-01"
app_service_name     = "dg-use-nonprod-rmp-app-01"
sql_server_name      = "dg-use-nonprod-rmp-sql-01"
sql_database_name    = "dg-use-nonprod-rmp-sqldb-01"
key_vault_name        = "dg-use-nonprod-rmp-kv-01"

# Enterprise application (service principal) Object ID for releasemgmtport-dev.
deploy_principal_object_id = "03382cf0-bb91-4856-bad0-253ac9ffae08"

# App's own sign-in — reusing releasemgmtport-dev (the GitHub Actions OIDC deployment app) rather
# than a separate registration. Audience is its default Application ID URI (api://<client-id>),
# matching the "Expose an API" default and what auth.interceptor.ts requests as a scope.
azure_ad_tenant_id = "f36628fb-a459-4a87-a3bf-ea3aede4d7eb"
azure_ad_client_id = "49eb83bf-411b-4ab6-bda2-c7afe12f41b0"
azure_ad_audience  = "api://49eb83bf-411b-4ab6-bda2-c7afe12f41b0"
