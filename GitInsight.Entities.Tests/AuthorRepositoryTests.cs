namespace GitInsight.Entities.Tests;

public sealed class AuthorRepositoryTests : IDisposable
{
    private readonly GitInsightContext _context;
    private readonly AuthorRepository _repository;
    public AuthorRepositoryTests() {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();

        _context = context;
        _repository = new AuthorRepository(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    } 
}