using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using KeyVaultMvcApp.Models;

namespace KeyVaultMvcApp.Migrations
{
    [DbContext(typeof(KeyVaultMvcApp.Data.AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)");
                entity.Property(p => p.Name)
                      .HasMaxLength(100);
            });
        }
    }
}