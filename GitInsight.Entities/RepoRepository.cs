namespace GitInsight.Entities;
using GitInsight.Core;

public class RepoRepository : IRepoRepository
{

    private readonly GitInsightContext _context;

    public RepoRepository(GitInsightContext context)
    {
        _context = context;
    }

    public (Response Response, int RepoID) Create(RepoCreateDTO repo)
    {
        var newRepo = _context.Repos.FirstOrDefault(c => c.Name.Equals(repo.Name));

        if (newRepo is null)
        {
            newRepo = new Repo(repo.Name);
            _context.Repos.Add(newRepo);
            _context.SaveChanges();
        }
        else
        {
            return (Response.Conflict, -1);
        }

        return (Response.Created, newRepo.Id);
    }

    public IReadOnlyCollection<RepoDTO> Read()
    {
        var repos = from r in _context.Repos
                    orderby r.Id
                    select new RepoDTO(r.Id, r.Name, r.LatestCommit, r.AllCommits.Select(c => c.Id).ToList());

        return repos.ToArray();
    }

    public RepoDTO Find(int repoId)
    {
        var repo = _context.Repos.FirstOrDefault(r => r.Id == repoId);

        return (repo is not null
            ? new RepoDTO(repo.Id, repo.Name, repo.LatestCommit, repo.AllCommits.Select(c => c.Id).ToList())
            : null)!;
    }

    public Response Update(RepoUpdateDTO repoUpdate)
    {
        var repo = _context.Repos.Find(repoUpdate.Id);

        if (repo is null)
        {
            return Response.NotFound;
        }
        else
        {
            //Merges two lists without duplicates
            var newList = getCommitsList(repoUpdate.AllCommits).ToList().Union(repo.AllCommits).ToList();

            //Saves the new list to a merged and ordered by date list
            repo.AllCommits = newList.OrderBy(c => c.Date).ToList();
            repo.FrequencyResult = new FrequencyResult(repo.AllCommits, repo.Name);
            repo.AuthorResult = new AuthorResult(repo.AllCommits, repo.Name);

            _context.SaveChanges();

            return Response.Updated;
        }
    }

    public Response Delete(int repoId)
    {
        var entity = _context.Repos.Find(repoId);

        if (entity is null)
        {
            return Response.NotFound;
        }

        _context.Repos.Remove(entity);
        _context.SaveChanges();

        return Response.Deleted;
    }



    private ICollection<Commit> getCommitsList(ICollection<int> commitIds) => _context.Commits.Where(c => commitIds.Contains(c.Id)).ToList();

}