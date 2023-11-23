using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Configuration;

namespace Server.Repositories.EfCoreRepository;
public class EfRepository : DbContext
{
    public DbSet<Car> Cars { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(connectionString: $"Server=localhost;Database=EFCoreDb;Integrated Security=True;TrustServerCertificate=True;");
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CarConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

