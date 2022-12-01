namespace GitInsight;

public class ResultHandler
{
    private readonly GitInsightContext _context;
    private readonly CommitRepository _repositoryCommit;
    private readonly RepoRepository _repositoryRepos;
    public ResultHandler(GitInsightContext context, CommitRepository commitRepository, RepoRepository repoRepository)
    {
        _context = context;
        _repositoryCommit = commitRepository;
        _repositoryRepos = repoRepository;
    }

    public AuthorResult CalculateAuthorResult(IEnumerable<CommitDTO> list, string repoName)
    {
        var Data = new List<Entry>();
        var q = list.GroupBy(
            (item => item.AuthorName),
            (key, elements) => new
            {
                key = key,
                items = elements
            }
        );
        foreach (var commit in q)
        {
            Data.Add(new Entry(commit.key, CalculateFrequencyResult(commit.items, repoName)));
        }
        return new AuthorResult(Data, repoName);
    }


    public FrequencyResult CalculateFrequencyResult(IEnumerable<CommitDTO> list, string repoName)
    {
        var Data = new List<EntryF>();
        var q = list.GroupBy(
            (item => item.Date.Date),
            (key, elements) => new
            {
                key = key,
                count = elements.Distinct().Count()
            }
        );

        foreach (var commit in q)
        {
            Data.Add(new EntryF(commit.count, commit.key));
        }
        return new FrequencyResult(Data, repoName);
    }

    public Response UpdateDateBaseWithResults(int repoId)
    {
        var repo = _repositoryRepos.FindAsync(repoId).Result;
        var newAuthorResult = CalculateAuthorResult(_repositoryCommit.ReadByRepoIdAsync(repoId).Result, repo.Name);
        var newFrequencyResult = CalculateFrequencyResult(_repositoryCommit.ReadByRepoIdAsync(repoId).Result, repo.Name);
        return _repositoryRepos.UpdateAsync(new RepoUpdateDTO(repo.Id, repo.Name, repo.LatestCommit, repo.AllCommits, newAuthorResult, newFrequencyResult)).Result;
    }
}