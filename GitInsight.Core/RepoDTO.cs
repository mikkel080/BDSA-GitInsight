namespace GitInsight.Core;

public record RepoDTO(int Id, [StringLength(100)]string Name, int LatestCommit, ICollection<int> AllCommits);
public record RepoCreateDTO([StringLength(100)]string Name, ICollection<int> AllCommits);
public record RepoUpdateDTO(int Id, [StringLength(100)]string Name, int LatestCommit, ICollection<int> AllCommits);