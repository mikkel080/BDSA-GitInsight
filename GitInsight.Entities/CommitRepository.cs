namespace GitInsight.Entities;
using GitInsight.Core;

public class CommitRepository : ICommitRepository
{

    private readonly GitInsightContext _context;

    public CommitRepository(GitInsightContext context)
    {
        _context = context;
    }

    public async Task<(Response Response, int CommitID)> CreateAsync(CommitCreateDTO commit)
    {
        var repo = await _context.Repos.FirstOrDefaultAsync(r => r.Id == commit.RepoID);

        if (repo is null)
        {
            return (Response.Conflict, -1);
        }

        var entity = new Commit()
        {
            Repo = repo,
            Author = await FindOrCreateAuthorAsync(commit.AuthorName),
            Date = commit.Date
        };
        _context.Commits.Add(entity);
        await _context.SaveChangesAsync();

        return (Response.Created, entity.Id);
    }

    public async Task<IReadOnlyCollection<CommitDTO>> ReadAsync()
    {
        var commits = from c in _context.Commits
                      select new CommitDTO(c.Id, c.Repo.Id, c.Author.Name, c.Date);
        return await commits.ToListAsync();
    }

    public async Task<CommitDTO> FindAsync(int commitId)
    {
        var commit = await (from c in _context.Commits
                            where c.Id == commitId
                            select new CommitDTO(c.Id, c.Repo.Id, c.Author.Name, c.Date)).FirstOrDefaultAsync();
        return commit!;
    }

    public async Task<Response> DeleteAsync(int commitId)
    {
        var entity = await _context.Commits.FindAsync(commitId);

        if (entity is null)
        {
            return Response.NotFound;
        }

        _context.Commits.Remove(entity);
        await _context.SaveChangesAsync();

        return Response.Deleted;
    }

    private async Task<Author> FindOrCreateAuthorAsync(string name) => await _context.Authors.Where(a => a.Name == name).FirstOrDefaultAsync() ?? new Author(name);

}
