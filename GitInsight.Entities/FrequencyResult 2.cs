namespace GitInsight.Entities;

public class FrequencyResult
{
    public IEnumerable<EntryF> Data { get; }
    public FrequencyResult(IEnumerable<Commit> list, string RepoName)
    {
        Data = new List<EntryF>();
        var q = list.GroupBy(
            (item => item.Date.Date),
            (key, elements) => new
            {
                key = key,
                count = elements.Distinct().Count()
            }
        );

        foreach (var commit in q)
        {
            Data = Data.Append(new EntryF(commit.count, commit.key));
        }
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