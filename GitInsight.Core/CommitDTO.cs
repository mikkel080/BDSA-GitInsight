namespace GitInsight.Core;

public record CommitDTO(int Id, string RepoName, string AuthorName, DateTime Date);
public record CommitCreateDTO(int RepoID, string AuthorName, DateTime Date);

