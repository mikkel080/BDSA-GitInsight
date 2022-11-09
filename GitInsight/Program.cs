namespace GitInsight;

using LibGit2Sharp;

public sealed class Program {    
    private readonly GitInsightContext _context;
    private readonly AuthorRepository _repositoryAuthor;
    private readonly CommitRepository _repositoryCommit;
    private readonly RepoRepository _repositoryRepos;

    public Program()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<GitInsightContext>();
        builder.UseSqlite(connection);
        var context = new GitInsightContext(builder.Options);
        context.Database.EnsureCreated();

        _context = context;
        _repositoryAuthor = new AuthorRepository(_context);
        _repositoryCommit = new CommitRepository(_context);
        _repositoryRepos = new RepoRepository(_context);
    }

    public string Run(string path, string mode){
        string output = "";
        using (var repo = new Repository(path)){
            var repoId = SaveTheData(repo);
            var commits = _repositoryCommit.Read().Where(c => c.RepoId == repoId).ToList();
            
            if(mode.Contains("A")){
                foreach (var c in AuthorMode(commits)){
                    output += c + "\n";
                }
            }else if(mode.Contains("F")){
                foreach (var c in FrequencyMode(commits)){
                    output += c + "\n";
                }
            }
            return output;
        }  
    }

    List<string> FrequencyMode(IEnumerable<CommitDTO> list, String prefix = ""){
        var stringList = new List<string>();
        var q =  list.GroupBy(
            (item => item.Date),
            (key, elements) => new {
                key = key, 
                count = elements.Distinct().Count()
            }
        );
        foreach (var commit in q){
            stringList.Add(prefix+commit.count +" "+commit.key.ToString(@"yyyy-MM-dd"));
        }
        return stringList;
    }

     List<string> AuthorMode(IEnumerable<CommitDTO> list){
        var stringList = new List<string>();
        var q = list.GroupBy(
            (item => item.AuthorName),
            (key, elements) => new {
                key = key,
                items = elements
            }
        );
        foreach (var commit in q){
            stringList.Add(commit.key);
            stringList.AddRange(FrequencyMode(commit.items, "\t"));
        }
        return stringList;
    }
    int SaveTheData(Repository repo){
        var (response, repoId) = _repositoryRepos.Create(new RepoCreateDTO(repo.Info.Path, new List<int>()));
        if (repoId == -1){
            return _repositoryRepos.Read().Where(r => r.Name == repo.Info.Path).First().Id;
        }
        foreach (var commit in repo.Commits.ToList()) {
            _repositoryCommit.Create(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.Date));
        }
        return repoId;
    }
}
