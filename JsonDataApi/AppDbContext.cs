using Microsoft.EntityFrameworkCore;
using JsonDataApi.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserContainerData> UserContainerData { get; set; }

    public DbSet<Payment> paymentData{get; set;}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Explicitly defining the primary key for UserContainerData if necessary
        modelBuilder.Entity<UserContainerData>().HasKey(uc => uc.Id);

        base.OnModelCreating(modelBuilder);
    }
}