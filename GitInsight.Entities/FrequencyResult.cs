namespace GitInsight.Entities;

public class FrequencyResult
{
    public IEnumerable<EntryF> Data { get; }
    public string RepoName { get; }
    public FrequencyResult(IEnumerable<EntryF> data, string repoName)
    {
        Data = data;
        RepoName = repoName;
    }

    public List<String> ToStringList()
    {
        var strings = new List<String>();
        foreach (var item in Data)
        {
            strings.Append(item.Count + " " + item.Date.ToString(@"yyyy-MM-dd"));
        }
        return strings;
    }
}
public record EntryF(int Count, DateTime Date);