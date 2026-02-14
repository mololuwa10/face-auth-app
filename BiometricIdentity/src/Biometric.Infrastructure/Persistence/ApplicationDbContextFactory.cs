using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Biometric.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            DotNetEnv.Env.TraversePath().Load(); // Load from .env file
            var connectionString = DotNetEnv.Env.GetString("NEON_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("NEON_CONNECTION_STRING not found in .env file!");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString, x => x.UseVector());

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
