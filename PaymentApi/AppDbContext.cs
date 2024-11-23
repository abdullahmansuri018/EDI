using Microsoft.EntityFrameworkCore;
using PaymentApi.Models;

namespace PaymentApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UserContainerData> UserContainerData { get; set; }

        public DbSet<User> Users { get; set; }

    }

   
}
