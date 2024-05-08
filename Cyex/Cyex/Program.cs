using Cyex.Interfaces;
using Cyex.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IThirdPartyService>(provider =>
{
    var environment = provider.GetRequiredService<IWebHostEnvironment>();
    var configuration = provider.GetRequiredService<IConfiguration>();

    var accessToken = environment.IsDevelopment()
        ? configuration["GitHubAccessToken"]
        : Environment.GetEnvironmentVariable("GITHUB_ACCESS_TOKEN");

    if (string.IsNullOrEmpty(accessToken))
    {
        throw new InvalidOperationException(
            environment.IsDevelopment()
                ? "[DEV] GitHub access token is not configured in appsettings.json 'GitHubAccessToken'."
                : "GitHub access token is not configured in environment variable GITHUB_ACCESS_TOKEN."
        );
    }

    return new GitHubService(new HttpClient(), accessToken);
    // return new AnotherOneService(httpClient, accessToken);
});
builder.Services.AddTransient<IScanService, ScanService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();