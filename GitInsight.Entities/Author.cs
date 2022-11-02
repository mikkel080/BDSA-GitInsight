namespace GitInsight.Entities;

public class Author{
    public int Id {get; set;}
    public string Name {get; set;}

    [EmailAddress]
    public string Email {get; set;}
    public ICollection<Commit> AllCommits {get; set;}

    public Author(string name, string email, ICollection<Commit> allCommits){
        Name = name;
        Email = email;
        AllCommits = allCommits;
    }
}