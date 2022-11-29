namespace GitInsight.Entities.Tests;

public sealed class AuthorRepositoryTests : IDisposable
{
    private readonly GitInsightContext _context;
    private readonly AuthorRepository _repository;
    public AuthorRepositoryTests()
    {
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
    public async void Create_author_returns_created_async()
    {
        // Arrange
        var newAuthorDTO = new AuthorCreateDTO("name", new List<int>());

        // Act
        var (response, newAuthorId) = await _repository.CreateAsync(newAuthorDTO);

        // Assert
        response.Should().Be(Response.Created);
        newAuthorId.Should().Be(1);
    }

    [Fact]
    public async void Create_author_returns_conflict_async()
    {
        // Arrange
        var name = "name";
        await _repository.CreateAsync(new AuthorCreateDTO(name, new List<int>()));

        var secondAuthorDTO = new AuthorCreateDTO(name, new List<int>());

        // Act
        var (response, secondAuthorId) = await _repository.CreateAsync(secondAuthorDTO);

        // Assert
        response.Should().Be(Response.Conflict);
    }

    [Fact]
    public async void Read_author_count_0_async()
    {
        // Arrange

        // Act
        var authorList = await _repository.ReadAsync();

        // Assert
        authorList.Count().Should().Be(0);
    }

    [Fact]
    public async void Read_author_count_1_async()
    {
        // Arrange
        await _repository.CreateAsync(new AuthorCreateDTO("name", new List<int>()));

        // Act
        var authorList = await _repository.ReadAsync();

        // Assert
        authorList.Count().Should().Be(1);
    }

    [Fact]
    public async void Author_find_found_async()
    {
        // Arrange
        var name = "name";
        await _repository.CreateAsync(new AuthorCreateDTO(name, new List<int>()));

        // Act
        var authorInRepo = await _repository.FindAsync(1);

        // Assert
        authorInRepo.Id.Should().Be(1);
        authorInRepo.Name.Should().BeEquivalentTo("name");
    }

    [Fact]
    public async void Author_find_not_found_async()
    {
        // Arrange

        // Act
        var author = await _repository.FindAsync(100);

        // Assert
        author.Should().BeNull();
    }

    [Fact]
    public async void Update_existing_author_async()
    {
        // Arrange
        var name = "name";
        await _repository.CreateAsync(new AuthorCreateDTO(name, new List<int>()));

        var authorUpdate = new AuthorUpdateDTO(1, name, new List<int>());

        // Act
        var response = await _repository.UpdateAsync(authorUpdate);

        // Assert
        response.Should().Be(Response.Updated);
    }

    [Fact]
    public async void Update_non_existing_author_async()
    {
        // Arrange
        var author = new AuthorUpdateDTO(100, "name", new List<int>());

        // Act
        var response = await _repository.UpdateAsync(author);

        // Assert
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public async void Delete_author_async()
    {
        // Arrange
        var newAuthor = new Author("name");
        _context.Authors.Add(newAuthor);
        await _context.SaveChangesAsync();

        // Act
        var response = await _repository.DeleteAsync(1);

        // Assert
        response.Should().Be(Response.Deleted);
    }

    [Fact]
    public async void Delete_nonexistant_author_async()
    {
        // Arrange

        // Act
        var response = await _repository.DeleteAsync(100);

        // Assert
        response.Should().Be(Response.NotFound);
    }
}