namespace GitInsight.Entities;

public class Repo{

    public int Id {get; set;} 
    public string Name {get; set;} 
    public int LatestCommit {get; set;} 
    public ICollection<Commit> AllCommits {get; set;} 

    public Repo(string name, int latestCommit, ICollection<Commit> allCommits){
        Name = name;
        LatestCommit = latestCommit;
        AllCommits = allCommits;
    }
}