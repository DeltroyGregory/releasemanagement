terraform {
  required_version = ">= 1.9.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 3.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.6"
    }
    time = {
      source  = "hashicorp/time"
      version = "~> 0.12"
    }
  }

  # Backend config (resource_group_name/storage_account_name/container_name/key) is supplied via
  # `terraform init -backend-config=...` in the GitHub Actions workflow, using the storage account
  # created by .devops/bootstrap/setup-oidc-and-state.ps1. Left empty here on purpose.
  #
  # use_oidc reads ARM_CLIENT_ID/ARM_TENANT_ID/ARM_SUBSCRIPTION_ID + GitHub's own
  # ACTIONS_ID_TOKEN_REQUEST_* env vars directly — it does NOT reuse azure/login's `az` CLI
  # session. Terraform's azurerm backend/provider refuse to authenticate via an Azure CLI session
  # that was itself established as a service principal (only real user `az login` sessions work
  # for CLI-based auth), so this is the only path that works in a GitHub Actions OIDC pipeline.
  backend "azurerm" {
    use_azuread_auth = true
    use_oidc         = true
  }
}

provider "azurerm" {
  subscription_id = var.subscription_id
  use_oidc         = true

  features {
    key_vault {
      purge_soft_delete_on_destroy = true
    }
  }
}

provider "azuread" {}

data "azurerm_client_config" "current" {}
