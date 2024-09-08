using My1kWordsEe.Components;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();

            var openApiKey =
                builder.Configuration["Secrets:OpenAiKey"] ??
                Environment.GetEnvironmentVariable("Secrets_OpenAiKey");

            if (string.IsNullOrWhiteSpace(openApiKey))
            {
                throw new ApplicationException("Secrets:OpenAiKey is missing");
            }

            var stabilityAiKey =
                builder.Configuration["Secrets:StabilityAiKey"] ??
                Environment.GetEnvironmentVariable("Secrets_StabilityAiKey");

            if (string.IsNullOrWhiteSpace(stabilityAiKey))
            {
                throw new ApplicationException("Secrets:StabilityAiKey is missing");
            }

            var azureBlobConnectionString =
                builder.Configuration["Secrets:AzureBlobConnectionString"] ??
                Environment.GetEnvironmentVariable("Secrets_AzureBlobConnectionString");

            if (string.IsNullOrWhiteSpace(azureBlobConnectionString))
            {
                throw new ApplicationException("Secrets:AzureBlobConnectionString is missing");
            }

            builder.Services.AddSingleton(new StabilityAiService(stabilityAiKey));
            builder.Services.AddSingleton(new OpenAiService(openApiKey));
            builder.Services.AddSingleton(new AzureBlobService(azureBlobConnectionString));
            builder.Services.AddSingleton(new TartuNlpService());
            builder.Services.AddSingleton<EnsureWordCommand>();

            // Add services to the container.
            builder.Services.AddRazorComponents()
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

            app.Run();
        }
    }
}
