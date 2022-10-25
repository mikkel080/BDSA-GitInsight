using LibGit2Sharp;
class Program {
    static void Main(string[] args){
        using (var repo = new Repository(args[0])){
            var commits = repo.Commits.ToList();
            Console.WriteLine(commits.Count());
            foreach (var commit in commits){
                Console.WriteLine(commit.Author.Name + " " + commit.Author.When);
            }
            FrequencyMode(commits);
        }
    }

    static void FrequencyMode(List<Commit> list){
        var q = list.GroupBy(
            (item => item.Author.When.Date),
            (key, elements) => new {
                key = key, 
                count = elements.Distinct().Count()
            }
        );
        foreach (var commit in q){
            Console.WriteLine(commit.key.ToShortDateString() + " " + commit.count);
        }
    }

    static void AuthorMode(){

    }
}
