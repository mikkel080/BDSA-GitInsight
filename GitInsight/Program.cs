namespace GitInsight;

using LibGit2Sharp;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

public sealed class Program
{
    private readonly GitInsightContext _context;
    private readonly AuthorRepository _repositoryAuthor;
    private readonly CommitRepository _repositoryCommit;
    private readonly RepoRepository _repositoryRepos;

    public Program(GitInsightContext context)
    {
        _context = context;
        _repositoryAuthor = new AuthorRepository(_context);
        _repositoryCommit = new CommitRepository(_context);
        _repositoryRepos = new RepoRepository(_context);
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

        return existingPath;
    }

    public string Run(string githubName, string repoName)
    {
        using (var repo = new Repository(getPathOrCloneRepo(githubName, repoName)))
        {
            var repoId = SaveOrUpdateTheData(repo, repoName);
            var commits = _repositoryCommit.Read().Where(c => c.RepoId == repoId).ToList();

            //Cursed but easy way
            var repoObject = _context.Repos.Where(r => r.Id == repoId).First();

            return JsonConvert.SerializeObject(new CombinedResult(repoObject.FrequencyResult!, repoObject.AuthorResult!), Formatting.Indented);

        }
    }

    int SaveOrUpdateTheData(Repository repo, string RepoName)
    {
        var (response, repoId) = _repositoryRepos.Create(new RepoCreateDTO(RepoName, new List<int>()));
        if (repoId == -1)
        {
            return _repositoryRepos.Read().Where(r => r.Name == repo.Info.Path).First().Id;
        }
        foreach (var commit in repo.Commits.ToList())
        {
            _repositoryCommit.Create(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.Date));
        }
        var repoDTO = _repositoryRepos.Find(repoId);
        _repositoryRepos.Update(new RepoUpdateDTO(repoDTO.Id, repoDTO.Name, repoDTO.LatestCommit, repoDTO.AllCommits));
        return repoId;
    }
    
    public object forkAnalysis(string githubName, string repoName)
    {
        using HttpClient client = new();

        var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var secret = configuration.GetSection("GITHUBAPI").Value;

        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitInsight", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", secret);

        var url = $"https://api.github.com/repos/{githubName}/{repoName}/forks";
        var json = client.GetStringAsync(url);

        return JsonConvert.DeserializeObject(json.Result)!;   
    }
}

public record CombinedResult(FrequencyResult FrequencyResult, AuthorResult AuthorResult);