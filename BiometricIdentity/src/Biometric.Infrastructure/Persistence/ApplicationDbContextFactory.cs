using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Biometric.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "src/Biometric.Api"))
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<ApplicationDbContextFactory>() // This pulls from your machine's secrets
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // We use a dummy string here because migrations only need to know
            // the PROVIDER (Postgres), not the actual password.
            optionsBuilder.UseNpgsql("Host=localhost;Database=dummy", x => x.UseVector());

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
