namespace GitInsight.Entities.Tests;

public sealed class RepoRepositoryTests : IDisposable
{
    private readonly GitInsightContext _context;
    private readonly RepoRepository _repository;

    private ICollection<Commit> getCommitsList(ICollection<int> commitIds) => _context.Commits.Where(c => commitIds.Contains(c.Id)).ToList();

    private ICollection<int> getCommitsAsDTOList(ICollection<Commit> commits) => commits.Select(c => c.Id).ToList();
    public RepoRepositoryTests() {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();

        var author = new Author("AuthorName");
        var repo = new Repo("RepoName");
        var commit = new Commit(){Repo = repo, Author = author, Date = DateTime.Now};
        var commit2 = new Commit(){Repo = repo, Author = author, Date = DateTime.Now.AddMilliseconds(10)};
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
    public void Create_repo_returns_created(){
        // Arrange
        var repo = new RepoCreateDTO("name", new List<int>());
        // Act
        var (response, repoId) = _repository.Create(repo);

        // Assert
        response.Should().Be(Response.Created);
        repoId.Should().Be(2);
    }

    [Fact]
    public void Create_repo_returns_conflict(){
        // Arrange
        var repo_1 = new RepoCreateDTO("name", new List<int>());
        var repo_2 = new RepoCreateDTO("name", new List<int>());
        // Act
        var (response_1, repo_1_Id) = _repository.Create(repo_1);
        var (response_2, repo_2_Id) = _repository.Create(repo_2);

        // Assert
        Assert.Equal(Response.Created, response_1);
        Assert.Equal(Response.Conflict, response_2);
    }

    [Fact]
    public void Read_repo_count_0() {
        // Arrange
        _repository.Delete(1);
        // Act
        var repoList = _repository.Read();

        // Assert
        repoList.Count().Should().Be(0);
    }

    [Fact]
    public void Read_repo_count_1() {
        // Act
        var repoList = _repository.Read();

        // Assert
        repoList.Count().Should().Be(1);
    }

    [Fact]
    public void repo_find_found() {
        // Arrange
        _repository.Create(new RepoCreateDTO("name", new List<int>()));
        
        // Act
        var repoInRepo = _repository.Find(2);

        // Assert
        repoInRepo.Id.Should().Be(2);
        repoInRepo.Name.Should().BeEquivalentTo("name");
    }

    [Fact]
    public void repo_find_not_found() {
        // Arrange
        
        // Act
        var repo = _repository.Find(100);

        // Assert
        repo.Should().BeNull();
    }

    [Fact]
    public void Update_repo_new_commits()
    {
        // Arrange
        _repository.Create(new RepoCreateDTO("name", new List<int>()));
        var repoUpdate = new RepoUpdateDTO(2, "name", 0, new List<int>(){1, 2});

        // Act
        var response = _repository.Update(repoUpdate);

        // Assert

        response.Should().Be(Response.Updated);
        _context.Commits.Where(c => c.Id == 1).First().RepoID.Should().Be(2);
        _repository.Find(2).LatestCommit.Should().Be(2);
    }

    [Fact]
    public void Update_repo_no_new_commits()
    {
        // Arrange
        var repo = new Repo("name");
        _context.Repos.Add(repo);
        _context.SaveChanges();

        var repoUpdate = new RepoUpdateDTO(repo.Id, repo.Name, repo.LatestCommit, new List<int>());

        // Act
        var response = _repository.Update(repoUpdate);

        // Assert
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Update_nonexistant_repo(){
        // Arrange

        var repoUpdate = new RepoUpdateDTO(100, "name", 0, new List<int>(){2});

        // Act
        var response = _repository.Update(repoUpdate);

        // Assert
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public void Delete_repo()
    {
        // Arrange
        var repo = new Repo("name");
        _context.Repos.Add(repo);
        _context.SaveChanges();

        // Act
        var response = _repository.Delete(2);

        // Assert
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public void Delete_repo_not_found()
    {
        // Arrange

        // Act
        var response = _repository.Delete(100);

        // Assert
        response.Should().Be(Response.NotFound);
    }
}