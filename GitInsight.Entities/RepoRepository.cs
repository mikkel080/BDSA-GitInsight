namespace GitInsight.Entities;
using GitInsight.Core;

public class RepoRepository : IRepoRepository {

    private readonly GitInsightContext _context;

    public RepoRepository(GitInsightContext context)
    {
        _context = context;
    }

}