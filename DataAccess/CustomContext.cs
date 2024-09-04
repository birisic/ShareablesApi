using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Metadata;
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

            modelBuilder.Entity<UserWorkspace>()
                        .HasKey(uw => new { uw.UserId, uw.WorkspaceId, uw.UseCaseId });

            modelBuilder.Entity<WorkspacesMedia>()
                        .HasKey(wm => new { wm.WorkspaceId, wm.MediaId });

            modelBuilder.Entity<LinkAccessLog>()
                .HasOne(e => e.Link)
                .WithOne(e => e.AccessLog)
                .IsRequired();
        }

        public override int SaveChanges()
        {
            IEnumerable<EntityEntry> entries = this.ChangeTracker.Entries();

            foreach (EntityEntry entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is Entity e)
                    {
                        e.DeletedAt = null;
                        e.CreatedAt = DateTime.UtcNow;
                    }
                }

                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is Entity e)
                    {
                        e.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            return base.SaveChanges();
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<UseCaseLog> UseCaseLogs { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<UserWorkspace> UsersWorkspaces { get; set; }
        public DbSet<WorkspacesMedia> WorkspacesMedia { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<LinkAccessLog> LinksAccessLogs { get; set; }
    }
}
