namespace GitInsight.Entities;
using GitInsight.Core;

public class RepoRepository : IRepoRepository {

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
            newRepo = new Repo(repo.Name, repo.LatestCommit, getCommitsList(repo.AllCommits));
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
                        select new RepoDTO(r.Id, r.Name, r.LatestCommit, getCommitsAsDTOList(r.AllCommits));

        return repos.ToArray();
    }

    public RepoDTO Find(int repoId)
    {
        var repo = _context.Repos.FirstOrDefault(r => r.Id == repoId);

        return (repo is not null 
            ? new RepoDTO(repo.Id, repo.Name, repo.LatestCommit, getCommitsAsDTOList(repo.AllCommits))
            : null)!;
    }

    public Response Update(RepoUpdateDTO repo)
    {
        var repoUpdate = _context.Repos.Find(repo.Id);

        if (repoUpdate is null) 
        {
            return Response.NotFound;
        }
        else
        {
            //Merges two lists without duplicates
            var newList = getCommitsList(repo.AllCommits).ToList().Union(repoUpdate.AllCommits).ToList();
            
            if (newList.Count <= repo.AllCommits.Count) 
            {
                return Response.Conflict;
            }
            else
            {
                //Saves the new list to a merged and ordered by date list
                repoUpdate.AllCommits = newList.OrderBy(c => c.Date).ToList();
                
                repoUpdate.LatestCommit = repo.LatestCommit;
                _context.SaveChanges();
                    
                return Response.Updated;
            }
            
        }
    }

    public Response Delete(int repoId){
        var entity = _context.Repos.Find(repoId);

        if (entity is null) {
            return Response.NotFound;
        }

        _context.Repos.Remove(entity);
        _context.SaveChanges();
        
        return Response.Deleted;
    }

    private ICollection<Commit> getCommitsList(ICollection<int> commitIds) => _context.Commits.Where(c => commitIds.Contains(c.Id)).ToList();

    private ICollection<int> getCommitsAsDTOList(ICollection<Commit> commits) => commits.Select(c => c.Id).ToList();

}