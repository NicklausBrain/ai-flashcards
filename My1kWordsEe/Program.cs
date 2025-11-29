using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using My1kWordsEe.Components;
using My1kWordsEe.Components.Account;
using My1kWordsEe.Data;
using My1kWordsEe.Models;
using My1kWordsEe.Models.Games;
using My1kWordsEe.Models.Games.Generation;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Db;
using My1kWordsEe.Services.Scoped;

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

            builder.Services.AddApplicationInsightsTelemetry();

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

            // singletons
            builder.Services.AddSingleton<EtWordsCache>();
            builder.Services.AddSingleton<StabilityAiClient>();
            builder.Services.AddSingleton<OpenAiClient>();
            builder.Services.AddSingleton<AzureStorageClient>();
            builder.Services.AddSingleton<AudioStorageClient>();
            builder.Services.AddSingleton<ImageStorageClient>();
            builder.Services.AddSingleton<FavoritesStorageClient>();
            builder.Services.AddSingleton<SamplesStorageClient>();
            builder.Services.AddSingleton<WordStorageClient>();
            builder.Services.AddSingleton<FormsStorageClient>();
            builder.Services.AddSingleton<UrlService>();
            builder.Services.AddSingleton<TartuNlpClient>();

            builder.Services.AddSingleton<AddEtWordCommand>();
            builder.Services.AddSingleton<GetOrAddEtWordCommand>();
            builder.Services.AddSingleton<GetEtSampleSentencesQuery>();
            builder.Services.AddSingleton<AddEtSampleSentenceCommand>();
            builder.Services.AddSingleton<DeleteEtSampleSentenceCommand>();
            builder.Services.AddSingleton<AddEtFormsCommand>();
            builder.Services.AddSingleton<GetOrAddEtFormsCommand>();

            builder.Services.AddSingleton<GenerateImageCommand>();
            builder.Services.AddSingleton<GenerateSpeechCommand>();
            builder.Services.AddSingleton<CheckEnTranslationCommand>();
            builder.Services.AddSingleton<CheckEtTranslationCommand>();
            builder.Services.AddSingleton<CheckEeListeningCommand>();
            builder.Services.AddSingleton<GetFavoritesQuery>();
            builder.Services.AddSingleton<AddToFavoritesCommand>();
            builder.Services.AddSingleton<RemoveFromFavoritesCommand>();
            builder.Services.AddSingleton<RedoSampleWordCommand>();
            builder.Services.AddSingleton<ReorderFavoritesCommand>();
            builder.Services.AddSingleton<UpdateScoreCommand>();

            // scoped states
            builder.Services.AddScoped<FavoritesStateContainer>();
            builder.Services.AddScoped<NextWordSelector>();
            builder.Services.AddScoped<Et1kWords>();
            builder.Services.AddScoped<TranslateToEnGameFactory>();
            builder.Services.AddScoped<TranslateToEtGameFactory>();
            builder.Services.AddScoped<ListenToEeGameFactory>();
            builder.Services.AddScoped<Word2WordMatchGameFactory>();

            // Blazor-specific services
            builder.Services
                .AddBlazorBootstrap()
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

            AddAuth(builder, secrets.azureCosmosConnectionString);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            // check MapStaticAssets instead
            app.UseStaticFiles();
            app.UseAntiforgery();


            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            return app;
        }

        private static (
           string openAiKey,
           string stabilityAiKey,
           string azureBlobConnectionString,
           string azureCosmosConnectionString) RequireSecrets(
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

            const string CosmosSecretKey = "Secrets:AzureCosmosConnectionString";
            var azureCosmosConnectionString = builder.Configuration[CosmosSecretKey];
            if (string.IsNullOrWhiteSpace(azureCosmosConnectionString))
            {
                throw new ApplicationException($"{CosmosSecretKey} is missing");
            }

            return (openAiKey, stabilityAiKey, azureBlobConnectionString, azureCosmosConnectionString);
        }

        private static void AddAuth(WebApplicationBuilder builder, string azureCosmosConnectionString)
        {
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseCosmos(azureCosmosConnectionString, "Auth");
            });

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            builder.Services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
        }
    }
}