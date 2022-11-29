namespace GitInsight.Entities;
using GitInsight.Core;

public class AuthorRepository : IAuthorRepository
{

    private readonly GitInsightContext _context;

    public AuthorRepository(GitInsightContext context)
    {
        _context = context;
    }

    public (Response Response, int AuthorID) Create(AuthorCreateDTO author)
    {
        var entity = (from a in _context.Authors
                      where a.Name == author.Name
                      select a).FirstOrDefault();
        if (entity is null)
        {
            entity = new Author(author.Name);
            _context.Authors.Add(entity);
            _context.SaveChanges();
            return (Response.Created, entity.Id);
        }
        else
        {
            return (Response.Conflict, entity.Id);
        }

    }

    public IReadOnlyCollection<AuthorDTO> Read()
    {
        var authors = from a in _context.Authors
                      select new AuthorDTO(a.Id, a.Name, getCommitsAsDTOList(a.AllCommits));
        return authors.ToList();
    }

    public AuthorDTO Find(int authorId)
    {
        var author = (from a in _context.Authors
                      where a.Id == authorId
                      select a).FirstOrDefault();
        if (author is null)
        {
            return null!;
        }
        else
        {
            return new AuthorDTO(author.Id, author.Name, getCommitsAsDTOList(author.AllCommits));
        }
    }

    public Response Update(AuthorUpdateDTO author)
    {
        var entity = _context.Authors.Find(author.Id);

        if (entity is null)
        {
            return Response.NotFound;
        }
        else
        {
            entity.Name = author.Name;
            _context.SaveChanges();
            return Response.Updated;
        }
    }

    public Response Delete(int authorId)
    {
        var entity = _context.Authors.Find(authorId);
        if (entity is null)
        {
            return Response.NotFound;
        }
        _context.Authors.Remove(entity);
        _context.SaveChanges();
        return Response.Deleted;
    }

    private static ICollection<int> getCommitsAsDTOList(ICollection<Commit> commits) => commits.Select(c => c.Id).ToList();
}