//Learn more about minimal api -> https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-7.0&tabs=visual-studio-code

var builder = WebApplication.CreateBuilder(args);
        
builder.Services.AddDbContext<GitInsightContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("GitInsight")));

// Add services to the container.
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
app.MapGet("/", () => "Write GitHub username and GitHub repo name in the url!");
app.MapGet("/{GithubName}/{RepoName}", (string GithubName, string RepoName, GitInsightContext context) => {
    var program = new GitInsight.Program(context);
    return program.Run(GithubName, RepoName);
});

app.Run();

public partial class ProgramAPI { }

