namespace GitInsight;

using LibGit2Sharp;
using LibGit2Sharp.Handlers;
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
        else if (Repository.IsValid(existingPath))
        {
            return existingPath;
        }
        return "";
    }

    public async Task<string> Run(string githubName, string repoName)
    {
        using (var repo = new Repository(getPathOrCloneRepo(githubName, repoName)))
        {
            int repoId;
            CheckForGitUpdates(repo);
            repoId = await CreateOrUpdateData(repo, repoName);

            //Cursed but easy way to get results
            var repoObject = _context.Repos.Where(r => r.Id == repoId).First();

            var ForkResult = new ForkResult(forkAnalysis(githubName, repoName));

            return JsonConvert.SerializeObject(new CombinedResult(repoObject.FrequencyResult!, repoObject.AuthorResult!,ForkResult), Formatting.Indented);
        }   
    }

    async Task<int> SaveDataAsync(Repository repo, int repoId)
    {
        foreach (var commit in repo.Commits.ToList())
        {
            await _repositoryCommit.CreateAsync(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.DateTime));
        }
        var repoDTO = await _repositoryRepos.FindAsync(repoId);

        await _repositoryRepos.UpdateAsync(new RepoUpdateDTO(repoDTO.Id, repoDTO.Name, repoDTO.LatestCommit, repoDTO.AllCommits));
        return repoId;
    }

    async Task<int> UpdateDataAsync(Repository repo, int repoId, RepoDTO repoDTO)
    {
        var allCommits = await _repositoryCommit.ReadAsync();
        var currentCommits = allCommits.Where(c => c.RepoId == repoId).Select(c => c.Date);

        foreach (var commit in repo.Commits.ToList())
        {
            if (currentCommits.Contains(commit.Author.When.DateTime)) { continue; }
            else
            {
                await _repositoryCommit.CreateAsync(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.DateTime));
            }
        }
        await _repositoryRepos.UpdateAsync(new RepoUpdateDTO(repoDTO.Id, repoDTO.Name, repoDTO.LatestCommit, repoDTO.AllCommits));
        return repoId;
    }

    public IEnumerable<String> forkAnalysis(string githubName, string repoName)
    {
        using HttpClient client = new();

        var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var secret = configuration.GetSection("GITHUBAPI").Value;

        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitInsight", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", secret);

        var perPage = 100;
        int page = 0;
        List<String> forks = new List<String>();
        while (forks.Count >= page * perPage)
        {
            page++;
            var pageSettings = $"?page={page}&per_page={perPage}";
            var url = $"https://api.github.com/repos/{githubName}/{repoName}/forks{pageSettings}";
            var json = client.GetStringAsync(url);
            var result = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(json.Result)!;

            foreach (var entry in result)
            {
                foreach (Newtonsoft.Json.Linq.JProperty? item in entry.Values<Newtonsoft.Json.Linq.JProperty>())
                {
                    if (item!.Name == "full_name")
                    {
                        forks.Add(item!.Value.ToString());
                    }
                }
            }
        }
        return forks;
    }
}

public record ForkResult(IEnumerable<String> Forks);
public record CombinedResult(FrequencyResult FrequencyResult, AuthorResult AuthorResult, ForkResult ForkResult);