using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

public class ForkResult
{
    public IEnumerable<RepositoryIdentifier> RepositoryIdentifiers {get; init;}
    private string key{get; init;}

    public ForkResult(string githubName, string repoName, IConfiguration configuration)
    {
        key = getKey(configuration);
        var perPage = 100;
        var page = 0;
        var forks = new List<RepositoryIdentifier>();
        while (forks.Count >= page * perPage)
        {
            page++;
            var result = getPage(page, perPage, githubName,repoName);
            foreach (var entry in result!.AsArray())
            {
                foreach (var item in entry!.AsObject())
                {
                    if (item!.Key == "full_name")
                    {
                        var value = item!.Value!.ToString().Split("/");
                        forks.Add(new RepositoryIdentifier(value[0], value[1]));
                    }
                }
            }
        }
        RepositoryIdentifiers = forks;
    }

    private string getKey(IConfiguration configuration)
    {
        var secret = configuration["GITHUBAPI"];

        if (secret == null)
        {
            secret = Environment.GetEnvironmentVariable("GITHUBAPI");
        }

        return secret!;
    }

    private JsonNode getPage(int page, int perPage, string orgName, string repoName)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitInsight", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", key);
        var pageSettings = $"?page={page}&per_page={perPage}";
        var url = $"https://api.github.com/repos/{orgName}/{repoName}/forks{pageSettings}";
        var json = client.GetStringAsync(url);
        var result = JsonSerializer.Deserialize<JsonNode>(json.Result);
        return result!;
    }
}


public record RepositoryIdentifier(string Organization, string Repository);