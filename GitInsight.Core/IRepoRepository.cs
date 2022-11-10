namespace GitInsight.Core;

public interface IRepoRepository{
    (Response Response, int RepoID) Create(RepoCreateDTO repo);
    IReadOnlyCollection<RepoDTO> Read();
    RepoDTO Find(int repoId);
    Response Update(RepoUpdateDTO repo);
    Response Delete(int repoId);
}