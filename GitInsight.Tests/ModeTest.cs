namespace GitInsight.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;

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
        var output = program.Run("Miniim98", "Assignment00_BDSA_2022");

        output.Should().Contain("2022-09-04T00:00:00");
        output.Should().Contain("\"Count\": 3");
        output.Should().Contain("\"Count\": 1");
        output.Should().Contain("\"Count\": 4");
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
        JsonSerializer.Deserialize<JsonNode>(output).Should().NotBeNull();
        output.Should().Contain("Assignment00_BDSA_2022");
    }
}