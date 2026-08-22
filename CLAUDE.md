# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this app is

MBM (Minute By Minute) is a release tracking tool: releases, tasks, the systems/apps touched by a release, fix versions, and comments. This is an early-stage skeleton — the current scope is basic CRUD over those entities, not the full release-governance platform (gates, checklists, ServiceNow/Jira/Confluence integrations, analytics, AI assistant, etc.) that a mature version of this idea would eventually have. Don't assume those features exist; check this doc and the code before building on top of something that isn't there yet.

This file is a map, not a spec — when in doubt, read the controller/component; this doc explains *why* things are shaped the way they are, not every field.

## Dev commands

**Backend** (run from repo root):
```
dotnet build
dotnet run                          # API on http://localhost:5026
dotnet ef migrations add <Name>     # Add a new EF Core migration
dotnet ef database update           # Apply pending migrations
```

**Frontend** (run from `ClientApp/`):
```
npm start        # ng serve --proxy-config proxy.conf.json → http://localhost:4200
npm run build    # Production build → dist/ClientApp/
npm test         # Vitest unit tests (via `ng test`)
```

Run both servers simultaneously for local development. The Angular dev server proxies all `/api/*` requests to the .NET backend at `http://localhost:5026`.

## Architecture

### Backend — .NET 10 Web API

**`Program.cs`** wires up EF Core (SQL Server), ASP.NET Core Identity (roles only, no local passwords), auth, CORS, runs pending migrations, and seeds roles/the dev admin user on startup — each step is wrapped in try/catch and logs-and-continues rather than crashing the app.

**Auth flow — dev fallback vs. real Azure AD:** there is no local register/login endpoint. `AuthController` only exposes `GET /api/auth/me` (reads identity/role claims off the authenticated principal). Which auth handler is wired up is decided at startup by whether `AzureAd:TenantId` is set:
- **Unset (current local dev state)** — `Program.cs` falls back to [DevAuthHandler.cs](Auth/DevAuthHandler.cs), which authenticates every request as a hardcoded seeded dev admin (`admin@mbm.local`, `Admin` role). No real Azure AD app registration exists yet.
- **Set** — uses `Microsoft.Identity.Web`'s `AddMicrosoftIdentityWebApi` against real Azure AD JWTs, the intended production path.

ASP.NET Core Identity (`IdentityUser`/`IdentityRole`) is used purely for role storage — [SeedRolesAndUsers.cs](Data/SeedRolesAndUsers.cs) creates roles and one seed admin user; it never issues a local JWT. There is no permissions/role-key system yet (no `RolePermissions` table) — only raw ASP.NET `[Authorize]`/role checks.

**Data layer:** EF Core Code First against SQL Server. Local dev points at `.\SQLEXPRESS` (see `appsettings.Development.json`); no managed-identity/Azure SQL wiring exists yet — production connection details are TBD. `AppDbContext` extends `IdentityDbContext<IdentityUser>`; see [Data/AppDbContext.cs](Data/AppDbContext.cs) for the full `DbSet` list. One migration exists so far (`InitialCreate`, see [Migrations/](Migrations/)).

**DTOs** in `/DTOs/` are what the API accepts and returns — never expose model classes directly.

**Entities / controllers (current full set — nothing else exists yet):**

| Entity | Controller | DTO file | Notes |
|---|---|---|---|
| `Release` | [ReleasesController.cs](Controllers/ReleasesController.cs) | `ReleaseDtos.cs` | Root entity. Has a `ReleaseType` enum (`Major`/`Minor`/`Patch`/`Hotfix`), free-text `Status`, optional `TargetDate`. Owns Tasks/ReleaseSystems/FixVersions/Comments (cascade delete). |
| `TaskItem` | [TasksController.cs](Controllers/TasksController.cs) | `TaskItemDtos.cs` | Belongs to a `Release`. |
| `ReleaseSystem` | [ReleaseSystemsController.cs](Controllers/ReleaseSystemsController.cs) | `ReleaseSystemDtos.cs` | The systems/apps a release touches. Belongs to a `Release`. |
| `AppVersion` | [AppVersionsController.cs](Controllers/AppVersionsController.cs) | `AppVersionDtos.cs` | Not yet linked via FK to `Release` in `AppDbContext`. |
| `FixVersion` | [FixVersionsController.cs](Controllers/FixVersionsController.cs) | `FixVersionDtos.cs` | Belongs to a `Release`. No Jira sync yet — that's aspirational. |
| `Comment` | [CommentsController.cs](Controllers/CommentsController.cs) | `CommentDtos.cs` | Belongs to a `Release`. |
| — | [AuthController.cs](Controllers/AuthController.cs) | `AuthMeDto.cs` | Just `GET /api/auth/me`. |

There is no `/Services/` directory — no ServiceNow/Jira/Confluence/Octopus/Databricks clients exist. No lifecycle stages, gates, checklists, task templates, CMDB, warranty tracking, analytics, worklist, calendar, or AI assistant controllers exist yet.

### Frontend — Angular 22 (standalone components)

All components are standalone. No NgModules. Uses Angular signals for state, `@if`/`@for` control flow syntax (not `*ngIf`/`*ngFor`).

**Auth:** MSAL (`@azure/msal-browser`, `@azure/msal-angular`) via [ClientApp/src/app/auth/msal.instance.ts](ClientApp/src/app/auth/msal.instance.ts). `authInterceptor` ([ClientApp/src/app/interceptors/auth.interceptor.ts](ClientApp/src/app/interceptors/auth.interceptor.ts)) attaches a bearer token to `/api/*` requests; `authGuard` ([ClientApp/src/app/auth/auth.guard.ts](ClientApp/src/app/auth/auth.guard.ts)) protects the shell route. The frontend always goes through the MSAL flow even while the backend is on `DevAuthHandler` locally.

**Routing** ([ClientApp/src/app/app.routes.ts](ClientApp/src/app/app.routes.ts)): everything sits under a single `Shell` layout route, gated by `authGuard`. Default route is `my-releases`. Routes: `my-releases` (releases mine-only), `releases` (all releases), `releases/new`, `releases/:id`, `releases/:id/edit`.

**Structure** (`ClientApp/src/app/`):
- `core/models.ts` + `core/services/` — one service per entity (`release.ts`, `task.ts`, `release-system.ts`, `app-version.ts`, `fix-version.ts`, `comment.ts`, `auth.ts`).
- `features/releases/` — `release-list`, `release-detail`, `release-form`, `task-form` components.
- `layout/shell/` — the single app shell (nav chrome) all routes render inside.
- `auth/`, `interceptors/` — MSAL guard/interceptor plumbing described above.

## Configuration

| Setting | Location | Notes |
|---|---|---|
| DB connection | `appsettings.Development.json` → `ConnectionStrings.DefaultConnection` | Local dev: `.\SQLEXPRESS`, trusted connection. `appsettings.json` (base/prod) leaves this blank — not yet configured for a real environment. |
| Azure AD (auth) | `appsettings.json`/`appsettings.Development.json` → `AzureAd` | `TenantId`/`ClientId`/`Audience` for backend token validation. Leave `TenantId` blank locally to use `DevAuthHandler` instead — see Auth flow above. No real app registration exists yet. |
| API proxy target | `ClientApp/proxy.conf.json` | Routes `/api` to `http://localhost:5026` |
| .NET ports | `Properties/launchSettings.json` | HTTP: 5026, HTTPS: 7268 |

## Production build

Not yet set up — no `wwwroot`, no deploy pipeline, no build-version stamping. `Program.cs` has the `UseDefaultFiles`/static-file-serving fallback wired up for a non-Development environment, but nothing populates `wwwroot/` yet. When that's built out: `cd ClientApp && npm run build` outputs to `ClientApp/dist/ClientApp/`, which would need to be copied into `wwwroot/` before `dotnet run`.
