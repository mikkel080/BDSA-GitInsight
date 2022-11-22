namespace GitInsight.Entities.Tests;

public sealed class CommitRepositoryTests : IDisposable
{
    private readonly GitInsightContext _context;
    private readonly CommitRepository _repository;

    public CommitRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();
        var repo = new Repo("name");
        context.Repos.Add(repo);
        context.SaveChanges();
        _context = context;
        _repository = new CommitRepository(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async void Create_commit_returns_created_async()
    {

        //Arrange
        var newcommitDTO = new CommitCreateDTO(1, "nameAuthor", new DateTime());

        // Act
        var (response, newCommitId) = await _repository.CreateAsync(newcommitDTO);
        var newAuthor = (from a in _context.Authors
                         where a.Name == newcommitDTO.AuthorName
                         select a).First();

        // Assert
        newAuthor.Id.Should().Be(1);
        response.Should().Be(Response.Created);
        newCommitId.Should().Be(1);
        _context.Repos.Where(r => r.Id == 1).First().AllCommits.Count().Should().Be(1);
    }

    [Fact]
    public async void Create_commit_returns_conflict_async()
    {
        // Arrange
        var firstCommitDTO = new CommitCreateDTO(2, "name", new DateTime());

        // Act
        var (response, firstCommitId) = await _repository.CreateAsync(firstCommitDTO);

        // Assert
        response.Should().Be(Response.Conflict);
        firstCommitId.Should().Be(-1);
    }

    [Fact]
    public async void Read_commit_count_0_async()
    {
        // Arrange

        // Act
        var commitList = await _repository.ReadAsync();

        // Assert
        commitList.Count().Should().Be(0);
    }

    [Fact]
    public async void Read_commit_count_1_async()
    {
        // Arrange

        await _repository.CreateAsync(new CommitCreateDTO(1, "name", DateTime.Now));

        // Act
        var commitList = await _repository.ReadAsync();

        // Assert
        commitList.Count().Should().Be(1);
    }

    [Fact]
    public async void Commit_find_found_async()
    {
        // Arrange
        await _repository.CreateAsync(new CommitCreateDTO(1, "name", DateTime.Now));

        // Act
        var commitInRepo = await _repository.FindAsync(1);

        // Assert
        commitInRepo.Id.Should().Be(1);
        commitInRepo.RepoId.Should().Be(1);
        commitInRepo.AuthorName.Should().Be("name");
    }

    [Fact]
    public async void commit_find_not_found_async()
    {
        // Arrange

        // Act
        var commit = await _repository.FindAsync(100);

        // Assert
        commit.Should().BeNull();
    }

    [Fact]
    public async void Delete_commit_async()
    {
        // Arrange
        await _repository.CreateAsync(new CommitCreateDTO(1, "name", DateTime.Now));

        // Act
        var response = await _repository.DeleteAsync(1);

        // Assert
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public async void Delete_commit_not_found_async()
    {
        // Arrange

        // Act
        var response = await _repository.DeleteAsync(100);

        // Assert
        response.Should().Be(Response.NotFound);
    }
}