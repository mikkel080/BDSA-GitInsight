namespace GitInsight.Entities.Tests;

public sealed class AuthorRepositoryTests : IDisposable
{
    private readonly GitInsightContext _context;
    private readonly AuthorRepository _repository;

    private ICollection<Commit> getCommitsList(ICollection<int> commitIds) => _context.Commits.Where(c => commitIds.Contains(c.Id)).ToList();

    private ICollection<int> getCommitsAsDTOList(ICollection<Commit> commits) => commits.Select(c => c.Id).ToList();
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

    [Fact]
    public void Create_author_returns_created(){
        // Arrange
        var newAuthorDTO = new AuthorCreateDTO("name", "email", new List<int>());

        // Act
        var (response, newAuthorId) = _repository.Create(newAuthorDTO);

        // Assert
        Assert.Equal(Response.Created, response);
        Assert.Equal(1, newAuthorId);
    }

    [Fact]
    public void Create_author_returns_conflict(){
        // Arrange
        var anEmail = "email";
        var firstAuthor = new Author("name_1", anEmail, new List<Commit>());
        _context.Authors.Add(firstAuthor);
        _context.SaveChanges();

        var secondAuthorDTO = new AuthorCreateDTO("name_2", anEmail, new List<int>());

        // Act
        var (response, secondAuthorId) = _repository.Create(secondAuthorDTO);

        // Assert
        response.Should().Be(Response.Conflict);
        secondAuthorId.Should().Be(1);
    }

    [Fact]
    public void Read_author_count_0() {
        // Arrange

        // Act
        var authorList = _repository.Read();

        // Assert
        Assert.True(authorList.Count() == 0);
    }

    [Fact]
    public void Read_author_count_1() {
        // Arrange
        var newAuthor = new Author("name", "email", new List<Commit>());
        _context.Authors.Add(newAuthor);
        _context.SaveChanges();

        // Act
        var authorList = _repository.Read();

        // Assert
        Assert.True(authorList.Count() == 1);
    }

    [Fact]
    public void Author_find_found() {
        // Arrange
        Author author = new Author("name", "email", new List<Commit>());
        _context.Authors.Add(author);
        _context.SaveChanges();
        
        // Act
        var authorInRepo = _repository.Find(1);

        // Assert()
        Assert.Equal(new AuthorDTO(author.Id, author.Name, author.Email, new List<int>()), authorInRepo);
    }

    [Fact]
    public void Author_find_not_found() {
        // Arrange
        
        // Act
        var author = _repository.Find(100);

        // Assert
        Assert.Null(author);
    }

    [Fact]
    public void Update_existing_author()
    {
        // Arrange
        var newauthor = new Author("name", "email", new List<Commit>());  
        _context.Authors.Add(newauthor);
        _context.SaveChanges();  

        var authorUpdate = new AuthorUpdateDTO(1, "name", "email", new List<int>());

        // Act
        var response = _repository.Update(authorUpdate);

        // Assert
        response.Should().Be(Response.Updated);
    }

    [Fact]
    public void Update_non_existing_author(){
        // Arrange
        var author = new AuthorUpdateDTO(100, "name", "email", new List<int>());

        // Act
        var response = _repository.Update(author);

        // Assert
        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public void Delete_author()
    {
        // Arrange
        var newAuthor = new Author("name", "email", new List<Commit>());
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