namespace GitInsight.Core;

public interface ICommitRepository
{
    (Response Response, int CommitID) Create(CommitCreateDTO commit);
    IReadOnlyCollection<CommitDTO> Read();
    CommitDTO Find(int commitId);
    Response Delete(int commitId);
}