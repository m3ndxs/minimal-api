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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var stringConnection = _configurationAppSettings.GetConnectionString("DefaultConnection")?.ToString();

        if (!string.IsNullOrEmpty(stringConnection))
        {
            optionsBuilder.UseSqlServer(stringConnection);
        }
    }
}