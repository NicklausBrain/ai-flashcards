using My1kWordsEe.Components;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var app = BuildWebHost(args);
            await app.RunAsync();
        }

        public static WebApplication BuildWebHost(string[] args)
        {
            // default log: Console, Debug, EventSource
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddUserSecrets<Program>();

            var secrets = RequireSecrets(builder);

            builder.Services.AddHttpClient(nameof(StabilityAiService))
                .AddStandardResilienceHandler(options =>
                {
                    options.RateLimiter.DefaultRateLimiterOptions.PermitLimit = 100;
                    options.RateLimiter.DefaultRateLimiterOptions.QueueLimit = 0;
                });

            builder.Services.AddHttpClient(nameof(TartuNlpService))
                .AddStandardResilienceHandler(options =>
                {
                    options.RateLimiter.DefaultRateLimiterOptions.PermitLimit = 100;
                    options.RateLimiter.DefaultRateLimiterOptions.QueueLimit = 0;
                });

            builder.Services.AddSingleton<StabilityAiService>();
            builder.Services.AddSingleton<OpenAiService>();
            builder.Services.AddSingleton<AzureBlobService>();
            builder.Services.AddSingleton<TartuNlpService>();
            builder.Services.AddSingleton<GetOrAddSampleWordCommand>();
            builder.Services.AddSingleton<AddSampleSentenceCommand>();
            builder.Services.AddSingleton<AddSampleWordCommand>();
            builder.Services.AddSingleton<AddAudioCommand>();

            // Blazor-specific services
            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddHubOptions(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(7);
                    options.HandshakeTimeout = TimeSpan.FromSeconds(14);
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseResponseCompression();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            return app;
        }

        private static (
           string openAiKey,
           string stabilityAiKey,
           string azureBlobConnectionString) RequireSecrets(
           WebApplicationBuilder builder)
        {
            var openAiKey =
               builder.Configuration[OpenAiService.ApiSecretKey];

            if (string.IsNullOrWhiteSpace(openAiKey))
            {
                throw new ApplicationException($"{OpenAiService.ApiSecretKey} is missing");
            }

            var stabilityAiKey =
                builder.Configuration[StabilityAiService.ApiSecretKey];

            if (string.IsNullOrWhiteSpace(stabilityAiKey))
            {
                throw new ApplicationException($"{StabilityAiService.ApiSecretKey} is missing");
            }

            var azureBlobConnectionString = builder.Configuration[AzureBlobService.ApiSecretKey];

            if (string.IsNullOrWhiteSpace(azureBlobConnectionString))
            {
                throw new ApplicationException($"{AzureBlobService.ApiSecretKey} is missing");
            }

            return (openAiKey, stabilityAiKey, azureBlobConnectionString);
        }
    }
}
