namespace GitInsight.Api.Tests
{
    [TestCaseOrderer("GitInsight.Api.Tests.PriorityOrderer", "GitInsight.Api.Tests")]
    public class GetTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthorTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact, TestPriority(0)]
        public async Task GetFrequencyResults()
        {
            var frequencyResult = await _client.GetFromJsonAsync<FrequencyResult>("frequency_results");

            frequencyResult.Data.Should().Contain(new EntryF(3, "2022-09-04T00:00:00"));
        }

        [Fact, TestPriority(1)]
        public async Task GetAuthorResults()
        {
            var frequencyResult = await _client.GetFromJsonAsync<FrequencyResult>("frequency_results");
            var authorResult = await _client.GetFromJsonAsync<AuthorResult>("author_result");

            authorResult.Data.Should().Contain(new Entry("Miniim98", frequencyResult));
        }

    }
}