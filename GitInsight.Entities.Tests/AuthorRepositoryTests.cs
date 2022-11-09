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
        var repo = new Repo("RepoName");

        _context = context;
        _repository = new AuthorRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void Create_author_returns_created(){
        // Arrange
        var newAuthorDTO = new AuthorCreateDTO("name", new List<int>());

        // Act
        var (response, newAuthorId) = _repository.Create(newAuthorDTO);

        // Assert
        response.Should().Be(Response.Created);
        newAuthorId.Should().Be(1);
    }

    [Fact]
    public void Create_author_returns_conflict(){
        // Arrange
        var name = "name";
        _repository.Create(new AuthorCreateDTO(name, new List<int>()));

        var secondAuthorDTO = new AuthorCreateDTO(name, new List<int>());

        // Act
        var (response, secondAuthorId) = _repository.Create(secondAuthorDTO);

        // Assert
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public void Read_author_count_0() {
        // Arrange

        // Act
        var authorList = _repository.Read();

        // Assert
        authorList.Count().Should().Be(0);
    }

    [Fact]
    public void Read_author_count_1() {
        // Arrange
        _repository.Create(new AuthorCreateDTO("name", new List<int>()));

        // Act
        var authorList = _repository.Read();

        // Assert
        authorList.Count().Should().Be(1);
    }

    [Fact]
    public void Author_find_found() {
        // Arrange
        var name = "name";
        _repository.Create(new AuthorCreateDTO(name, new List<int>()));
        
        // Act
        var authorInRepo = _repository.Find(1);

        // Assert
        authorInRepo.Id.Should().Be(1);
        authorInRepo.Name.Should().BeEquivalentTo("name");
    }

    [Fact]
    public void Author_find_not_found() {
        // Arrange
        
        // Act
        var author = _repository.Find(100);

        // Assert
        author.Should().BeNull();
    }

    [Fact]
    public void Update_existing_author()
    {
        // Arrange
        var name = "name";
        _repository.Create(new AuthorCreateDTO(name, new List<int>()));

        var authorUpdate = new AuthorUpdateDTO(1, name, new List<int>());

        // Act
        var response = _repository.Update(authorUpdate);

        // Assert
        response.Should().Be(Response.Updated);
    }

    [Fact]
    public void Update_non_existing_author(){
        // Arrange
        var author = new AuthorUpdateDTO(100, "name", new List<int>());

        // Act
        var response = _repository.Update(author);

        // Assert
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public void Delete_author()
    {
        // Arrange
        var newAuthor = new Author("name");
        _context.Authors.Add(newAuthor);
        _context.SaveChanges();

        // Act
        var response = _repository.Delete(1);

        // Assert
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public void Delete_nonexistant_author()
    {
        // Arrange

        // Act
        var response = _repository.Delete(100);

        // Assert
        response.Should().Be(Response.NotFound);
    }
}