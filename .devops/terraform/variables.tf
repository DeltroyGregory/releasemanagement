variable "subscription_id" {
  description = "Azure subscription ID to deploy into."
  type        = string
}

variable "location" {
  description = "Azure region for all resources."
  type        = string
  default     = "centralus"
}

variable "resource_group_name" {
  description = "Resource group name (created by this Terraform)."
  type        = string
  default     = "dg-use-nonprod-rmp-01"
}

variable "app_service_name" {
  description = "Web app name. Must be globally unique (azurewebsites.net) — bump app_service_name_suffix if it's taken."
  type        = string
  default     = "dg-use-nonprod-rmp-app-01"
}

variable "app_service_name_suffix" {
  description = "Optional suffix appended to app_service_name if the base name collides globally."
  type        = string
  default     = ""
}

variable "sql_server_name" {
  description = "SQL logical server name. Must be globally unique — bump sql_server_name_suffix if it's taken."
  type        = string
  default     = "dg-use-nonprod-rmp-sql-01"
}

variable "sql_server_name_suffix" {
  type    = string
  default = ""
}

variable "sql_database_name" {
  type    = string
  default = "dg-use-nonprod-rmp-sqldb-01"
}

variable "key_vault_name" {
  description = "Must be <=24 chars and globally unique across all of Azure. This name is exactly 24 chars, so there's no room for a suffix — if it collides, fall back to dg-use-nonprod-rmp-kv (22 chars)."
  type        = string
  default     = "dg-use-nonprod-rmp-kv-01"
}

variable "deploy_principal_object_id" {
  description = "Object ID of the GitHub Actions OIDC service principal (printed by .devops/bootstrap/setup-oidc-and-state.ps1). Set as the SQL AAD admin and granted Key Vault access."
  type        = string
}

variable "sql_aad_admin_login" {
  description = "Display name for the SQL AAD admin entry (the OIDC service principal)."
  type        = string
  default     = "rmp-github-actions-deployer"
}

variable "azure_ad_tenant_id" {
  description = "The app's own Azure AD app registration TenantId (separate from the GitHub OIDC deployment identity). Leave blank until a real registration exists — Program.cs falls back to DevAuthHandler when unset."
  type        = string
  default     = ""
}

variable "azure_ad_client_id" {
  type    = string
  default = ""
}

variable "azure_ad_audience" {
  type    = string
  default = ""
}

variable "bootstrap_admin_email" {
  description = "Email address that JitUserProvisioning promotes to Admin on sign-in (checked every request, not just first login) — solves the chicken-and-egg problem of nobody being Admin yet in a fresh Azure AD environment."
  type        = string
  default     = ""
}
