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

    [Fact]
    public void analysisReturnsSomethingAtAll()
    {
        var forkNames = program.forkAnalysis("itu-bdsa", "project-description");

        forkNames.Should().NotBeNull();
        forkNames.Count().Should().BeGreaterThanOrEqualTo(1);
        forkNames.Count().Should().BeGreaterThanOrEqualTo(37);
        forkNames.Should().Contain("JonasUJ/project-description");
    }
}