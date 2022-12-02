namespace GitInsight;

using System.Text.Json;
using Microsoft.Extensions.Configuration;

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
        string existingPath = path + @$"{repoName}";
        if (!Directory.Exists(existingPath) || !Repository.IsValid(existingPath))
        {
            return Repository.Clone($"https://github.com/{githubName}/{repoName}.git", path + $"{repoName}");
        }
        return existingPath;
    }

    public string Run(string githubName, string repoName)
    {
        using (var repo = new Repository(getPathOrCloneRepo(githubName, repoName)))
        {
            int repoId;
            CheckForGitUpdates(repo);
            repoId = CreateOrUpdateData(repo, githubName + "/" + repoName);

            var repoDTO = _repositoryRepos.FindAsync(repoId).Result;
            var RepositoryIdentifier = new RepositoryIdentifier(githubName, repoName);

            var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var ForkResult = new ForkResult(githubName, repoName, configuration);

            var options = new JsonSerializerOptions { WriteIndented = true };
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
            SaveData(repo, repoId);
        }

        else if (response == Response.AlreadyExists)
        {
            var repoDTO = _repositoryRepos.FindAsync(repoId).Result;
            var latestDate = _repositoryCommit.FindAsync(repoDTO.LatestCommit).Result;
            if (repo.Commits.First().Author.When.DateTime.Date != latestDate.Date)
            {
                UpdateData(repo, repoId, repoDTO);
            }
        }
        return repoId;
    }

    void SaveData(Repository repo, int repoId)
    {
        foreach (var commit in repo.Commits.ToList())
        {
            var reponseCreate = _repositoryCommit.CreateAsync(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.DateTime)).Result;
        }
        var repoDTO = _repositoryRepos.FindAsync(repoId).Result;
        _resultHandler.UpdateDateBaseWithResults(repoId);
    }

    void UpdateData(Repository repo, int repoId, RepoDTO repoDTO)
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
}

public record CombinedResult(RepositoryIdentifier RepositoryIdentifier, FrequencyResult FrequencyResult, AuthorResult AuthorResult, ForkResult ForkResult);

