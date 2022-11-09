namespace GitInsight.Entities;

public class Commit {
    public int Id {get; set;}
    public int? RepoID { get;set;}
    public virtual Repo Repo {get; set;} 
    public int? AuthorID { get;set;}
    public virtual Author Author {get; set;} 
    public DateTime Date {get; set;} 

    /*Ugly fix to avoid null warning from Repo and Author
    Link to issue https://github.com/dotnet/efcore/issues/12078 */
    public Commit() {
        Author = default!;
        Repo = default!;
    }
}