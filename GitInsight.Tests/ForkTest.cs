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
    public void JSON_is_returned()
    {
        var RESTCall = program.forkAnalysis("itu-bdsa", "project-description");

        RESTCall.Should().NotBeNull();
        
    }
}