namespace GitInsight.Entities;

public class Commit {
    public int Id {get; set;}
    public Repo Repo {get; set;} 
    public Author Author {get; set;} 
    public DateTime Date {get; set;} 

    public Commit(Repo repo, Author author, DateTime date) {
        Repo = repo;
        Author = author;
        Date = date;
    }
}