using rmp.Auth;
using rmp.Data;
using rmp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

const string ClientDevCors = "ClientDev";

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// No real Azure AD app registration exists yet in local dev — fall back to a dev-only auth handler
// that always authenticates as the seeded dev admin. A real deployment always sets AzureAd:TenantId.
var azureAdTenantId = builder.Configuration["AzureAd:TenantId"];
var useDevAuth = string.IsNullOrWhiteSpace(azureAdTenantId);

if (useDevAuth)
{
    builder.Services.AddAuthentication(DevAuthHandler.SchemeName)
        .AddScheme<AuthenticationSchemeOptions, DevAuthHandler>(DevAuthHandler.SchemeName, _ => { });
}
else
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
}

builder.Services.AddTransient<IClaimsTransformation, DbRoleClaimsTransformation>();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy(ClientDevCors, policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Database migration failed");
    }

    try
    {
        await SeedRolesAndUsers.RunAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Role/user seeding failed");
    }

    try
    {
        await SeedPermissions.RunAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Permission seeding failed");
    }
}

app.UseHttpsRedirection();

app.UseCors(ClientDevCors);

app.UseAuthentication();
app.UseMiddleware<JitUserProvisioning>();
app.UseAuthorization();

app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
}

app.Run();
