namespace GitInsight.Entities;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Commit> AllCommits { get; set; }

    public Author(string name)
    {
        Name = name;
        AllCommits = new List<Commit>();
    }
}