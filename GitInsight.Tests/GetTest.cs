namespace GitInsight.Api.Tests;

[TestCaseOrderer("GitInsight.Api.Tests.PriorityOrderer", "GitInsight.Api.Tests")]
public class GetTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GetTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact, TestPriority(0)]
    public async Task GetPrompt()
    {
        var prompt = await _client.GetStringAsync("");

        prompt.Should().Contain("Write GitHub username and GitHub repo name in the url!");
    }

    [Fact, TestPriority(1)]
    public async Task GetJson()
    {
        var json = await _client.GetStringAsync("Miniim98/Assignment00_BDSA_2022");

        json.Should().Contain("Miniim98");
    }

}
