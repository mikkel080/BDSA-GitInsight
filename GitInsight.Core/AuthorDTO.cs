namespace GitInsight.Core;

public record AuthorDTO(int Id, string Name, string Email, ICollection<int> AllCommits);
public record AuthorCreateDTO([StringLength(100)]string Name, [EmailAddress, StringLength(100)]string Email, ICollection<int> AllCommits);
public record AuthorUpdateDTO(int Id, [StringLength(100)]string Name, [EmailAddress, StringLength(100)]string Email, ICollection<int> AllCommits);