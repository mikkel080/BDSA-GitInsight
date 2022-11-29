namespace GitInsight.Core;

public interface ICommitRepository
{
    Task<(Response Response, int CommitID)> CreateAsync(CommitCreateDTO commit);
    Task<IReadOnlyCollection<CommitDTO>> ReadAsync();
    Task<CommitDTO> FindAsync(int commitId);
    Task<Response> DeleteAsync(int commitId);
}