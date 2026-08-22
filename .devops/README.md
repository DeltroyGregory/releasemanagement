# .devops

Azure infrastructure (Terraform) and the deploy pipeline for RMP's `dev` environment.

- **`bootstrap/`** — one-time setup (Terraform remote state + GitHub Actions OIDC identity). Run this first — see [bootstrap/README.md](bootstrap/README.md). Not run by the pipeline itself.
- **`terraform/`** — the actual infrastructure: resource group `dg-use-nonprod-rmp-01`, a Linux App Service (`.NET 10`, system-assigned managed identity), an Azure SQL server/database (Microsoft Entra-only auth — no SQL password ever exists), a Key Vault, and Log Analytics + Application Insights. Flat structure on purpose (no modules) — this is a single environment; modules are worth extracting once `prod` exists.
- **`sql/grant-managed-identity.sql`** — idempotent script the pipeline runs after every `terraform apply`, granting the web app's managed identity access to its own database (AAD auth only, run by the pipeline's own OIDC identity as the SQL AAD admin).

The pipeline itself is `.github/workflows/deploy-dev.yml` (GitHub Actions, not Azure Pipelines, despite the folder name — chosen because the repo already lives on GitHub). It runs `terraform plan` on pull requests touching `terraform/**`, and `terraform apply` + build + deploy on push to `development`.

## Prerequisites before the pipeline will run

See [bootstrap/README.md](bootstrap/README.md) for the exact one-time steps: run the bootstrap script, create a GitHub Environment named `dev`, add the repo variables it prints, and fill in `deploy_principal_object_id` in `terraform/dev.tfvars`.

## Known things to double-check at first apply

- `dotnet_version = "10.0"` in `terraform/main.tf`'s `application_stack` block — Azure App Service's supported Linux .NET runtime list may lag a very recent .NET release. If `terraform apply` rejects it, check `az webapp list-runtimes --os linux` and adjust.
- `app_service_name` / `sql_server_name` (`dg-use-nonprod-rmp-app-01` / `-sql-01`) must be globally unique across all of Azure, not just this subscription. If `terraform apply` fails on a name collision, set `app_service_name_suffix` / `sql_server_name_suffix` in `dev.tfvars`.
- `key_vault_name` (`dg-use-nonprod-rmp-kv-01`) is exactly at Key Vault's 24-character limit, so there's no room for a suffix — if it collides, change it to `dg-use-nonprod-rmp-kv` (22 chars) instead.
