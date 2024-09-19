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

            builder.Services.AddSingleton(new StabilityAiService(secrets.stabilityAiKey));
            builder.Services.AddSingleton((p) => new OpenAiService(
                p.GetRequiredService<ILogger<OpenAiService>>(), secrets.openAiKey));
            builder.Services.AddSingleton((p) => new AzureBlobService(
                p.GetRequiredService<ILogger<AzureBlobService>>(), secrets.azureBlobConnectionString));
            builder.Services.AddSingleton((p) => new TartuNlpService(
                p.GetRequiredService<ILogger<TartuNlpService>>()));
            builder.Services.AddSingleton<GetOrAddSampleWordCommand>();
            builder.Services.AddSingleton<AddSampleSentenceCommand>();
            builder.Services.AddSingleton<AddSampleWordCommand>();
            builder.Services.AddSingleton<AddAudioCommand>();

            // Blazor-specific services
            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents();

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
               builder.Configuration["Secrets:OpenAiKey"];

            if (string.IsNullOrWhiteSpace(openAiKey))
            {
                throw new ApplicationException("Secrets:OpenAiKey is missing");
            }

            var stabilityAiKey =
                builder.Configuration["Secrets:StabilityAiKey"];

            if (string.IsNullOrWhiteSpace(stabilityAiKey))
            {
                throw new ApplicationException("Secrets:StabilityAiKey is missing");
            }

            var azureBlobConnectionString =
                builder.Configuration["Secrets:AzureBlobConnectionString"];

            if (string.IsNullOrWhiteSpace(azureBlobConnectionString))
            {
                throw new ApplicationException("Secrets:AzureBlobConnectionString is missing");
            }

            return (openAiKey, stabilityAiKey, azureBlobConnectionString);
        }
    }
}
