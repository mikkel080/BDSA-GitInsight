namespace GitInsight.Core;

public interface IAuthorRepository
{
    Task<(Response Response, int AuthorID)> CreateAsync(AuthorCreateDTO author);
    Task<IReadOnlyCollection<AuthorDTO>> ReadAsync();
    Task<AuthorDTO> FindAsync(int authorId);
    Task<Response> UpdateAsync(AuthorUpdateDTO author);
    Task<Response> DeleteAsync(int authorId);
}