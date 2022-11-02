namespace GitInsight.Entities.Tests;

public sealed class CommitRepositoryTests : IDisposable
{
    private readonly GitInsightContext _context;
    private readonly CommitRepository _repository;

    public CommitRepositoryTests() {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();

        _context = context;
        _repository = new CommitRepository(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    } 
}