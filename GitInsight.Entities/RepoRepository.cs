namespace GitInsight.Entities;
using GitInsight.Core;

public class RepoRepository : IRepoRepository
{

    private readonly GitInsightContext _context;

    public RepoRepository(GitInsightContext context)
    {
        _context = context;
    }

    public async Task<(Response Response, int RepoID)> CreateAsync(RepoCreateDTO repo)
    {
        var newRepo = await _context.Repos.FirstOrDefaultAsync(c => c.Name.Equals(repo.Name));

        if (newRepo is null)
        {
            newRepo = new Repo(repo.Name);
            _context.Repos.Add(newRepo);
            await _context.SaveChangesAsync();
        }
        else
        {
            _context.Entry(newRepo).Collection(r => r.AllCommits).Load();
            return (Response.AlreadyExists, newRepo.Id);
        }

        return (Response.Created, newRepo.Id);
    }

    public async Task<IReadOnlyCollection<RepoDTO>> ReadAsync()
    {
        var repos = from r in _context.Repos
                    orderby r.Id
                    select new RepoDTO(r.Id, r.Name, r.LatestCommit, r.AllCommits.Select(c => c.Id).ToList(), r.AuthorResult!, r.FrequencyResult!);

        return await repos.ToArrayAsync();
    }

    public async Task<RepoDTO> FindAsync(int repoId)
    {
        var repo = await (from r in _context.Repos
                          where r.Id == repoId
                          select new RepoDTO(r.Id, r.Name, r.LatestCommit, r.AllCommits.Select(c => c.Id).ToList(), r.AuthorResult!, r.FrequencyResult!)).FirstOrDefaultAsync();

        return repo!;
    }

    public async Task<Response> UpdateAsync(RepoUpdateDTO repoUpdate)
    {
        var repo = await _context.Repos.FindAsync(repoUpdate.Id);

        if (repo is null)
        {
            return Response.NotFound;
        }
        else
        {
            if (!repo.Name.Equals(repoUpdate.Name) && _context.Repos.Where(r => r.Name.Equals(repoUpdate.Name)).FirstOrDefault() is null)
            {
                repo.Name = repoUpdate.Name;
            }

            repo.AuthorResult = repoUpdate.AuthorResult;
            repo.FrequencyResult = repoUpdate.FrequencyResult;

            await _context.SaveChangesAsync();

            return Response.Updated;
        }
    }

    public async Task<Response> DeleteAsync(int repoId)
    {
        var entity = await _context.Repos.FindAsync(repoId);

        if (entity is null)
        {
            return Response.NotFound;
        }

        _context.Repos.Remove(entity);
        await _context.SaveChangesAsync();

        return Response.Deleted;
    }



    private async Task<ICollection<Commit>> getCommitsListAsync(ICollection<int> commitIds) => await _context.Commits.Where(c => commitIds.Contains(c.Id)).ToListAsync();

}