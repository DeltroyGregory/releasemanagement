# .devops

Azure infrastructure (Terraform) and the deploy pipeline for RMP's `dev` environment.

- **`bootstrap/`** — one-time setup (Terraform remote state + GitHub Actions OIDC identity). Run this first — see [bootstrap/README.md](bootstrap/README.md). Not run by the pipeline itself.
- **`terraform/`** — the actual infrastructure: resource group `dg-use-nonprod-rmp-01`, a Linux App Service (`.NET 10`, system-assigned managed identity), an Azure SQL server/database (Microsoft Entra-only auth — no SQL password ever exists), a Key Vault, and Log Analytics + Application Insights. Flat structure on purpose (no modules) — this is a single environment; modules are worth extracting once `prod` exists.
- **`sql/grant-managed-identity.sql`** — idempotent script the pipeline runs after every `terraform apply`, granting the web app's managed identity access to its own database (AAD auth only, run by the pipeline's own OIDC identity as the SQL AAD admin).

The pipeline is two separate GitHub Actions workflows (not Azure Pipelines, despite the folder name — chosen because the repo already lives on GitHub), decoupled so either can be re-run independently:

- **`.github/workflows/terraform-dev.yml`** — `terraform plan` on pull requests touching `terraform/**`, `terraform apply` on push to `development` (or manual `workflow_dispatch`).
- **`.github/workflows/deploy-app-dev.yml`** — builds and deploys the app. Fully independent of `terraform-dev.yml`: triggers on every push to `development` (or manual `workflow_dispatch`) regardless of Terraform's state, rather than waiting on it — if infra isn't up yet this will just fail at the SQL/App Service steps, and you re-run it once `terraform-dev` has succeeded. Since it has no way to read another workflow's outputs, it uses the same resource names already in `terraform/dev.tfvars` (`RESOURCE_GROUP_NAME`/`APP_SERVICE_NAME`/`SQL_SERVER_NAME`/`SQL_DATABASE_NAME`, hardcoded at the top of the file) instead of Terraform outputs — they're deterministic chosen names, not values generated at apply time, so this is safe as long as the two stay in sync if you ever rename something.

## Prerequisites before the pipeline will run

See [bootstrap/README.md](bootstrap/README.md) for the exact one-time steps: create a GitHub Environment named `dev`, run the bootstrap script, add the values it prints as **variables on that `dev` environment** (not repo-wide), and fill in `deploy_principal_object_id` in `terraform/dev.tfvars`. A `terraform` job failing at the Azure login step with "client-id and tenant-id" missing means one of those steps hasn't been done yet.

## Auth notes

Terraform authenticates to Azure via its own native OIDC support (`use_oidc = true` in `terraform/providers.tf`, fed by `ARM_CLIENT_ID`/`ARM_TENANT_ID`/`ARM_SUBSCRIPTION_ID`/`ARM_USE_OIDC` env vars on the `terraform` job) — **not** by reusing an `azure/login` CLI session. The azurerm provider explicitly refuses to authenticate through an Azure CLI session that was itself established as a service principal (only real interactive-user `az login` sessions work for CLI-based auth), so there's deliberately no `azure/login` step in the `terraform` job. `build-and-deploy` still uses `azure/login` — that's fine, because it runs raw `az` CLI commands directly rather than asking Terraform to borrow the session.

## Known things to double-check at first apply

- `dotnet_version = "10.0"` in `terraform/main.tf`'s `application_stack` block — Azure App Service's supported Linux .NET runtime list may lag a very recent .NET release. If `terraform apply` rejects it, check `az webapp list-runtimes --os linux` and adjust.
- `app_service_name` / `sql_server_name` (`dg-use-nonprod-rmp-app-01` / `-sql-01`) must be globally unique across all of Azure, not just this subscription. If `terraform apply` fails on a name collision, set `app_service_name_suffix` / `sql_server_name_suffix` in `dev.tfvars`.
- `key_vault_name` (`dg-use-nonprod-rmp-kv-01`) is exactly at Key Vault's 24-character limit, so there's no room for a suffix — if it collides, change it to `dg-use-nonprod-rmp-kv` (22 chars) instead.
