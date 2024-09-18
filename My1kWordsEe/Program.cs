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
            // default log: Console, Debug, EventSource
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();

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

            builder.Services.AddSingleton(new StabilityAiService(stabilityAiKey));
            builder.Services.AddSingleton((p) => new OpenAiService(
                p.GetRequiredService<ILogger<OpenAiService>>(), openAiKey));
            builder.Services.AddSingleton((p) => new AzureBlobService(
                p.GetRequiredService<ILogger<AzureBlobService>>(), azureBlobConnectionString));
            builder.Services.AddSingleton((p) => new TartuNlpService(
                p.GetRequiredService<ILogger<TartuNlpService>>()));
            builder.Services.AddSingleton<GetOrAddSampleWordCommand>();
            builder.Services.AddSingleton<AddSampleSentenceCommand>();
            builder.Services.AddSingleton<AddSampleWordCommand>();
            builder.Services.AddSingleton<AddAudioCommand>();

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
