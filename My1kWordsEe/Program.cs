using My1kWordsEe.Components;
using My1kWordsEe.Services;

namespace My1kWordsEe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var openApiKey = builder.Configuration["Secrets:OpenAiKey"];

            if (string.IsNullOrWhiteSpace(openApiKey))
            {
                throw new ApplicationException("Secrets:OpenAiKey is missing");
            }
            builder.Services.AddSingleton(new OpenAiService(openApiKey));

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
