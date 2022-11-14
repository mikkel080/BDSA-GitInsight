namespace GitInsight;

using LibGit2Sharp;

public sealed class Program {    
    private readonly GitInsightContext _context;
    private readonly AuthorRepository _repositoryAuthor;
    private readonly CommitRepository _repositoryCommit;
    private readonly RepoRepository _repositoryRepos;

    public Program(GitInsightContext context)
    {
        _context = context;
        _repositoryAuthor = new AuthorRepository(_context);
        _repositoryCommit = new CommitRepository(_context);
        _repositoryRepos = new RepoRepository(_context);
    }

    public string getPathOrCloneRepo(string githubName, string repoName)
    {
        //Temp folders does not get deleted themselves so remember to delete
        var path = Path.GetTempPath();
        string existingPath;
        if(path.Contains(@"\"))
        {
            existingPath = path+@$"\{repoName}";
        }
        else
        {
            existingPath = path+@$"/{repoName}";

        }
        if (!Directory.Exists(existingPath))
        {
           return Repository.Clone($"https://github.com/{githubName}/{repoName}.git", path + $"{repoName}");
        }      
        
       return existingPath;
    }

    public string Run(string githubName, string repoName, string mode)
    {
        string output = "";
        using (var repo = new Repository(getPathOrCloneRepo(githubName, repoName)))
        {
            var repoId = SaveTheData(repo);
            var commits = _repositoryCommit.Read().Where(c => c.RepoId == repoId).ToList();
            
            if(mode.Contains("A"))
            {
                foreach (var c in AuthorMode(commits))
                {
                    output += c + "\n";
                }
            }
            else if(mode.Contains("F"))
            {
                foreach (var c in FrequencyMode(commits))
                {
                    output += c + "\n";
                }
            }
            return output;
        }  
    }

    List<string> FrequencyMode(IEnumerable<CommitDTO> list, string prefix = "")
    {
        var stringList = new List<string>();
        var q =  list.GroupBy(
            (item => item.Date),
            (key, elements) => new 
            {
                key = key, 
                count = elements.Distinct().Count()
            }
        );
        foreach (var commit in q)
        {
            stringList.Add(prefix+commit.count +" "+commit.key.ToString(@"yyyy-MM-dd"));

        }
        return stringList;
    }
    
     List<string> AuthorMode(IEnumerable<CommitDTO> list)
     {
        var stringList = new List<string>();
        var q = list.GroupBy(
            (item => item.AuthorName),
            (key, elements) => new 
            {
                key = key,
                items = elements
            }
        );
        foreach (var commit in q)
        {
            stringList.Add(commit.key);
            stringList.AddRange(FrequencyMode(commit.items, "\t"));
        }
        return stringList;
    }
    
    int SaveTheData(Repository repo)
    {
        var (response, repoId) = _repositoryRepos.Create(new RepoCreateDTO(repo.Info.Path, new List<int>()));
        if (repoId == -1)
        {
            return _repositoryRepos.Read().Where(r => r.Name == repo.Info.Path).First().Id;
        }
        foreach (var commit in repo.Commits.ToList()) 
        {
            _repositoryCommit.Create(new CommitCreateDTO(repoId, commit.Author.Name, commit.Author.When.Date));
        }
        return repoId;
    }
}
