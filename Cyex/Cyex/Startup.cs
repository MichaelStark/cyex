using Cyex.Interfaces;
using Cyex.Services;

namespace Cyex
{
    public class Startup(IConfiguration configuration)
    {
        private IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddSingleton<IScanService, ScanService>();
            services.AddSingleton<IThirdPartyService>(provider =>
            {
                var environment = provider.GetRequiredService<IWebHostEnvironment>();

                var accessToken = environment.IsDevelopment()
                    ? Configuration["GitHubAccessToken"]
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
            });
            services.AddSingleton<NpmPackageInfoService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}