# One-time bootstrap

Run once, locally, before the GitHub Actions pipeline can do anything. Requires:
- Azure CLI (`az`), logged in (`az login`) as a user with **Owner** (or **Contributor** + **User Access Administrator**) on the target subscription, and **Application Administrator** (or **Cloud Application Administrator**) in Azure AD — this script creates an app registration and assigns subscription-level RBAC.
- PowerShell 7+.

```powershell
cd .devops/bootstrap
./setup-oidc-and-state.ps1
```

## What it creates

1. `rg-rmp-tfstate` resource group + a storage account + `tfstate` blob container — Terraform's remote state backend. Can't be created by the same Terraform run that stores its state there, hence the separate script.
2. An Azure AD app registration + service principal for GitHub Actions, with a **federated credential** trusting GitHub's OIDC issuer scoped to this repo's `dev` GitHub *Environment* specifically (not just a branch) — no client secret is ever created or stored.
3. `Contributor` on the subscription (Terraform creates the `dg-use-nonprod-rmp-01` resource group itself, so it needs subscription-level rights the first time) and `Storage Blob Data Contributor` on the state storage account.

## After running it

1. Add the printed values as **repo variables** — Settings → Secrets and variables → Actions → Variables: `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `TF_STATE_RG`, `TF_STATE_STORAGE`, `TF_STATE_CONTAINER`. None of these are secret on their own once OIDC trust exists — no client secret is involved.
2. Create a GitHub **Environment** named `dev` (Settings → Environments) — the federated credential's subject is scoped to it, so the OIDC login step fails without it.
3. Set `deploy_principal_object_id` in `.devops/terraform/dev.tfvars` to the printed service principal object ID.
4. Push to `development` — the `terraform` job applies the infra, then `build-and-deploy` ships the app to it.

## Adding `prod` later

Re-run this script with `-GitHubEnvironment prod` (and a separate `AppRegistrationName`), add a `prod.tfvars` with its own resource names, and add a second job/workflow gated by a `prod` GitHub Environment with required reviewers.
