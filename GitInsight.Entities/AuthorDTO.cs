namespace GitInsight.Core;

public record AuthorDTO(int Id, string Name, ICollection<int> AllCommits);
public record AuthorCreateDTO([StringLength(100)] string Name, ICollection<int> AllCommits);
public record AuthorUpdateDTO(int Id, [StringLength(100)] string Name, ICollection<int> AllCommits);