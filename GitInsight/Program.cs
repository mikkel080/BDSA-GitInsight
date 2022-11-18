namespace GitInsight;

using LibGit2Sharp;


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

    public async Task<string> Run(string githubName, string repoName)
    {
        using (var repo = new Repository(getPathOrCloneRepo(githubName, repoName)))
        {
            var repoId = await SaveOrUpdateTheDataAsync(repo, repoName);

            //Cursed but easy way to get results
            var repoObject = _context.Repos.Where(r => r.Id == repoId).First();

            return JsonConvert.SerializeObject(new CombinedResult(repoObject.FrequencyResult!, repoObject.AuthorResult!), Formatting.Indented);
        }
    }

    async Task<int> SaveOrUpdateTheDataAsync(Repository repo, string RepoName)
    {
        var (response, repoId) = await _repositoryRepos.CreateAsync(new RepoCreateDTO(RepoName, new List<int>()));
        if (repoId == -1)
        {
            var repoDTOExists = await _repositoryRepos.FindAsync(repoId);
            var commitLatestExists = await _repositoryCommit.FindAsync(repoDTOExists.LatestCommit);

            if (!commitLatestExists.Date.Equals(repo.Commits.First().Author.When.Date))
            {
                await _repositoryRepos.UpdateAsync(new RepoUpdateDTO(repoDTOExists.Id, repoDTOExists.Name, repoDTOExists.LatestCommit, repoDTOExists.AllCommits));
            }
            var res = _repositoryRepos.ReadAsync().Result;
            return res.Where(r => r.Name == RepoName).First().Id;
        }
        foreach (var commit in repo.Commits.ToList())
        {
            await _repositoryCommit.CreateAsync(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.Date));
        }
        var repoDTO = await _repositoryRepos.FindAsync(repoId);

        await _repositoryRepos.UpdateAsync(new RepoUpdateDTO(repoDTO.Id, repoDTO.Name, repoDTO.LatestCommit, repoDTO.AllCommits));


        return repoId;
    }
}

public record CombinedResult(FrequencyResult FrequencyResult, AuthorResult AuthorResult);