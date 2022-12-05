namespace GitInsight.Blazor.Data
{
    public class Rootobject
    {
        public Repositoryidentifier RepositoryIdentifier { get; set; }
        public Frequencyresult FrequencyResult { get; set; }
        public Authorresult AuthorResult { get; set; }
        public Forkresult ForkResult { get; set; }

        public Rootobject(Repositoryidentifier repositoryIdentifier, Frequencyresult frequencyResult, Authorresult authorResult, Forkresult forkResult)
        {
            RepositoryIdentifier = repositoryIdentifier;
            FrequencyResult = frequencyResult;
            AuthorResult = authorResult;
            ForkResult = forkResult;
        }
    }

    public class Repositoryidentifier
    {
        public string Organization { get; set; }
        public string Repository { get; set; }

        public Repositoryidentifier(string organization, string repository)
        {
            Repository = repository;
            Organization = organization;
        }
    }

    public class Frequencyresult
    {
        public Datum[] Data { get; set; }

        public Frequencyresult(Datum[] data)
        {
            Data = data;
        }
    }

    public class Datum
    {
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }

    public class Authorresult
    {
        public Datum1[] Data { get; set; }
        public Authorresult(Datum1[] data)
        {
            Data = data;
        }
    }

    public class Datum1
    {
        public string AuthorName { get; set; }
        public Frequencyresults FrequencyResults { get; set; }

        public Datum1( string authorName, Frequencyresults frequencyresults)
        {
            AuthorName = authorName;
            FrequencyResults = frequencyresults;
        }
    }

    public class Frequencyresults
    {
        public Datum2[] Data { get; set; }

        public Frequencyresults(Datum2[] data)
        {
            Data = data;
        }
    }

    public class Datum2
    {
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }

    public class Forkresult
    {
        public Repositoryidentifier[] RepositoryIdentifiers { get; set; }

        public Forkresult(Repositoryidentifier[] repositoryidentifiers)
        {
            RepositoryIdentifiers = repositoryidentifiers;
        }
    }

}
