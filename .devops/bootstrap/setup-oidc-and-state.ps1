<#
.SYNOPSIS
  One-time bootstrap for RMP's Azure deployment pipeline. Creates the Terraform remote-state
  storage and a GitHub Actions OIDC service principal (federated credential — no client secret).

.DESCRIPTION
  Run once, locally, after `az login`, by someone with Owner (or Contributor + User Access
  Administrator) on the target subscription and Application Administrator in Azure AD. Re-running
  it is mostly idempotent for the app-registration/RBAC steps, but will fail on the storage
  account step if run twice (by design — it's meant to run once).

  NOTE: for the actual `dev` environment, these resources were provisioned by hand rather than by
  running this script (state storage account `dgusenonprodrmpsa01` in resource group
  `dg-use-nonprod-rmp-shared-01`, app registration `releasemgmtport-dev`) — the defaults below
  match what already exists so this script stays an accurate reference/redo path, not because it
  was actually run. Re-running it as-is against `dev` would fail (storage account already exists);
  use it as a template for a future `prod` environment instead (with different parameter values).

.EXAMPLE
  ./setup-oidc-and-state.ps1
#>

param(
    [string]$SubscriptionId = "f1face66-23ea-4977-925e-ba992cc94597",
    [string]$Location = "centralus",
    [string]$GitHubOrg = "DeltroyGregory",
    [string]$GitHubRepo = "releasemanagement",
    [string]$GitHubEnvironment = "dev",
    [string]$StateResourceGroup = "dg-use-nonprod-rmp-shared-01",
    [string]$StateStorageAccountName = "dgusenonprodrmpsa01",
    [string]$StateContainerName = "tfstate",
    [string]$AppRegistrationName = "releasemgmtport-dev"
)

$ErrorActionPreference = "Stop"

Write-Host "Setting active subscription to $SubscriptionId..."
az account set --subscription $SubscriptionId

# --- 1. Terraform remote state storage -------------------------------------------------------
# Can't be created by the Terraform config that will store its state here (chicken-and-egg).
$storageAccountName = $StateStorageAccountName

Write-Host "Creating resource group $StateResourceGroup..."
az group create --name $StateResourceGroup --location $Location --output none

Write-Host "Creating storage account $storageAccountName..."
az storage account create `
    --name $storageAccountName `
    --resource-group $StateResourceGroup `
    --location $Location `
    --sku Standard_LRS `
    --kind StorageV2 `
    --min-tls-version TLS1_2 `
    --allow-blob-public-access false `
    --output none

Write-Host "Creating blob container $StateContainerName..."
az storage container create `
    --name $StateContainerName `
    --account-name $storageAccountName `
    --auth-mode login `
    --output none

# --- 2. GitHub Actions OIDC app registration + federated credential ---------------------------
Write-Host "Creating Azure AD app registration $AppRegistrationName..."
$appId = az ad app create --display-name $AppRegistrationName --query appId -o tsv

Write-Host "Creating service principal for app $appId..."
$spObjectId = az ad sp create --id $appId --query id -o tsv

# Subject is scoped to a GitHub *Environment*, not just a branch — you must create a GitHub
# Environment named $GitHubEnvironment (Settings -> Environments) or the OIDC login will fail.
$federatedSubject = "repo:${GitHubOrg}/${GitHubRepo}:environment:${GitHubEnvironment}"
Write-Host "Creating federated credential for subject '$federatedSubject'..."

$credentialJson = @{
    name      = "github-actions-$GitHubEnvironment"
    issuer    = "https://token.actions.githubusercontent.com"
    subject   = $federatedSubject
    audiences = @("api://AzureADTokenExchange")
} | ConvertTo-Json -Compress

$tempFile = New-TemporaryFile
Set-Content -Path $tempFile -Value $credentialJson -Encoding utf8
az ad app federated-credential create --id $appId --parameters "@$tempFile" --output none
Remove-Item $tempFile

# --- 3. RBAC ------------------------------------------------------------------------------------
Write-Host "Granting Contributor on the subscription (Terraform creates dg-use-nonprod-rmp-01 itself)..."
az role assignment create `
    --assignee $appId `
    --role "Contributor" `
    --scope "/subscriptions/$SubscriptionId" `
    --output none

Write-Host "Granting Storage Blob Data Contributor on the state storage account..."
$stateStorageId = az storage account show --name $storageAccountName --resource-group $StateResourceGroup --query id -o tsv
az role assignment create `
    --assignee $appId `
    --role "Storage Blob Data Contributor" `
    --scope $stateStorageId `
    --output none

$tenantId = az account show --query tenantId -o tsv

# --- 4. Print what's needed next -----------------------------------------------------------------
Write-Host ""
Write-Host "===================================================================="
Write-Host "Bootstrap complete."
Write-Host ""
Write-Host "Add these as GitHub repo VARIABLES (Settings > Secrets and variables > Actions > Variables)"
Write-Host "-- not secrets, none of these are sensitive on their own once OIDC trust is set up:"
Write-Host "  AZURE_CLIENT_ID       = $appId"
Write-Host "  AZURE_TENANT_ID       = $tenantId"
Write-Host "  AZURE_SUBSCRIPTION_ID = $SubscriptionId"
Write-Host "  TF_STATE_RG           = $StateResourceGroup"
Write-Host "  TF_STATE_STORAGE      = $storageAccountName"
Write-Host "  TF_STATE_CONTAINER    = $StateContainerName"
Write-Host ""
Write-Host "Set this in .devops/terraform/dev.tfvars:"
Write-Host "  deploy_principal_object_id = `"$spObjectId`""
Write-Host ""
Write-Host "Before pushing: create a GitHub Environment named '$GitHubEnvironment' under repo"
Write-Host "Settings -> Environments — the federated credential above only trusts tokens minted"
Write-Host "for that specific environment."
Write-Host "===================================================================="
