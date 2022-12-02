namespace GitInsight.Tests;

public class ForkTest
{
    Program program;
    public ForkTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();
        program = new Program(context);

    }

    [Fact]
    public void analysisReturnsSomethingAtAll()
    {
        var forkNames = program.forkAnalysis("itu-bdsa", "lecture-code");

        forkNames.Should().NotBeNull();
        forkNames.RepositoryIdentifiers.Count().Should().BeGreaterThanOrEqualTo(1);
        forkNames.RepositoryIdentifiers.Count().Should().BeGreaterThanOrEqualTo(9);
        forkNames.RepositoryIdentifiers.Should().Contain(new RepositoryIdentifier("jskoven", "lecture-code"));
    }

    [Fact(Skip = "Takes up a lot of requests")]
    public void analysisResultReturnsNumberHigherThanPageCount()
    {
        var forkNames = program.forkAnalysis("processing", "p5.js");
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