namespace GitInsight;

using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class Program
{
    private readonly GitInsightContext _context;
    private readonly AuthorRepository _repositoryAuthor;
    private readonly CommitRepository _repositoryCommit;
    private readonly RepoRepository _repositoryRepos;
    private ResultHandler _resultHandler;

    public Program(GitInsightContext context)
    {
        _context = context;
        _repositoryAuthor = new AuthorRepository(_context);
        _repositoryCommit = new CommitRepository(_context);
        _repositoryRepos = new RepoRepository(_context);
        _resultHandler = new ResultHandler(_context, _repositoryCommit, _repositoryRepos);
    }

    public string getPathOrCloneRepo(string githubName, string repoName)
    {
        //Temp folders does not get deleted themselves so remember to delete
        var path = Path.GetTempPath();
        string existingPath;
        if (path.Contains(@"\"))
        {
            existingPath = path + @$"\{repoName}";
        }
        else
        {
            existingPath = path + @$"/{repoName}";

        }
        if (!Directory.Exists(existingPath))
        {
            return Repository.Clone($"https://github.com/{githubName}/{repoName}.git", path + $"{repoName}");
        }
        else if (Repository.IsValid(existingPath))
        {
            return existingPath;
        }
        return "";
    }

    public string Run(string githubName, string repoName)
    {
        using (var repo = new Repository(getPathOrCloneRepo(githubName, repoName)))
        {
            int repoId;
            CheckForGitUpdates(repo);
            repoId = CreateOrUpdateData(repo, githubName + "/" + repoName);

            var repoDTO= _repositoryRepos.FindAsync(repoId).Result;
            var RepositoryIdentifier = new RepositoryIdentifier(githubName, repoName);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var ForkResult = forkAnalysis(githubName, repoName);
            var CombinedResult = new CombinedResult(RepositoryIdentifier, repoDTO.FrequencyResult!, repoDTO.AuthorResult!, ForkResult);
            return JsonSerializer.Serialize(CombinedResult, options);
        }
    }

    void CheckForGitUpdates(Repository repo)
    {
        // Credential information to fetch
        LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
        options.FetchOptions = new FetchOptions();
        options.FetchOptions.CredentialsProvider = new CredentialsHandler(
        (url, usernameFromUrl, types) =>
            new UsernamePasswordCredentials()
            {
                Username = "USERNAME",
                Password = "PASSWORD"
            });

        // User information to create a merge commit
        var signature = new LibGit2Sharp.Signature(
            new Identity("MERGE_USER_NAME", "MERGE_USER_EMAIL"), DateTimeOffset.Now);

        // Pull
        Commands.Pull(repo, signature, options);
    }

    int CreateOrUpdateData(Repository repo, string RepoName)
    {
        var (response, repoId) = _repositoryRepos.CreateAsync(new RepoCreateDTO(RepoName, new List<int>())).Result;
        if (response == Response.Created)
        {
            SaveDataAsync(repo, repoId);
        }
        else
        {
            var repoDTO = _repositoryRepos.FindAsync(repoId).Result;
            var latestDate = _repositoryCommit.FindAsync(repoDTO.LatestCommit).Result;
            if (repo.Commits.First().Author.When.DateTime != latestDate.Date)
            {
                UpdateDataAsync(repo, repoId, repoDTO);
            }
        }
        return repoId;
    }

    void SaveDataAsync(Repository repo, int repoId)
    {
        foreach (var commit in repo.Commits.ToList())
        {
            var reponseCreate = _repositoryCommit.CreateAsync(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.DateTime)).Result;
        }
        var repoDTO = _repositoryRepos.FindAsync(repoId).Result;
        _resultHandler.UpdateDateBaseWithResults(repoId);
    }

    void UpdateDataAsync(Repository repo, int repoId, RepoDTO repoDTO)
    {
        var allCommits = _repositoryCommit.ReadAsync().Result;
        var currentCommits = allCommits.Where(c => c.RepoId == repoId).Select(c => c.Date);

        foreach (var commit in repo.Commits.ToList())
        {
            if (currentCommits.Contains(commit.Author.When.DateTime)) { continue; }
            else
            {
                var responseCreate = _repositoryCommit.CreateAsync(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.DateTime)).Result;
            }
        }
        _resultHandler.UpdateDateBaseWithResults(repoId);
    }

    public ForkResult forkAnalysis(string githubName, string repoName)
    {
        using HttpClient client = new();

        var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var secret = configuration["GITHUBAPI"];

        var envSecret = Environment.GetEnvironmentVariable("GITHUBAPI");

        if (secret == null)
        {
            secret = envSecret;
        }

        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitInsight", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", secret);

        var perPage = 100;
        var page = 0;
        var forks = new List<RepositoryIdentifier>();
        while (forks.Count >= page * perPage)
        {
            page++;
            var pageSettings = $"?page={page}&per_page={perPage}";
            var url = $"https://api.github.com/repos/{githubName}/{repoName}/forks{pageSettings}";
            var json = client.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<JsonNode>(json.Result);

            foreach (var entry in result!.AsArray())
            {
                foreach (var item in entry!.AsObject())
                {
                    if (item!.Key == "full_name")
                    {
                        var value = item!.Value!.ToString().Split("/");
                        var Organization = value[0];
                        var Repository = value[1];
                        var element = new RepositoryIdentifier(Organization, Repository);
                        forks.Add(element);
                    }
                }
            }
        }
        return new ForkResult(forks);
    }
}

public record CombinedResult(RepositoryIdentifier RepositoryIdentifier, FrequencyResult FrequencyResult, AuthorResult AuthorResult, ForkResult ForkResult);
public record RepositoryIdentifier(string Organization, string Repository);
public record ForkResult(IEnumerable<RepositoryIdentifier> RepositoryIdentifiers);
