namespace GitInsight.Entities;
using GitInsight.Core;

public class CommitRepository : ICommitRepository {
    
    private readonly GitInsightContext _context;

    public CommitRepository(GitInsightContext context)
    {
        _context = context;
    }
}