namespace GitInsight.Entities;
using GitInsight.Core;

public class AuthorRepository : IAuthorRepository {

    private readonly GitInsightContext _context;

    public AuthorRepository(GitInsightContext context)
    {
        _context = context;
    }

}