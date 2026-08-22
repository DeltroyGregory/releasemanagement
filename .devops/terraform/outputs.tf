output "resource_group_name" {
  value = azurerm_resource_group.main.name
}

output "app_service_name" {
  value = azurerm_linux_web_app.main.name
}

output "sql_server_name" {
  value = azurerm_mssql_server.main.name
}

output "sql_server_fqdn" {
  value = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "sql_database_name" {
  value = azurerm_mssql_database.main.name
}

output "webapp_principal_id" {
  value = azurerm_linux_web_app.main.identity[0].principal_id
}

# Grant this the Directory Readers role in Entra ID (Portal or PowerShell — Terraform's OIDC
# identity doesn't have rights to do it) before the post-apply SQL grant script will work.
output "sql_server_identity_principal_id" {
  value = azurerm_mssql_server.main.identity[0].principal_id
}
