namespace GitInsight.Tests;

public class ForkTest
{
    Program program;
    IConfiguration configuration;
    public ForkTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();
        program = new Program(context);
        configuration = new ConfigurationBuilder().AddUserSecrets<GitInsight.Program>().Build();
    }

    [Fact]
    public void analysisReturnsSomethingAtAll()
    {
        
        var forkNames = new ForkResult("itu-bdsa", "lecture-code", configuration);

        forkNames.Should().NotBeNull();
        forkNames.RepositoryIdentifiers.Count().Should().BeGreaterThanOrEqualTo(1);
        forkNames.RepositoryIdentifiers.Count().Should().BeGreaterThanOrEqualTo(9);
        forkNames.RepositoryIdentifiers.Should().Contain(new RepositoryIdentifier("jskoven", "lecture-code"));
    }

    [Fact(Skip = "Takes up a lot of requests")]
    public void analysisResultReturnsNumberHigherThanPageCount()
    {
        var forkNames = new ForkResult("processing", "p5.js", configuration);
        forkNames.RepositoryIdentifiers.Count().Should().BeGreaterThanOrEqualTo(2000);
    }

    [Fact]
    public void RESTfulAPIIncludesForkInfo()
    {
        var forkNames = program.Run("itu-bdsa", "lecture-code");
        forkNames.Should().Contain("jskoven");
        forkNames.Should().Contain("Clara-Lyngeraa");
        forkNames.Should().Contain("duckth");
        forkNames.Should().Contain("fredpetersen");
        forkNames.Should().Contain("katrinesando");
        forkNames.Should().Contain("LysetsDal");
        forkNames.Should().Contain("SDeLaVida");
        forkNames.Should().Contain("TheNooby127");
        forkNames.Should().Contain("viggostarcke");
    }
}