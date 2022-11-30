namespace GitInsight.Entities;

public class Repo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LatestCommit { get => AllCommits is null || AllCommits.Count() == 0 ? 0 : AllCommits.OrderBy(c => c.Date).ToList().Last().Id ;}
    public virtual ICollection<Commit> AllCommits { get; set;}
    public FrequencyResult? FrequencyResult;
    public AuthorResult? AuthorResult;

    public Repo(string name)
    {
        Name = name;
        AllCommits = new List<Commit>();
    }
}