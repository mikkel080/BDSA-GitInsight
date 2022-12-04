using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GitInsight.Api.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<ProgramMinimalAPI>, IAsyncLifetime
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GitInsightContext>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                var connectionString = $"Server=localhost;Database=GitInsight;User Id=sa;Password=<YourStrong@Passw0rd>;Trusted_Connection=False;Encrypt=False";

                services.AddDbContext<GitInsightContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            });

            builder.UseEnvironment("Development");
        }

        public async Task InitializeAsync()
        {
            using var scope = Services.CreateAsyncScope();
            using var _context = scope.ServiceProvider.GetRequiredService<GitInsightContext>();

            await _context.Database.MigrateAsync();

            await _context.SaveChangesAsync();

        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            using var scope = Services.CreateAsyncScope();
            using var context = scope.ServiceProvider.GetRequiredService<GitInsightContext>();

            await context.Database.EnsureDeletedAsync();
        }
    }
}