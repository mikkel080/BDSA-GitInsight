using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GitInsight;
using GitInsight.Entities;

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

            var repo_1 = new Repo("test_repo_1");
            var repo_2 = new Repo("test_repo_2");
            _context.Repos.AddRange(repo_1, repo_2);
            await _context.SaveChangesAsync();

            var author_1 = new Author("test_author_1");
            var author_2 = new Author("test_auhtor_2");
            _context.Authors.AddRange(author_1, author_2);
            await _context.SaveChangesAsync();

            var commit_1 = new Commit(){AuthorID = 1, RepoID = 1 };
            var commit_2 = new Commit(){AuthorID = 1, RepoID = 1 };
            _context.Commits.AddRange(commit_1, commit_2);
            await _context.SaveChangesAsync();

            var resultHandler = ResultHandler(_context, _context.Commits, _context.Repos);

        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            using var scope = Services.CreateAsyncScope();
            using var context = scope.ServiceProvider.GetRequiredService<GitInsightContext>();

            await context.Database.EnsureDeletedAsync();
        }
    }
}