namespace GitInsight.Entities;

public class Repo{

    public int Id {get; set;} 
    public string Name {get; set;} 
    public int LatestCommit {get; set;} 
    public virtual ICollection<Commit> AllCommits {get; set;} 

    public Repo(string name){
        Name = name;
        LatestCommit = 0;
        AllCommits = new List<Commit>();
    }
}