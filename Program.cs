using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KeyVaultMvcApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Key Vault integration
var keyVaultUrl = builder.Configuration["KeyVault:VaultUrl"];
if (string.IsNullOrEmpty(keyVaultUrl))
{
    throw new InvalidOperationException("Key Vault URL is not configured.");
}

// Initialize DbContext configuration
string connectionString;

try 
{
    // Create Key Vault client
    var client = new SecretClient(
        new Uri(keyVaultUrl), 
        new DefaultAzureCredential());

    // Retrieve the secret
    KeyVaultSecret secret = await client.GetSecretAsync("SQLConnectionString");
    connectionString = secret?.Value;

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("SQL connection string not found in Key Vault.");
    }
}
catch (Exception ex)
{
    // Fallback to local DB for development
    connectionString = "Server=(localdb)\\mssqllocaldb;Database=KeyVaultMvcTempDb;Trusted_Connection=True;";
    Console.WriteLine($"Key Vault access failed, using local DB: {ex.Message}");
}

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();