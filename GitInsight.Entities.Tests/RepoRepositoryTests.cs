namespace GitInsight.Entities.Tests;

public sealed class RepoRepositoryTests : IDisposable
{
    private readonly GitInsightContext _context;
    private readonly RepoRepository _repository;
    public RepoRepositoryTests() {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();

        _context = context;
        _repository = new RepoRepository(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    } 
}