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

    [Fact]
    public void Create_commit_returns_created(){
        // Arrange
        var repo = new Repo("name", 1, new List<Commit>());
        _context.Repos.Add(repo);
        _context.SaveChanges();
        var author = new Author("name", "email", new List<Commit>());
        _context.Authors.Add(author);
        _context.SaveChanges();
        var newcommitDTO = new CommitCreateDTO(1, 1, new DateTime());

        // Act
        var (response, newCommitId) = _repository.Create(newcommitDTO);

        // Assert
        Assert.Equal(Response.Created, response);
        Assert.Equal(1, newCommitId);
    }

    [Fact]
    public void Create_commit_returns_conflict(){
        // Arrange
        var firstCommitDTO = new CommitCreateDTO(1, 1, new DateTime());

        // Act
        var (response, firstCommitId) = _repository.Create(firstCommitDTO);

        // Assert
        response.Should().Be(Response.Conflict);
        firstCommitId.Should().Be(1);
    }

    [Fact]
    public void Read_commit_count_0() {
        // Arrange

        // Act
        var commitList = _repository.Read();

        // Assert
        Assert.True(commitList.Count() == 0);
    }

    [Fact]
    public void Read_commit_count_1() {
        // Arrange
        var repo = new Repo("name", 1, new List<Commit>());
        _context.Repos.Add(repo);
        _context.SaveChanges();
        var author = new Author("name", "email", new List<Commit>());
        _context.Authors.Add(author);
        _context.SaveChanges();
        var newCommit = new Commit(repo, author, new DateTime());
        _context.Commits.Add(newCommit);
        _context.SaveChanges();

        // Act
        var commitList = _repository.Read();

        // Assert
        Assert.True(commitList.Count() == 1);
    }

    [Fact]
    public void Commit_find_found() {
        // Arrange
        var repo = new Repo("name", 1, new List<Commit>());
        _context.Repos.Add(repo);
        _context.SaveChanges();
        var author = new Author("name", "email", new List<Commit>());
        _context.Authors.Add(author);
        _context.SaveChanges();
        var commit = new Commit(repo, author, new DateTime());
        _context.Commits.Add(commit);
        _context.SaveChanges();
        
        // Act
        var commitInRepo = _repository.Find(1);

        // Assert()
        Assert.Equal(new CommitDTO(commit.Id, commit.Repo.Name, commit.Author.Name, new DateTime()), commitInRepo);
    }

    [Fact]
    public void commit_find_not_found() {
        // Arrange
        
        // Act
        var commit = _repository.Find(100);

        // Assert
        Assert.Null(commit);
    }

    [Fact]
    public void Delete_commit()
    {
        // Arrange
        var repo = new Repo("name", 1, new List<Commit>());
        _context.Repos.Add(repo);
        _context.SaveChanges();
        var author = new Author("name", "email", new List<Commit>());
        _context.Authors.Add(author);
        _context.SaveChanges();
        var commit = new Commit(repo, author, new DateTime());
        _context.Commits.Add(commit);
        _context.SaveChanges();

        // Act
        var response = _repository.Delete(1);

        // Assert
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public void Delete_commit_not_found()
    {
        // Arrange

        // Act
        var response = _repository.Delete(100);

        // Assert
        response.Should().Be(Response.NotFound);
    }
}