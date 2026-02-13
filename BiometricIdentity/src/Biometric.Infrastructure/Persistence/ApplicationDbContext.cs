using Biometric.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biometric.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enable pgvector extension in Postgres
            modelBuilder.HasPostgresExtension("vector");

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.FaceEmbedding).HasColumnType("vector(512)");
            });
        }
    }
}
