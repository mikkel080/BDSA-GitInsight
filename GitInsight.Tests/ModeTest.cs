namespace GitInsight.Tests;

public class ModeTest
{
    Program program;
    public ModeTest()
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
    public void CommitFrequency()
    {
        var output = program.Run("Miniim98", "Assignment00_BDSA_2022", "F");

        output.Should().Contain("3 2022-09-02");
    }

    [Fact]
    public void CommitAuthor()
    {
        var output =  program.Run("Miniim98", "Assignment00_BDSA_2022", "A");

        output.Should().Contain("Amalie (amdh)");
        output.Should().Contain("1 2022-09-02");

    }
}