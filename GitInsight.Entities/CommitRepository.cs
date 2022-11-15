namespace GitInsight.Entities;
using GitInsight.Core;

public class CommitRepository : ICommitRepository
{

    private readonly GitInsightContext _context;

    public CommitRepository(GitInsightContext context)
    {
        _context = context;
    }

    public (Response Response, int CommitID) Create(CommitCreateDTO commit)
    {
        var repo = _context.Repos.FirstOrDefault(r => r.Id == commit.RepoID);

        if (repo is null)
        {
            return (Response.Conflict, -1);
        }

        var entity = new Commit()
        {
            Repo = repo,
            Author = FindOrCreateAuthor(commit.AuthorName),
            Date = commit.Date
        };
        _context.Commits.Add(entity);
        _context.SaveChanges();

        return (Response.Created, entity.Id);
    }

    public IReadOnlyCollection<CommitDTO> Read()
    {
        var commits = from c in _context.Commits
                      select new CommitDTO(c.Id, c.Repo.Id, c.Author.Name, c.Date);
        return commits.ToList();
    }

    public CommitDTO Find(int commitId)
    {
        var commit = (from c in _context.Commits
                      where c.Id == commitId
                      select c).FirstOrDefault();
        if (commit is null)
        {
            return null!;
        }
        else
        {
            return new CommitDTO(commit.Id, commit.Repo.Id, commit.Author.Name, commit.Date);
        }
    }

    public Response Delete(int commitId)
    {
        var entity = _context.Commits.Find(commitId);

        if (entity is null)
        {
            return Response.NotFound;
        }

        _context.Commits.Remove(entity);
        _context.SaveChanges();

        return Response.Deleted;
    }

    private Author FindOrCreateAuthor(string name) => _context.Authors.Where(a => a.Name == name).FirstOrDefault() ?? new Author(name);

}
