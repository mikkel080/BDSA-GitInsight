namespace GitInsight;

using LibGit2Sharp;
using LibGit2Sharp.Handlers;

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

            return JsonConvert.SerializeObject(new CombinedResult(repoObject.FrequencyResult!, repoObject.AuthorResult!), Formatting.Indented);
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

    async Task<int> CreateOrUpdateData(Repository repo, string RepoName)
    {
        var (response, repoId) = await _repositoryRepos.CreateAsync(new RepoCreateDTO(RepoName, new List<int>()));
        if (response == Response.Created)
        {
            await SaveDataAsync(repo, repoId);
        }
        else
        {
            var repoDTO = await _repositoryRepos.FindAsync(repoId);
            var latestDate = await _repositoryCommit.FindAsync(repoDTO.LatestCommit);
            if (repo.Commits.First().Author.When.DateTime != latestDate.Date)
            {
                await UpdateDataAsync(repo, repoId, repoDTO);
            }
        }
        return repoId;
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
}

public record CombinedResult(FrequencyResult FrequencyResult, AuthorResult AuthorResult);