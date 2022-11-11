using Microsoft.AspNetCore.Mvc;
using GitInsight.Core;

namespace GitInsight.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RepositoryController : ControllerBase
{
    private readonly ILogger<RepositoryController> _logger;

    private readonly IRepoRepository _repository;

    public RepositoryController(ILogger<RepositoryController> logger, IRepoRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet]
    public async Task<IEnumerable<RepoDTO>> Get() => _repository.Read();

    [HttpGet("{personName}")]
    public async Task<ActionResult<RepoDTO>> Get(string personName)
    {
        return new RepoDTO(1, personName,1, new List<int>());
    }
}