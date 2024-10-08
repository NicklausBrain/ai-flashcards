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

            builder.Services.AddHttpClient(nameof(StabilityAiClient))
                .AddStandardResilienceHandler(options =>
                {
                    options.RateLimiter.DefaultRateLimiterOptions.PermitLimit = 100;
                    options.RateLimiter.DefaultRateLimiterOptions.QueueLimit = 0;
                });

            builder.Services.AddHttpClient(nameof(TartuNlpClient))
                .AddStandardResilienceHandler(options =>
                {
                    options.RateLimiter.DefaultRateLimiterOptions.PermitLimit = 100;
                    options.RateLimiter.DefaultRateLimiterOptions.QueueLimit = 0;
                });

            builder.Services.AddSingleton<StabilityAiClient>();
            builder.Services.AddSingleton<OpenAiClient>();
            builder.Services.AddSingleton<AzureStorageClient>();
            builder.Services.AddSingleton<TartuNlpClient>();
            builder.Services.AddSingleton<GetOrAddSampleWordCommand>();
            builder.Services.AddSingleton<AddSampleSentenceCommand>();
            builder.Services.AddSingleton<AddSampleWordCommand>();
            builder.Services.AddSingleton<AddAudioCommand>();
            builder.Services.AddSingleton<CheckEnTranslationCommand>();

            // Blazor-specific services
            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents(options =>
                {
                    options.DisconnectedCircuitMaxRetained = 10;
                    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(1);
                    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
                    options.MaxBufferedUnacknowledgedRenderBatches = 10;
                })
                .AddHubOptions(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(15);
                    options.HandshakeTimeout = TimeSpan.FromSeconds(10);
                    options.MaximumReceiveMessageSize = 16 * 1024;
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
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
               builder.Configuration[OpenAiClient.ApiSecretKey];

            if (string.IsNullOrWhiteSpace(openAiKey))
            {
                throw new ApplicationException($"{OpenAiClient.ApiSecretKey} is missing");
            }

            var stabilityAiKey =
                builder.Configuration[StabilityAiClient.ApiSecretKey];

            if (string.IsNullOrWhiteSpace(stabilityAiKey))
            {
                throw new ApplicationException($"{StabilityAiClient.ApiSecretKey} is missing");
            }

            var azureBlobConnectionString = builder.Configuration[AzureStorageClient.ApiSecretKey];

            if (string.IsNullOrWhiteSpace(azureBlobConnectionString))
            {
                throw new ApplicationException($"{AzureStorageClient.ApiSecretKey} is missing");
            }

            return (openAiKey, stabilityAiKey, azureBlobConnectionString);
        }
    }
}
