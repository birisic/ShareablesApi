using Domain;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess
{
    public class CustomContext : DbContext
    {
        private readonly string _connectionString;
        public CustomContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public CustomContext()
        {
            _connectionString = "Data Source=localhost;Initial Catalog=ShareablesASP;TrustServerCertificate=true;Integrated security = true";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }

        //DB Sets
        public DbSet<User> Users { get; set; }
    }
}
