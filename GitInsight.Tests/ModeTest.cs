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
    public void CommitFrequencyAsync()
    {
        var output = program.Run("Miniim98", "Assignment00_BDSA_2022");

        output.Should().Contain("2022-09-04T00:00:00");
        output.Should().Contain("\"Count\": 3");
    }

    [Fact]
    public void CommitAuthor()
    {
        var output = program.Run("Miniim98", "Assignment00_BDSA_2022");

        output.Should().Contain("Amalie (amdh)");
        output.Should().Contain("2022-09-04T00:00:00");

    }

    [Fact]
    public void test_if_string_is_json()
    {
        var output = program.Run("Miniim98", "Assignment00_BDSA_2022");
        JsonConvert.DeserializeObject(output).Should().NotBe(null);
    }
}