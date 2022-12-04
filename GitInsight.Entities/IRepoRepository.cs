namespace GitInsight.Entities;

public interface IRepoRepository
{
    Task<(Response Response, int RepoID)> CreateAsync(RepoCreateDTO repo);
    Task<IReadOnlyCollection<RepoDTO>> ReadAsync();
    Task<RepoDTO> FindAsync(int repoId);
    Task<Response> UpdateAsync(RepoUpdateDTO repo);
    Task<Response> DeleteAsync(int repoId);
}