namespace GitInsight;

using LibGit2Sharp;
public class Program
{
    public static void Main(string[] args)
    {
        using (var repo = new Repository(args[0]))
        {
            var commits = repo.Commits.ToList();

            if (args.Count() < 2)
            {
                return;
            }
            if (args[1] == "A")
            {
                AuthorMode(commits);
            }
            else if (args[1] == "F")
            {
                FrequencyMode(commits);
            }
            else
            {
                return;
            }
        }
    }

    static void FrequencyMode(IEnumerable<Commit> list, String prefix = "")
    {
        var q = list.GroupBy(
            (item => item.Author.When.Date),
            (key, elements) => new
            {
                key = key,
                count = elements.Distinct().Count()
            }
        );
        foreach (var commit in q)
        {
            Console.WriteLine(prefix + commit.count + " " + commit.key.ToString(@"yyyy-MM-dd"));
        }
    }

    static void AuthorMode(IEnumerable<Commit> list)
    {
        var q = list.GroupBy(
            (item => item.Author.Name),
            (key, elements) => new
            {
                key = key,
                items = elements
            }
        );
        foreach (var commit in q)
        {
            Console.WriteLine(commit.key);
            FrequencyMode(commit.items, "\t");
            Console.WriteLine();
        }
    }
}
