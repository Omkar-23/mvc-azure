using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KeyVaultMvcApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Key Vault Configuration
var keyVaultUrl = builder.Configuration["KeyVault:VaultUrl"];
if (string.IsNullOrEmpty(keyVaultUrl))
{
    throw new InvalidOperationException("Key Vault URL is not configured.");
}

try
{
    // This will use Managed Identity when deployed to Azure
    var credential = new DefaultAzureCredential();
    
    var client = new SecretClient(new Uri(keyVaultUrl), credential);
    KeyVaultSecret secret = await client.GetSecretAsync("SQLConnectionString");
    string connectionString = secret.Value;
    
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseSqlServer(connectionString));
}
catch (Exception ex)
{
    // Will appear in Application Insights or App Service logs
    Console.WriteLine($"Key Vault access failed: {ex.Message}"); 
    throw;
}

var app = builder.Build();

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