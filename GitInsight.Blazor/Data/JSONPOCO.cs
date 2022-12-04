namespace GitInsight.Blazor.Data;

public class Rootobject
{
    public Repositoryidentifier ?RepositoryIdentifier { get; set; }
    public Frequencyresult ?FrequencyResult { get; set; }
    public Authorresult ?AuthorResult { get; set; }
    public Forkresult ?ForkResult { get; set; }
}

public class Repositoryidentifier
{
    public string ?Organization { get; set; }
    public string ?Repository { get; set; }
}

public class Frequencyresult
{
    public Datum[] ?Data { get; set; }
}

public class Datum
{
    public int Count { get; set; }
    public DateTime Date { get; set; }
}

public class Authorresult
{
    public Datum1[] ?Data { get; set; }
}

public class Datum1
{
    public string ?AuthorName { get; set; }
    public Frequencyresults ?FrequencyResults { get; set; }
}

public class Frequencyresults
{
    public Datum2[] ?Data { get; set; }
}

public class Datum2
{
    public int Count { get; set; }
    public DateTime Date { get; set; }
}

public class Forkresult
{
    public Repositoryidentifier[] ?RepositoryIdentifiers { get; set; }
}
