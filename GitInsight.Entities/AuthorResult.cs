namespace GitInsight.Entities;

public class AuthorResult
{
    public IEnumerable<Entry> Data { get; }
    public string RepoName { get; }
    public AuthorResult(IEnumerable<Entry> data, string repoName)
    {
        Data = data;
        RepoName = repoName;
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