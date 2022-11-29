namespace GitInsight.Entities;

public class AuthorResult
{
    public IEnumerable<Entry> Data { get; }
    public AuthorResult(IEnumerable<Commit> list, string RepoName)
    {
        Data = new List<Entry>();
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
            Data = Data.Append(new Entry(commit.key, new FrequencyResult(commit.items, RepoName)));
        }
    }
    public List<String> ToStringList()
    {
        var strings = new List<String>();
        foreach (var item in Data)
        {
            strings.Add(item.AuthorName);
            strings.AddRange(item.FrequencyResults.ToStringList());
        }
        return strings;

    }

}
public record Entry(string AuthorName, FrequencyResult FrequencyResults);