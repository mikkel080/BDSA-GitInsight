namespace GitInsight.Entities;

public class AuthorRepository : IAuthorRepository
{

    private readonly GitInsightContext _context;

    public AuthorRepository(GitInsightContext context)
    {
        _context = context;
    }

    public async Task<(Response Response, int AuthorID)> CreateAsync(AuthorCreateDTO author)
    {
        var entity = await (from a in _context.Authors
                            where a.Name == author.Name
                            select a).FirstOrDefaultAsync();
        if (entity is null)
        {
            entity = new Author(author.Name);
            _context.Authors.Add(entity);
            await _context.SaveChangesAsync();
            return (Response.Created, entity.Id);
        }
        else
        {
            return (Response.Conflict, entity.Id);
        }

    }

    public async Task<IReadOnlyCollection<AuthorDTO>> ReadAsync()
    {
        var authors = from a in _context.Authors
                      select new AuthorDTO(a.Id, a.Name, getCommitsAsDTOList(a.AllCommits));
        return await authors.ToListAsync();
    }

    public async Task<AuthorDTO> FindAsync(int authorId)
    {
        var author = await (from a in _context.Authors
                            where a.Id == authorId
                            select new AuthorDTO(a.Id, a.Name, getCommitsAsDTOList(a.AllCommits))).FirstOrDefaultAsync();
        return author!;
    }

    public async Task<Response> UpdateAsync(AuthorUpdateDTO author)
    {
        var entity = await _context.Authors.FindAsync(author.Id);

        if (entity is null)
        {
            return Response.NotFound;
        }
        else
        {
            entity.Name = author.Name;
            await _context.SaveChangesAsync();
            return Response.Updated;
        }
    }

    public async Task<Response> DeleteAsync(int authorId)
    {
        var entity = await _context.Authors.FindAsync(authorId);
        if (entity is null)
        {
            return Response.NotFound;
        }
        _context.Authors.Remove(entity);
        await _context.SaveChangesAsync();
        return Response.Deleted;
    }

    private static ICollection<int> getCommitsAsDTOList(ICollection<Commit> commits) => commits.Select(c => c.Id).ToList();
}