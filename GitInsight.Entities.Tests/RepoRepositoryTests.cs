namespace GitInsight.Entities.Tests;

public sealed class RepoRepositoryTests : IDisposable
{
    private readonly GitInsightContext _context;
    private readonly RepoRepository _repository;

    private ICollection<Commit> getCommitsList(ICollection<int> commitIds) => _context.Commits.Where(c => commitIds.Contains(c.Id)).ToList();

    private ICollection<int> getCommitsAsDTOList(ICollection<Commit> commits) => commits.Select(c => c.Id).ToList();
    public RepoRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();

        var author = new Author("AuthorName");
        var repo = new Repo("RepoName");
        var commit = new Commit() { Repo = repo, Author = author, Date = DateTime.Now };
        var commit2 = new Commit() { Repo = repo, Author = author, Date = DateTime.Now.AddMilliseconds(10) };
        context.Authors.Add(author);
        context.Repos.Add(repo);
        context.Commits.AddRange(commit, commit2);
        context.SaveChanges();

        _context = context;
        _repository = new RepoRepository(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async void Create_repo_returns_created_async()
    {
        // Arrange
        var repo = new RepoCreateDTO("name", new List<int>());
        // Act
        var (response, repoId) = await _repository.CreateAsync(repo);

        // Assert
        response.Should().Be(Response.Created);
        repoId.Should().Be(2);
    }

    [Fact]
    public async void Create_repo_returns_conflict_async()
    {
        // Arrange
        var repo_1 = new RepoCreateDTO("name", new List<int>());
        var repo_2 = new RepoCreateDTO("name", new List<int>());
        // Act
        var (response_1, repo_1_Id) = await _repository.CreateAsync(repo_1);
        var (response_2, repo_2_Id) = await _repository.CreateAsync(repo_2);

        // Assert
        Assert.Equal(Response.Created, response_1);
        Assert.Equal(Response.Conflict, response_2);
    }

    [Fact]
    public async void Read_repo_count_0_async()
    {
        // Arrange
        await _repository.DeleteAsync(1);
        // Act
        var repoList = await _repository.ReadAsync();

        // Assert
        repoList.Count().Should().Be(0);
    }

    [Fact]
    public async void Read_repo_count_1_async()
    {
        // Act
        var repoList = await _repository.ReadAsync();

        // Assert
        repoList.Count().Should().Be(1);
    }

    [Fact]
    public async void repo_find_found_async()
    {
        // Arrange
        await _repository.CreateAsync(new RepoCreateDTO("name", new List<int>()));

        // Act
        var repoInRepo = await _repository.FindAsync(2);

        // Assert
        repoInRepo.Id.Should().Be(2);
        repoInRepo.Name.Should().BeEquivalentTo("name");
    }

    [Fact]
    public async void repo_find_not_found_async()
    {
        // Arrange

        // Act
        var repo = await _repository.FindAsync(100);

        // Assert
        repo.Should().BeNull();
    }

    [Fact]
    public async void Update_repo_new_commits_async()
    {
        // Arrange
        await _repository.CreateAsync(new RepoCreateDTO("name", new List<int>()));
        var repoUpdate = new RepoUpdateDTO(2, "name", 0, new List<int>() { 1, 2 });

        // Act
        var response = await _repository.UpdateAsync(repoUpdate);

        // Assert
        var res = await _repository.FindAsync(2);
        response.Should().Be(Response.Updated);
        _context.Commits.Where(c => c.Id == 1).First().RepoID.Should().Be(2);
        res.LatestCommit.Should().Be(2);
    }

    [Fact]
    public async void Update_nonexistant_repo_async()
    {
        // Arrange

        var repoUpdate = new RepoUpdateDTO(100, "name", 0, new List<int>() { 2 });

        // Act
        var response = await _repository.UpdateAsync(repoUpdate);

        // Assert
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async void Delete_repo_async()
    {
        // Arrange
        var repo = new Repo("name");
        _context.Repos.Add(repo);
        await _context.SaveChangesAsync();

        // Act
        var response = await _repository.DeleteAsync(2);

        // Assert
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public async void Delete_repo_not_found_async()
    {
        // Arrange

        // Act
        var response = await _repository.DeleteAsync(100);

        // Assert
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async void commit_added_to_repo_latest_commit_async()
    {

        _context.Commits.Add(new Commit { RepoID = 1, AuthorID = 1, Date = DateTime.Now.AddHours(1) });
        await _context.SaveChangesAsync();

        var result = await _repository.FindAsync(1);

        result.LatestCommit.Should().Be(3);
    }
}