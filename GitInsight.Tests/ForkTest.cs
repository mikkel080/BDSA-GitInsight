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
    public void testTheTruth()
    {
        true.Should().Be(true);
    }

    [Fact(Skip = "Unable to use Secrets with Github Actions at the moment")]
    public void analysisReturnsMoreThanZeroForksGivenAForkedRepository()
    {
        var forkNames = program.forkAnalysis("itu-bdsa", "lecture-code");

        forkNames.Should().NotBeNull();
        forkNames.Count().Should().BeGreaterThanOrEqualTo(1);
        forkNames.Count().Should().BeGreaterThanOrEqualTo(9);
        forkNames.Should().Contain("jskoven/lecture-code");
    }

    [Fact(Skip = "Takes up a lot of requests")]
    public void analysisResultReturnsNumberHigherThanPageCount()
    {
        var forkNames = program.forkAnalysis("processing", "p5.js");
        forkNames.Count().Should().BeGreaterThanOrEqualTo(2000);
    }

    [Fact]
    public void RESTfulAPIIncludesForkInfo()
    {
        var forkNames = program.Run("itu-bdsa", "lecture-code");
        forkNames.Result.Should().Contain("jskoven/lecture-code");
    }
}