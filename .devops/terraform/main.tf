resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_log_analytics_workspace" "main" {
  name                = "dg-use-nonprod-rmp-law-01"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "main" {
  name                = "dg-use-nonprod-rmp-appi-01"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
}

resource "azurerm_service_plan" "main" {
  name                = "dg-use-nonprod-rmp-plan-01"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "B1"
}

# Microsoft Entra-only authentication — no SQL login/password ever exists. The deploy pipeline's
# OIDC service principal is the AAD admin, so it can grant the web app's managed identity DB
# access purely via AAD (see .devops/sql/grant-managed-identity.sql).
resource "azurerm_mssql_server" "main" {
  name                         = "${var.sql_server_name}${var.sql_server_name_suffix}"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = azurerm_resource_group.main.location
  version                      = "12.0"
  minimum_tls_version          = "1.2"
  public_network_access_enabled = true

  # Required for CREATE USER ... FROM EXTERNAL PROVIDER to resolve any principal other than the
  # connecting AAD admin itself (e.g. the web app's managed identity, in the post-apply grant
  # script) — the server needs its own identity to look principals up in Entra ID. That identity
  # then needs the Directory Readers role, which Terraform can't grant itself (requires Privileged
  # Role Administrator / Global Administrator) — see .devops/README.md for the one-time manual step.
  identity {
    type = "SystemAssigned"
  }

  azuread_administrator {
    login_username              = var.sql_aad_admin_login
    object_id                   = var.deploy_principal_object_id
    azuread_authentication_only = true
  }
}

# The ARM API can report the SQL logical server as created before it's actually queryable by
# dependent resources (firewall rules, databases) — a known azurerm-provider race condition. A
# short forced delay avoids needing to manually retry `terraform apply` when it's hit.
resource "time_sleep" "wait_for_sql_server" {
  depends_on      = [azurerm_mssql_server.main]
  create_duration = "60s"
}

resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"

  depends_on = [time_sleep.wait_for_sql_server]
}

resource "azurerm_mssql_database" "main" {
  name        = var.sql_database_name
  server_id   = azurerm_mssql_server.main.id
  sku_name    = "Basic"
  max_size_gb = 2

  depends_on = [time_sleep.wait_for_sql_server]
}

resource "azurerm_key_vault" "main" {
  name                        = var.key_vault_name
  resource_group_name         = azurerm_resource_group.main.name
  location                    = azurerm_resource_group.main.location
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  sku_name                    = "standard"
  enable_rbac_authorization   = true
  soft_delete_retention_days  = 7
}

# The pipeline's own identity needs to be able to write the seed-admin secret below.
resource "azurerm_role_assignment" "deploy_principal_kv_admin" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Administrator"
  principal_id         = var.deploy_principal_object_id
}

resource "random_password" "seed_admin" {
  length  = 24
  special = true
}

resource "azurerm_key_vault_secret" "seed_admin_password" {
  name         = "SEED-ADMIN-PASSWORD"
  value        = random_password.seed_admin.result
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_role_assignment.deploy_principal_kv_admin]
}

resource "azurerm_linux_web_app" "main" {
  name                = "${var.app_service_name}${var.app_service_name_suffix}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true

  identity {
    type = "SystemAssigned"
  }

  site_config {
    minimum_tls_version = "1.2"
    application_stack {
      dotnet_version = "10.0"
    }
  }

  # No SQL password (Managed Identity auth in the connection string) and no App Insights/Key
  # Vault secret handling needed beyond the Key Vault reference below.
  app_settings = {
    ASPNETCORE_ENVIRONMENT                 = "Production"
    "AzureAd__Instance"                    = "https://login.microsoftonline.com/"
    "AzureAd__TenantId"                    = var.azure_ad_tenant_id
    "AzureAd__ClientId"                    = var.azure_ad_client_id
    "AzureAd__Audience"                    = var.azure_ad_audience
    "ConnectionStrings__DefaultConnection" = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Database=${azurerm_mssql_database.main.name};Authentication=Active Directory Managed Identity;Encrypt=True;TrustServerCertificate=False;"
    APPLICATIONINSIGHTS_CONNECTION_STRING  = azurerm_application_insights.main.connection_string
    SEED_ADMIN_PASSWORD                    = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.seed_admin_password.versionless_id})"
    BootstrapAdminEmail                    = var.bootstrap_admin_email
  }
}

resource "azurerm_role_assignment" "webapp_kv_secrets_user" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_linux_web_app.main.identity[0].principal_id
}
