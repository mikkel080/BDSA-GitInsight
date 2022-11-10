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
    public void Create_commit_returns_created(){
      
        //Arrange
        var newcommitDTO = new CommitCreateDTO(1, "nameAuthor", new DateTime());

        // Act
        var (response, newCommitId) = _repository.Create(newcommitDTO);
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
    public void Create_commit_returns_conflict(){
        // Arrange
        var firstCommitDTO = new CommitCreateDTO(2, "name", new DateTime());

        // Act
        var (response, firstCommitId) = _repository.Create(firstCommitDTO);

        // Assert
        response.Should().Be(Response.Conflict);
        firstCommitId.Should().Be(-1);
    }

    [Fact]
    public void Read_commit_count_0() {
        // Arrange

        // Act
        var commitList = _repository.Read();

        // Assert
        commitList.Count().Should().Be(0);
    }

    [Fact]
    public void Read_commit_count_1() {
        // Arrange

        _repository.Create(new CommitCreateDTO(1, "name", DateTime.Now));

        // Act
        var commitList = _repository.Read();

        // Assert
        commitList.Count().Should().Be(1);
    }

    [Fact]
    public void Commit_find_found() {
        // Arrange
        _repository.Create(new CommitCreateDTO(1, "name", DateTime.Now));
        
        // Act
        var commitInRepo = _repository.Find(1);

        // Assert
        commitInRepo.Id.Should().Be(1);
        commitInRepo.RepoId.Should().Be(1);
        commitInRepo.AuthorName.Should().Be("name");
    }

    [Fact]
    public void commit_find_not_found() {
        // Arrange
        
        // Act
        var commit = _repository.Find(100);

        // Assert
        commit.Should().BeNull();
    }

    [Fact]
    public void Delete_commit()
    {
        // Arrange
        _repository.Create(new CommitCreateDTO(1, "name", DateTime.Now));

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