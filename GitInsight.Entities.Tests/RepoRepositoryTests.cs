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
        var repo = new RepoCreateDTO("name", 0, new List<int>());
        // Act
        var (response, repoId) = _repository.Create(repo);

        // Assert
        Assert.Equal(Response.Created, response);
        Assert.Equal(1, repoId);
    }

    [Fact]
    public void Create_repo_returns_conflict(){
        // Arrange
        var repo_1 = new RepoCreateDTO("name", 0, new List<int>());
        var repo_2 = new RepoCreateDTO("name", 0, new List<int>());
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

        // Act
        var repoList = _repository.Read();

        // Assert
        Assert.True(repoList.Count() == 0);
    }

    [Fact]
    public void Read_repo_count_1() {
        // Arrange
        var repo = new Repo("name", 0);
        _context.Repos.Add(repo);
        _context.SaveChanges();

        // Act
        var repoList = _repository.Read();

        // Assert
        Assert.True(repoList.Count() == 1);
    }

    [Fact]
    public void repo_find_found() {
        // Arrange
        var repo = new Repo("name", 0);
        _context.Repos.Add(repo);
        _context.SaveChanges();
        
        // Act
        var repoInRepo = _repository.Find(1);

        // Assert()
        Assert.Equal(new RepoDTO(repo.Id, repo.Name, repo.LatestCommit, getCommitsAsDTOList(repo.AllCommits)), repoInRepo);
    }

    [Fact]
    public void repo_find_not_found() {
        // Arrange
        
        // Act
        var repo = _repository.Find(100);

        // Assert
        Assert.Null(repo);
    }

    [Fact]
    public void Update_repo_new_commits()
    {
        // Arrange
        var repo = new Repo("name", 0);
        _context.Repos.Add(repo);
        _context.SaveChanges();

        var repoUpdate = new RepoUpdateDTO(repo.Id, repo.Name, repo.LatestCommit, getCommitsAsDTOList(repo.AllCommits));

        // Act
        var response = _repository.Update(repoUpdate);

        // Assert
        response.Should().Be(Response.Updated);
    }

    [Fact]
    public void Update_repo_no_new_commits()
    {
        // Arrange
        var repo = new Repo("name", 0);
        _context.Repos.Add(repo);
        _context.SaveChanges();

        var repoUpdate = new RepoUpdateDTO(repo.Id, repo.Name, repo.LatestCommit, getCommitsAsDTOList(repo.AllCommits));

        // Act
        var response = _repository.Update(repoUpdate);

        // Assert
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Update_nonexistant_repo(){
        // Arrange
        var repo = new Repo("name", 0);
        _context.Repos.Add(repo);
        _context.SaveChanges();
        var author = new Author("name");
        _context.Authors.Add(author);
        _context.SaveChanges();
        var commit = new Commit(){Repo = repo, Author = author, Date = new DateTime()};

        var repoUpdate = new RepoUpdateDTO(repo.Id, repo.Name, repo.LatestCommit, getCommitsAsDTOList(repo.AllCommits));

        // Act
        var response = _repository.Update(repoUpdate);

        // Assert
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public void Delete_repo()
    {
        // Arrange
        var repo = new Repo("name", 0);
        _context.Repos.Add(repo);
        _context.SaveChanges();

        // Act
        var response = _repository.Delete(1);

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