using Microsoft.EntityFrameworkCore;
using KeyVaultMvcApp.Models;
 
namespace KeyVaultMvcApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}