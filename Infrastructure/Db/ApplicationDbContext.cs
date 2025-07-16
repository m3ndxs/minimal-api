using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastrucuture.Db;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configurationAppSettings;
    public ApplicationDbContext(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }
    public DbSet<Admin> Admins { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>().HasData(
            new Admin
            {
                Id = 1,
                Email = "admin@test.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConnection = _configurationAppSettings.GetConnectionString("DefaultConnection")?.ToString();

            if (!string.IsNullOrEmpty(stringConnection))
            {
                optionsBuilder.UseSqlServer(stringConnection);
            }
        }
    }
}