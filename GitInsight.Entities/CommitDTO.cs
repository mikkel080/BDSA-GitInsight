namespace GitInsight.Entities;

public record CommitDTO(int Id, int RepoId, string AuthorName, DateTime Date);
public record CommitCreateDTO(int RepoID, string AuthorName, DateTime Date);

