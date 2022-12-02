
var builder = WebApplication.CreateBuilder(args);

//Building database
var factory = new GitInsightContextFactory();
using var context = factory.CreateDbContext(args);
await context.Database.MigrateAsync();

// Add services to the container.
builder.Services.AddDbContext<GitInsightContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("GitInsight")));

builder.Services.AddScoped<IRepoRepository, RepoRepository>();
builder.Services.AddScoped<ICommitRepository, CommitRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();


// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? "";


app.MapGet("/", () =>
{
    return "Write GitHub username and GitHub repo name in the url! ";
}).WithOpenApi();
app.MapGet("/{GithubName}/{RepoName}", (string GithubName, string RepoName, GitInsightContext context) =>
{
    var program = new GitInsight.Program(context);
    return program.Run(GithubName, RepoName);
}).WithOpenApi();

app.Run();

public partial class ProgramMinimalAPI { }


