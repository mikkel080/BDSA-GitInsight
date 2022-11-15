namespace GitInsight.Core;

public interface IAuthorRepository
{
    (Response Response, int AuthorID) Create(AuthorCreateDTO author);
    IReadOnlyCollection<AuthorDTO> Read();
    AuthorDTO Find(int authorId);
    Response Update(AuthorUpdateDTO author);
    Response Delete(int authorId);
}